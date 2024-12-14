using Entities.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infastructure.SqlServer.Repositories.SqlServer.DataContext
{
    public class PaymentTransaction
    {
        public int Id { get; set; }
        public long TransactionId { get; set; }
        public PaymentGateway Gateway { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }
        public DateTime CreatedAt { get; set; }
        public int OrderId { get; set; }

        public virtual Order Order { get; set; } = default!;
    }
}
