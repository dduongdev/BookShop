using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VNPay.Models
{
    /// <summary>
    /// Thông tin về ngân hàng liên quan đến giao dịch.
    /// </summary>
    public class BankingInfor
    {
        /// <summary>
        /// Mã ngân hàng thực hiện giao dịch. VD: VCB (Vietcombank)
        /// </summary>
        public string BankCode { get; set; } = default!;

        /// <summary>
        /// Mã giao dịch ở phía ngân hàng, được dùng để theo dõi và đối soát giao dịch với ngân hàng.
        /// </summary>
        public string BankTransactionId { get; set; } = default!;
    }
}
