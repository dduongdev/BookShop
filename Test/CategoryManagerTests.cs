using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using UseCases;
using Entities;
using Entities.Enums;
using UseCases.Repositories;
using UseCases.UnitOfWork;
using UseCases.TaskResults;

public class CategoryManagerTests
{
    private readonly Mock<ICategoryRepository> _categoryRepositoryMock;
    private readonly Mock<ICategoryUnitOfWork> _categoryUnitOfWorkMock;
    private readonly CategoryManager _categoryManager;

    public CategoryManagerTests()
    {
        _categoryRepositoryMock = new Mock<ICategoryRepository>();
        _categoryUnitOfWorkMock = new Mock<ICategoryUnitOfWork>();

        _categoryManager = new CategoryManager(_categoryRepositoryMock.Object, _categoryUnitOfWorkMock.Object);
    }

    [Fact]
    public async Task GetAllActiveAsync_ShouldReturnOnlyActiveCategories()
    {
        // Arrange
        var categories = new List<Category>
        {
            new Category { Id = 1, Name = "Category 1", Status = EntityStatus.Active },
            new Category { Id = 2, Name = "Category 2", Status = EntityStatus.Suspended }
        };
        _categoryRepositoryMock.Setup(repo => repo.GetAllAsync()).ReturnsAsync(categories);

        // Act
        var result = await _categoryManager.GetAllActiveAsync();

        // Assert
        Assert.Single(result); // Chỉ có 1 category Active
        Assert.Contains(result, c => c.Status == EntityStatus.Active);
    }

    [Fact]
    public async Task SuspendAsync_ShouldSuspendCategoryAndItsBooks()
    {
        // Arrange
        var category = new Category { Id = 1, Name = "Category 1", Status = EntityStatus.Active };
        var books = new List<Book>
    {
        new Book
        {
            Id = 1,
            Title = "Book 1",
            Price = 100m,
            Stock = 10,
            CategoryId = 1,
            Status = EntityStatus.Active,
            Author = "Author 1",
            Description = "Description for Book 1",
            DiscountPercentage = 10m,
            PublisherId = 1,
            ImagesDirectory = "/images/book1.jpg",
            CreatedAt = DateTime.UtcNow
        },
        new Book
        {
            Id = 2,
            Title = "Book 2",
            Price = 150m,
            Stock = 5,
            CategoryId = 1,
            Status = EntityStatus.Active,
            Author = "Author 2",
            Description = "Description for Book 2",
            DiscountPercentage = 5m,
            PublisherId = 1,
            ImagesDirectory = "/images/book2.jpg",
            CreatedAt = DateTime.UtcNow
        }
    };

        _categoryUnitOfWorkMock.Setup(uow => uow.BeginTransactionAsync()).Returns(Task.CompletedTask);
        _categoryUnitOfWorkMock.Setup(uow => uow.CategoryRepository.GetByIdAsync(1)).ReturnsAsync(category);
        _categoryUnitOfWorkMock.Setup(uow => uow.BookRepository.GetAllAsync()).ReturnsAsync(books);
        _categoryUnitOfWorkMock.Setup(uow => uow.SaveChangesAsync()).Returns(Task.CompletedTask);

        // Act
        var result = await _categoryManager.SuspendAsync(1);

        // Assert
        Assert.Equal(AtomicTaskResultCodes.Success, result.ResultCode);

        // Kiểm tra tất cả các sách trong danh mục đều bị chuyển trạng thái sang Suspended
        Assert.All(books, b => Assert.Equal(EntityStatus.Suspended, b.Status));

        // Kiểm tra xem `SaveChangesAsync` được gọi
        _categoryUnitOfWorkMock.Verify(uow => uow.SaveChangesAsync(), Times.Once);
    }


    [Fact]
    public async Task SuspendAsync_ShouldReturnFailedResult_WhenCategoryNotFound()
    {
        // Arrange
        _categoryUnitOfWorkMock.Setup(uow => uow.CategoryRepository.GetByIdAsync(1)).ReturnsAsync((Category)null);

        // Act
        var result = await _categoryManager.SuspendAsync(1);

        // Assert
        Assert.Equal(AtomicTaskResultCodes.Failed, result.ResultCode);
        Assert.Equal("Category not found.", result.Message);
    }

    [Fact]
    public async Task ActivateAsync_ShouldActivateCategory()
    {
        // Arrange
        var category = new Category { Id = 1, Name = "Category 1", Status = EntityStatus.Suspended };
        _categoryRepositoryMock.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync(category);
        _categoryRepositoryMock.Setup(repo => repo.UpdateAsync(It.IsAny<Category>())).Returns(Task.CompletedTask);

        // Act
        await _categoryManager.ActivateAsync(1);

        // Assert
        Assert.Equal(EntityStatus.Active, category.Status);
        _categoryRepositoryMock.Verify(repo => repo.UpdateAsync(It.Is<Category>(c => c.Status == EntityStatus.Active)), Times.Once);
    }
}
