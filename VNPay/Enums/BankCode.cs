using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VNPay.Enums
{
    public enum BankCode : sbyte
    {
        /// <summary>
        /// Tất cả các phương thức thanh toán.
        /// Lựa chọn này cho phép khách hàng thanh toán bằng bất kỳ phương thức này mà VNPay hỗ trợ.
        /// </summary>
        [Description("Tất cả phương thức thanh toán.")]
        ANY,

        /// <summary>
        /// Thanh toán bằng cách quét mã QR qua ứng dụng hỗ trợ VNPay-QR.
        /// Phương thức này yêu cầu khách hàng sử dụng ứng dụng ngân hàng hoặc ví điện tử có tách hợp VNPay-QR.
        /// </summary>
        [Description("Thanh toán quét mã QR")]
        VNPAYQR,

        /// <summary>
        /// Thanh toán bằng thẻ ATM nội địa hoặc tài khoản ngân hàng tại Việt Nam.
        /// Áp dụng cho khách hàng sử dụng thẻ ATM hoặc liên kết tài khoản ngân hàng nội địa.
        /// </summary>
        [Description("Thẻ ATM hoặc tài khoản ngân hàng tại Việt Nam")]
        VNBANK,

        /// <summary>
        /// Thanh toán bằng thẻ thanh toán quốc tế.
        /// Hỗ trợ các loại thẻ quốc tế như Visa, MasterCard, JCB và các loại thẻ tương tự.
        /// </summary>
        [Description("Thẻ thanh toán quốc tế.")]
        INTCARD
    }
}
