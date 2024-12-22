using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VNPay.Enums;

namespace VNPay.Models
{
    /// <summary>
    /// Yêu cầu thanh toán gửi đến cổng thanh toán VNPay
    /// </summary>
    public class PaymentRequest
    {
        /// <summary>
        /// Mã tham chiếu giao dịch (Transaction reference). Đây là mã số duy nhất dùng để xác định giao dịch.
        /// Lưu ý: Giá trị này bắt buộc và cần đảm bảo không bị trùng lặp giữa các giao dịch.
        /// </summary>
        public int PaymentId { get; set; }

        /// <summary>
        /// Thông tin mô tả nội dung thanh toán, không dấu và không bao gồm các ký tự đặt biệt.
        /// </summary>
        public string Description { get; set; } = default!;

        /// <summary>
        /// Số tiền thanh toán. Số tiền không mang các giá trị phân tách thập phân, phần nghìn, ký tự tiền tệ. Số tiền phải nằm trong khoảng 5.000 (VND) đến 1.000.000.000 (VND).
        /// </summary>
        public double Money { get; set; }

        /// <summary>
        /// Địa chỉ IP của thiết bị thực hiện giao dịch.
        /// </summary>
        public string IpAddress { get; set; } = default!;

        /// <summary>
        /// Mã phương thức thanh toán, mã loại ngân hàng hoặc ví điện tử thanh toán. Nếu mang giá trị <c>BankCode.ANY</c> thì chuyển hướng người dùng sang VNPay chọn phương thức thanh toán.
        /// </summary>
        public BankCode BankCode { get; set; } = BankCode.ANY;

        /// <summary>
        /// Thời điểm khởi tạo giao dịch. Giá trị mặc định là ngày và giờ hiện tại tại thời điểm yêu cầu được khởi tạo.
        /// </summary>
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        /// <summary>
        /// Đơn vị tiền tệ sử dụng thanh toán.
        /// </summary>
        public Currency Currency { get; set; } = Currency.VND;

        /// <summary>
        /// Ngôn ngữ hiển thị trên giao diện thanh toán của VNPay, mặc định là tiếng Việt.
        /// </summary>
        public DisplayLanguage Language { get; set; } = DisplayLanguage.Vietnamese;
    }
}
