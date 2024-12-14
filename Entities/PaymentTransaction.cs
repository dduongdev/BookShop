using Entities.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class PaymentTransaction
    {
        public int Id { get; set; }
        public long TransactionId { get; set; }
        public PaymentGateway Gateway { get; set; }
        public decimal Amount { get; set; }
        public DateTime CreatedAt { get; set; }
        public int OrderId { get; set; }
    }
}
