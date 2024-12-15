using Entities;
using Entities.Enums;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UseCases;
using UseCases.Repositories;
using UseCases.TaskResults;
using UseCases.UnitOfWork;

namespace Test
{
    public class OrderManagerTests
    {
        private readonly Mock<IOrderUnitOfWork> _orderUnitOfWorkMock;
        private readonly Mock<IBookRepository> _bookRepositoryMock;
        private readonly Mock<IOrderRepository> _orderRepositoryMock;
        private readonly Mock<IOrderItemRepository> _orderItemRepositoryMock;
        private readonly OrderManager _orderManager;

        public OrderManagerTests()
        {
            _orderRepositoryMock = new Mock<IOrderRepository>();
            _orderUnitOfWorkMock = new Mock<IOrderUnitOfWork>();
            _bookRepositoryMock = new Mock<IBookRepository>();
            _orderItemRepositoryMock = new Mock<IOrderItemRepository>();

            _orderUnitOfWorkMock.Setup(uow => uow.OrderRepository).Returns(_orderRepositoryMock.Object);
            _orderUnitOfWorkMock.Setup(uow => uow.OrderItemRepository).Returns(_orderItemRepositoryMock.Object);
            _orderUnitOfWorkMock.Setup(uow => uow.BookRepository).Returns(_bookRepositoryMock.Object);

            _orderManager = new OrderManager(_orderUnitOfWorkMock.Object, _orderRepositoryMock.Object);
        }

        [Fact]
        public async Task CreateAsync_Success_ReturnsSuccess()
        {
            var order = new Order
            {
                DeliveryAddress = "",
                DeliveryPhone = "",
                OrderItems = new List<OrderItem>()
                {
                    new OrderItem
                    {
                        PriceAtPurchase = 400,
                        Quantity = 2,
                        BookId = 1
                    }
                }
            };

            var book = new Book
            {
                Id = 1,
                Price = 500,
                Stock = 10,
                Title = "C"
            };

            _orderRepositoryMock.Setup(repo => repo.AddAsync(order)).Returns(Task.CompletedTask);
            _bookRepositoryMock.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync(book);
            _bookRepositoryMock.Setup(repo => repo.UpdateAsync(It.IsAny<Book>())).Returns(Task.CompletedTask);
            _orderItemRepositoryMock.Setup(repo => repo.AddAsync(It.IsAny<OrderItem>())).Returns(Task.CompletedTask);
            _orderUnitOfWorkMock.Setup(uow => uow.SaveChangesAsync()).Returns(Task.CompletedTask);

            // Act
            var result = await _orderManager.CreateAsync(order);

            // Assert
            Assert.Equal(AtomicTaskResultCodes.Success, result.ResultCode);
            Assert.Equal(8, book.Stock);
            _orderRepositoryMock.Verify(repo => repo.AddAsync(order), Times.Once);
            _bookRepositoryMock.Verify(repo => repo.UpdateAsync(It.IsAny<Book>()), Times.Once);
            _orderItemRepositoryMock.Verify(repo => repo.AddAsync(It.IsAny<OrderItem>()), Times.Once);
        }

