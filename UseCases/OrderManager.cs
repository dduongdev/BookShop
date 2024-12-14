using Entities;
using Entities.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UseCases.Repositories;
using UseCases.TaskResults;
using UseCases.UnitOfWork;

namespace UseCases
{
    public class OrderManager
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IOrderUnitOfWork _orderUnitOfWork;

        public OrderManager(IOrderUnitOfWork orderUnitOfWork, IOrderRepository orderRepository)
        {
            _orderUnitOfWork = orderUnitOfWork;
            _orderRepository = orderRepository;
        }

        public Task<IEnumerable<Order>> GetAllAsync()
        {
            return _orderRepository.GetAllAsync();
        }

        public async Task<AtomicTaskResult> CreateAsync(Order order)
        {
            try
            {
                await _orderUnitOfWork.BeginTransactionAsync();

                await _orderUnitOfWork.OrderRepository.AddAsync(order);

                foreach (var orderItem in order.OrderItems)
                {
                    orderItem.OrderId = order.Id;

                    var orderedBook = await _orderUnitOfWork.BookRepository.GetByIdAsync(orderItem.BookId);

                    if (orderedBook == null)
                    {
                        return new AtomicTaskResult(AtomicTaskResultCodes.Failed, "An error occurred.");
                    }

                    orderedBook.Stock -= orderItem.Quantity;
                    if (orderedBook.Stock == 0)
                    {
                        orderedBook.Status = EntityStatus.Suspended;
                    }
                    await _orderUnitOfWork.BookRepository.UpdateAsync(orderedBook);

                    await _orderUnitOfWork.OrderItemRepository.AddAsync(orderItem);
                }

                await _orderUnitOfWork.SaveChangesAsync();

                return AtomicTaskResult.Success;

            }
            catch (Exception ex)
            {
                await _orderUnitOfWork.CancelTransactionAsync();
                return new AtomicTaskResult(AtomicTaskResultCodes.Failed, ex.Message);
            }
        }

        public async Task<AtomicTaskResult> CancelAsync(int id)
        {
            try
            {
                await _orderUnitOfWork.BeginTransactionAsync();

                var foundOrder = await _orderUnitOfWork.OrderRepository.GetByIdAsync(id);

                if (foundOrder == null)
                {
                    return new AtomicTaskResult(AtomicTaskResultCodes.Failed, "Order not found.");
                }

                foreach (var orderItem in foundOrder.OrderItems)
                {
                    var orderedBook = await _orderUnitOfWork.BookRepository.GetByIdAsync(orderItem.BookId);

                    if (orderedBook == null)
                    {
                        return new AtomicTaskResult(AtomicTaskResultCodes.Failed, "An error occurred.");
                    }

                    orderedBook.Stock += orderItem.Quantity;
                    await _orderUnitOfWork.BookRepository.UpdateAsync(orderedBook);
                    await _orderUnitOfWork.OrderItemRepository.DeleteAsync(orderItem.Id);
                }

                await _orderUnitOfWork.OrderRepository.DeleteAsync(foundOrder.Id);

                await _orderUnitOfWork.SaveChangesAsync();

                return AtomicTaskResult.Success;
            }
            catch (Exception ex)
            {
                await _orderUnitOfWork.CancelTransactionAsync();
                return new AtomicTaskResult(AtomicTaskResultCodes.Failed, ex.Message);
            }
        }

        public Task<Order?> GetByIdAsync(int id)
        {
            return _orderRepository.GetByIdAsync(id);
        }

        public async Task ChangeStatus(int id, OrderStatus newStatus)
        {
            var foundOrder = await _orderRepository.GetByIdAsync(id);
            if (foundOrder != null)
            {
                foundOrder.Status = newStatus;
                await _orderRepository.UpdateAsync(foundOrder);
            }
        }
    }
}
