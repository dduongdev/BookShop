using Entities.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.SqlServer.Repositories.SqlServer.DataContext
{
    public class PaymentTransaction
    {
        public int Id { get; set; }
        public string BankCode { get; set; } = default!;
        public long TransactionId { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }
        public DateTime CreatedAt { get; set; }
        public int OrderId { get; set; }

        public virtual Order Order { get; set; } = default!;
    }
}