        [Fact]
        public async Task CreateAsync_Success_BookShouldBeSuspended()
        {
            var order = new Order
            {
                DeliveryAddress = "",
                DeliveryPhone = "",
                OrderItems = new List<OrderItem>()
                {
                    new OrderItem
                    {
                        PriceAtPurchase = 400,
                        Quantity = 2,
                        BookId = 1
                    }
                }
            };

            var book = new Book
            {
                Id = 1,
                Price = 500,
                Stock = 2,
                Title = "C",
                Status = Entities.Enums.EntityStatus.Active
            };

            _orderRepositoryMock.Setup(repo => repo.AddAsync(order)).Returns(Task.CompletedTask);
            _bookRepositoryMock.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync(book);
            _bookRepositoryMock.Setup(repo => repo.UpdateAsync(It.IsAny<Book>())).Returns(Task.CompletedTask);
            _orderItemRepositoryMock.Setup(repo => repo.AddAsync(It.IsAny<OrderItem>())).Returns(Task.CompletedTask);
            _orderUnitOfWorkMock.Setup(uow => uow.SaveChangesAsync()).Returns(Task.CompletedTask);

            // Act
            var result = await _orderManager.CreateAsync(order);

            // Assert
            Assert.Equal(AtomicTaskResultCodes.Success, result.ResultCode);
            Assert.Equal(0, book.Stock);
            Assert.Equal(Entities.Enums.EntityStatus.Suspended, book.Status);
            _orderRepositoryMock.Verify(repo => repo.AddAsync(order), Times.Once);
            _bookRepositoryMock.Verify(repo => repo.UpdateAsync(It.IsAny<Book>()), Times.Once);
            _orderItemRepositoryMock.Verify(repo => repo.AddAsync(It.IsAny<OrderItem>()), Times.Once);
        }

        [Fact]
        public async Task CreateAsync_BookNotFound_ReturnsFailed()
        {
            // Arrange
            var order = new Order
            {
                DeliveryAddress = "",
                DeliveryPhone = "",
                OrderItems = new List<OrderItem>()
                {
                    new OrderItem
                    {
                        PriceAtPurchase = 400,
                        Quantity = 2,
                        BookId = 1
                    }
                }
            };

            _orderRepositoryMock.Setup(repo => repo.AddAsync(order)).Returns(Task.CompletedTask);
            _bookRepositoryMock.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync((Book?)null);

            // Act
            var result = await _orderManager.CreateAsync(order);

            // Assert
            Assert.Equal(AtomicTaskResultCodes.Failed, result.ResultCode);
            Assert.Equal("An error occurred.", result.Message);
        }

        [Fact]
        public async Task CancelAsync_Success_ReturnsSuccess()
        {
            // Arrange
            var order = new Order
            {
                Id = 1,
                DeliveryAddress = "",
                DeliveryPhone = "",
                OrderItems = new List<OrderItem>
            {
                new OrderItem 
                { 
                    Id = 1, 
                    BookId = 1, 
                    Quantity = 2,
                    PriceAtPurchase = 400
                }
            }
            };

            var book = new Book {
                Id = 1,
                Price = 500,
                Stock = 2,
                Title = "C",
            };

            _orderRepositoryMock.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync(order);
            _bookRepositoryMock.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync(book);
            _bookRepositoryMock.Setup(repo => repo.UpdateAsync(It.IsAny<Book>())).Returns(Task.CompletedTask);
            _orderItemRepositoryMock.Setup(repo => repo.DeleteAsync(1)).Returns(Task.CompletedTask);
            _orderRepositoryMock.Setup(repo => repo.DeleteAsync(1)).Returns(Task.CompletedTask);
            _orderUnitOfWorkMock.Setup(uow => uow.SaveChangesAsync()).Returns(Task.CompletedTask);

            // Act
            var result = await _orderManager.CancelAsync(1);

            // Assert
            Assert.Equal(AtomicTaskResultCodes.Success, result.ResultCode);
            Assert.Equal(4, book.Stock);
            _bookRepositoryMock.Verify(repo => repo.UpdateAsync(It.IsAny<Book>()), Times.Once);
            _orderItemRepositoryMock.Verify(repo => repo.DeleteAsync(1), Times.Once);
            _orderRepositoryMock.Verify(repo => repo.DeleteAsync(1), Times.Once);
        }

        [Fact]
        public async Task CancelAsync_OrderNotFound_ReturnsFailed()
        {
            // Arrange
            _orderRepositoryMock.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync((Order?)null);

            // Act
            var result = await _orderManager.CancelAsync(1);

            // Assert
            Assert.Equal(AtomicTaskResultCodes.Failed, result.ResultCode);
            Assert.Equal("Order not found.", result.Message);
        }
    }
}
