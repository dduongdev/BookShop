using Entities;
using Entities.Enums;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UseCases;
using UseCases.Repositories;
using UseCases.TaskResults;
using UseCases.UnitOfWork;
using Xunit;

namespace Test
{
    public class PublisherManagerTests
    {
        private readonly Mock<IPublisherRepository> _publisherRepositoryMock;
        private readonly Mock<IBookRepository> _bookRepositoryMock;
        private readonly Mock<IPublisherUnitOfWork> _publisherUnitOfWorkMock;
        private readonly PublisherManager _publisherManager;

        public PublisherManagerTests()
        {
            _publisherRepositoryMock = new Mock<IPublisherRepository>();
            _bookRepositoryMock = new Mock<IBookRepository>();
            _publisherUnitOfWorkMock = new Mock<IPublisherUnitOfWork>();

            _publisherUnitOfWorkMock.Setup(uow => uow.PublisherRepository).Returns(_publisherRepositoryMock.Object);
            _publisherUnitOfWorkMock.Setup(uow => uow.BookRepository).Returns(_bookRepositoryMock.Object);

            _publisherManager = new PublisherManager(_publisherUnitOfWorkMock.Object, _publisherRepositoryMock.Object);
        }

        [Fact]
        public async Task GetAllActiveAsync_ReturnsOnlyActivePublishers()
        {
            // Arrange
            var publishers = new List<Publisher>
            {
                new Publisher { Id = 1, Name = "Publisher 1", Status = EntityStatus.Active },
                new Publisher { Id = 2, Name = "Publisher 2", Status = EntityStatus.Suspended },
                new Publisher { Id = 3, Name = "Publisher 3", Status = EntityStatus.Active }
            };

            _publisherRepositoryMock.Setup(repo => repo.GetAllAsync()).ReturnsAsync(publishers);

            // Act
            var result = await _publisherManager.GetAllActiveAsync();

            // Assert
            Assert.Equal(2, result.Count());
            Assert.All(result, publisher => Assert.Equal(EntityStatus.Active, publisher.Status));
        }

        [Fact]
        public async Task SuspendAsync_SuccessfullySuspendsPublisherAndBooks()
        {
            // Arrange
            var publisher = new Publisher { Id = 1, Name = "Publisher 1", Status = EntityStatus.Active };
            var books = new List<Book>
            {
                new Book { Id = 1, Title = "Book 1", Price = 10.00m, Stock = 5, PublisherId = 1, Status = EntityStatus.Active },
                new Book { Id = 2, Title = "Book 2", Price = 15.00m, Stock = 3, PublisherId = 1, Status = EntityStatus.Active }
            };

            _publisherUnitOfWorkMock.Setup(uow => uow.PublisherRepository.GetByIdAsync(1)).ReturnsAsync(publisher);
            _publisherUnitOfWorkMock.Setup(uow => uow.BookRepository.GetAllAsync()).ReturnsAsync(books);
            _publisherUnitOfWorkMock.Setup(uow => uow.SaveChangesAsync()).Returns(Task.CompletedTask);

            // Act
            var result = await _publisherManager.SuspendAsync(1);

            // Assert
            Assert.Equal(AtomicTaskResultCodes.Success, result.ResultCode);
            Assert.Equal(EntityStatus.Suspended, publisher.Status);
            Assert.All(books, book => Assert.Equal(EntityStatus.Suspended, book.Status));
            _publisherUnitOfWorkMock.Verify(uow => uow.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task SuspendAsync_PublisherNotFound_ReturnsFailed()
        {
            // Arrange
            _publisherUnitOfWorkMock.Setup(uow => uow.PublisherRepository.GetByIdAsync(1)).ReturnsAsync((Publisher?)null);

            // Act
            var result = await _publisherManager.SuspendAsync(1);

            // Assert
            Assert.Equal(AtomicTaskResultCodes.Failed, result.ResultCode);
            Assert.Equal("Publisher not found.", result.Message);
        }

        [Fact]
        public async Task ActivateAsync_ActivatesPublisher()
        {
            // Arrange
            var publisher = new Publisher { Id = 1, Name = "Publisher 1", Status = EntityStatus.Suspended };

            _publisherRepositoryMock.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync(publisher);

            // Act
            await _publisherManager.ActivateAsync(1);

            // Assert
            Assert.Equal(EntityStatus.Active, publisher.Status);
            _publisherRepositoryMock.Verify(repo => repo.UpdateAsync(It.IsAny<Publisher>()), Times.Once);
        }
    }
}
