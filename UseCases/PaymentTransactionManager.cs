using Entities;
using Entities.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UseCases.TaskResults;
using UseCases.UnitOfWork;

namespace UseCases
{
    public class PaymentTransactionManager
    {
        private readonly IPaymentTransactionUnitOfWork _paymentTransactionUnitOfWork;

        public PaymentTransactionManager(IPaymentTransactionUnitOfWork paymentTransactionUnitOfWork)
        {
            _paymentTransactionUnitOfWork = paymentTransactionUnitOfWork;
        }

        public async Task<AtomicTaskResult> AddAsync(PaymentTransaction paymentTransaction)
        {
            try
            {
                await _paymentTransactionUnitOfWork.BeginTransactionAsync();

                var order = await _paymentTransactionUnitOfWork.OrderRepository.GetByIdAsync(paymentTransaction.OrderId);

                if (order == null)
                {
                    return new AtomicTaskResult(AtomicTaskResultCodes.Failed, "Order not found.");
                }

                order.PaymentStatus = PaymentStatus.Paid;
                await _paymentTransactionUnitOfWork.OrderRepository.UpdateAsync(order);

                await _paymentTransactionUnitOfWork.PaymentTransactionRepository.AddAsync(paymentTransaction);

                await _paymentTransactionUnitOfWork.SaveChangesAsync();

                return AtomicTaskResult.Success;
            }
            catch (Exception ex)
            {
                await _paymentTransactionUnitOfWork.CancelTransactionAsync();
                return new AtomicTaskResult(AtomicTaskResultCodes.Failed, ex.Message);
            }
        }
    }
}
