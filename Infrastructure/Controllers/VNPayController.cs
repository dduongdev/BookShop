using Entities;
using Microsoft.AspNetCore.Mvc;
using UseCases;
using VNPay;
using VNPay.Enums;
using VNPay.Models;
using VNPay.Utilities;

namespace Infrastructure.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VNPayController : Controller
    {
        private readonly IVNPay _vnpay;
        private readonly IConfiguration _configuration;
        private readonly PaymentTransactionManager _paymentTransactionManager;

        public VNPayController(IVNPay vnpay, IConfiguration configuration, PaymentTransactionManager paymentTransactionManager)
        {
            _vnpay = vnpay;
            _configuration = configuration;
            _paymentTransactionManager = paymentTransactionManager;

            _vnpay.Initialize(
                _configuration["Vnpay:TmnCode"] ?? throw new ArgumentNullException("TmnCode is not defined."),
                _configuration["Vnpay:HashSecret"] ?? throw new ArgumentNullException("HashSecret is not defined."),
                _configuration["Vnpay:BaseUrl"] ?? throw new ArgumentNullException("BaseUrl is not defined."),
                _configuration["Vnpay:CallbackUrl"] ?? throw new ArgumentNullException("CallbackUrl is not defined.")
            );

        }

        /// <summary>
        /// Tạo url thanh toán.
        /// </summary>
        /// <param name="money">Số tiền phải thanh toán.</param>
        /// <param name="description">Mô tả giao dịch.</param>
        /// <returns></returns>
        [HttpGet("CreatePaymentUrl")]
        public ActionResult<string> CreatePaymentUrl(int orderId, double money, string description)
        {
            try
            {
                var ipAddress = NetworkHelper.GetIpAddress(HttpContext);

                var request = new PaymentRequest
                {
                    PaymentId = DateTime.Now.Ticks,
                    Money = money,
                    Description = description,
                    IpAddress = ipAddress,
                    BankCode = BankCode.ANY, // Tùy chọn. Mặc định là tất cả phương thức giao dịch
                    CreatedDate = DateTime.Now, // Tùy chọn. Mặc định là thời điểm hiện tại
                    Currency = Currency.VND, // Tùy chọn. Mặc định là VND (Việt Nam đồng)
                    Language = DisplayLanguage.Vietnamese // Tùy chọn. Mặc định là tiếng Việt
                };

                var paymentUrl = _vnpay.GetPaymentUrl(request);

                return Redirect(paymentUrl);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Thực hiện hành động sau khi thanh toán. URL này cần được khai báo với VNPay trước.
        /// </summary>
        /// <returns></returns>
        [HttpGet("IpnAction")]
        public async Task<IActionResult> IpnAction()
        {
            if (Request.QueryString.HasValue)
            {
                try
                {
                    var paymentResult = _vnpay.GetPaymentResult(Request.Query);
                    if (paymentResult.IsSuccess)
                    {
                        var paymentDescription = paymentResult.Description;
                        int lastSpaceIndex = paymentDescription.LastIndexOf(' ');
                        if (lastSpaceIndex != -1 && int.TryParse(paymentDescription.Substring(lastSpaceIndex + 1), out int orderId))
                        {
                            var paymentTransaction = new PaymentTransaction
                            {
                                OrderId = orderId,
                                BankCode = paymentResult.BankingInfor.BankCode,
                                Amount = (decimal)(paymentResult.Money / 100),
                                TransactionId = paymentResult.VnpayTransactionId
                            };

                            var result = await _paymentTransactionManager.AddAsync(paymentTransaction);
                            if (result.ResultCode == UseCases.TaskResults.AtomicTaskResultCodes.Failed)
                            {
                                return BadRequest("An error occured.");
                            }
                        }
                        else
                        {
                            return NotFound();
                        }

                        return Ok();
                    }

                    return BadRequest("Thanh toán thất bại.");
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }

            return NotFound("Không tìm thấy thông tin thanh toán.");
        }

        /// <summary>
        /// Trả kết quả thanh toán về cho người dùng
        /// </summary>
        /// <returns></returns>
        [HttpGet("Callback")]
        public ActionResult<string> Callback()
        {
            if (Request.QueryString.HasValue)
            {
                try
                {
                    var paymentResult = _vnpay.GetPaymentResult(Request.Query);
                    var resultDescription = $"{paymentResult.PaymentResponse.Description}. {paymentResult.TransactionStatus.Description}.";

                    if (paymentResult.IsSuccess)
                    {
                        return Ok("Thanh toán thành công.");
                    }

                    return BadRequest(resultDescription);
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }

            return NotFound("Không tìm thấy thông tin thanh toán.");
        }
    }
}
