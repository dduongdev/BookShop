using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VNPay.Models;

namespace VNPay
{
    public interface IVNPay
    {
        /// <summary>
        /// Khởi tạo các tham số cần thiết cho giao dịch thanh toán với VNPay
        /// Phương thức này thiết lập các tham số như mã cửa hàng, mật khẩu bảo mật và URL callback.
        /// </summary>
        /// <param name="tmnCode">Mã cửa hàng của bạn trên VNPay</param>
        /// <param name="hashSecret">Mật khẩu bảo mật dùng để mã hóa và xác thực giao dịch.</param>
        /// <param name="baseUrl">URL của trang web thanh toán, mặc định sử dụng URL của môi trường Sandbox.</param>
        /// <param name="callbackUrl">URL mà VNPay sẽ gọi lại sau khi giao dịch hoàn tất.</param>
        /// <param name="version">Phiên bản API mà bạn đang sử dụng.</param>
        /// <param name="orderType">Loại đơn hàng.</param>
        void Initialize(
            string tmnCode,
            string hashSecret,
            string baseUrl,
            string callbackUrl,
            string version = "2.1.0",
            string orderType = "other");

        /// <summary>
        /// Tạo URL thanh toán cho giao dịch dựa trên các tham số trong yêu cầu thanh toán.
        /// </summary>
        /// <param name="request">Thông tin yêu cầu thanh toán, bao gồm các tham số như mã giao dịch, số tiền, mô tả, ...</param>
        /// <returns>URL thanh toán để chuyển hướng người dùng tới trang thanh toán của VNPay.</returns>
        string GetPaymentUrl(PaymentRequest request);

        /// <summary>
        /// Thực hiện giao dịch thanh toán và trả về kết quả.
        /// Phương thức này được gọi khi nhận được thông tin callback từ VNPay.
        /// </summary>
        /// <param name="parameters">Thông tin các tham số trả về từ VNPay qua callback.</param>
        /// <returns></returns>
        PaymentResult GetPaymentResult(IQueryCollection parameters);
    }
}
