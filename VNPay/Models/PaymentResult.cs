using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VNPay.Models
{
    /// <summary>
    /// Phản hồi từ VNPay sau khi thực hiện giao dịch thanh toán.
    /// </summary>
    public class PaymentResult
    {
        /// <summary>
        /// Mã tham chiếu giao dịch (Transaction reference). Đây là mã số duy nhất dùng để xác định giao dịch.
        /// </summary>
        public long PaymentId { get; set; }

        /// <summary>
        /// Trạng thái thành công của giao dịch.
        /// Giá trị là <c>true</c> nếu chữ ký chính xác, <see cref="PaymentResponse.Code"/> và <see cref="TransactionStatus"/> đều bằng <c>0</c>.
        /// </summary>
        public bool IsSuccess { get; set; }

        /// <summary>
        /// Số tiền thực hiện giao dịch.
        /// </summary>
        public double Money { get; set; }

        /// <summary>
        /// Thông tin mô tả nội dung thanh toán, viết bằng tiếng Việt không dấu.
        /// </summary>
        public string Description { get; set; } = default!;

        /// <summary>
        /// Thời gian phản hồi từ VNPay, được ghi nhận tại thời điểm giao dịch kết thúc.
        /// </summary>
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// Mã giao dịch được ghi nhận trên hệ thống VNPay, đại diện cho giao dịch duy nhất tại VNPay.
        /// </summary>
        public long VnpayTransactionId { get; set; }

        /// <summary>
        /// Phương thức thanh toán được sử dụng, VD: thẻ tín dụng, ví điện tử, hoặc chuyển khoản ngân hàng.
        /// </summary>
        public string PaymentMethod { get; set; } = default!;

        /// <summary>
        /// Phản hồi chi tiết từ hệ thống VNPay về giao dịch.
        /// </summary>
        public PaymentResponse PaymentResponse { get; set; } = default!;

        /// <summary>
        /// Trạng thái giao dịch sau khi thực hiện, VD: Chờ xử lý, thành công hoặc thất bại.
        /// </summary>
        public TransactionStatus TransactionStatus { get; set; } = default!;

        /// <summary>
        /// Thông tin ngân hàng liên quan đến giao dịch, bao gồm tên ngân hàng và mã ngân hàng.
        /// </summary>
        public BankingInfor BankingInfor { get; set; } = default!;
    }
}
