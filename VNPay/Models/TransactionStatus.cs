using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VNPay.Enums;

namespace VNPay.Models
{
    /// <summary>
    /// Trạng thái của giao dịch sau khi được xử lý.
    /// </summary>
    public class TransactionStatus
    {
        /// <summary>
        /// Mã trạng thái của giao dịch cho VNPay định nghĩa.
        /// </summary>
        public TransactionStatusCode Code { get; set; }

        /// <summary>
        /// Mô tả chi tiết về trạng thái giao dịch.
        /// </summary>
        public string Description { get; set; } = string.Empty;
    }
}
