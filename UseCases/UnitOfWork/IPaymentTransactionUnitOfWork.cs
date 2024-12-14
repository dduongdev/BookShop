using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UseCases.Repositories;

namespace UseCases.UnitOfWork
{
    public interface IPaymentTransactionUnitOfWork : IUnitOfWork
    {
        IPaymentTransactionRepository PaymentTransactionRepository { get; }
        IOrderRepository OrderRepository { get; }
    }
}
