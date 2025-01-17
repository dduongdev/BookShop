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
    public class CategoryManager
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly ICategoryUnitOfWork _categoryUnitOfWork;

        public CategoryManager(ICategoryRepository categoryRepository, ICategoryUnitOfWork categoryUnitOfWork)
        {
            _categoryRepository = categoryRepository;
            _categoryUnitOfWork = categoryUnitOfWork;
        }

        public Task<IEnumerable<Category>> GetAllAsync()
        {
            return _categoryRepository.GetAllAsync();
        }

        public async Task<IEnumerable<Category>> GetAllActiveAsync()
        {
            var storedCategories = await _categoryRepository.GetAllAsync();
            return storedCategories.Where(_ => _.Status == EntityStatus.Active);
        }

        public Task<Category?> GetByIdAsync(int id)
        {
            return _categoryRepository.GetByIdAsync(id);
        }

        public Task AddAsync(Category category)
        {
            return _categoryRepository.AddAsync(category);
        }

        public Task UpdateAsync(Category category)
        {
            return _categoryRepository.UpdateAsync(category);
        }

        public async Task<AtomicTaskResult> SuspendAsync(int id)
        {
            try
            {
                await _categoryUnitOfWork.BeginTransactionAsync();

                var foundCategory = await _categoryUnitOfWork.CategoryRepository.GetByIdAsync(id);
                if (foundCategory == null)
                {
                    return new AtomicTaskResult(AtomicTaskResultCodes.Failed, "Category not found.");
                }

                foundCategory.Status = EntityStatus.Suspended;

                var books = await _categoryUnitOfWork.BookRepository.GetAllAsync();
                foreach (var book in books)
                {
                    if (book.CategoryId == foundCategory.Id)
                    {
                        book.Status = EntityStatus.Suspended;
                        await _categoryUnitOfWork.BookRepository.UpdateAsync(book);
                    }
                }

                await _categoryUnitOfWork.CategoryRepository.UpdateAsync(foundCategory);
                await _categoryUnitOfWork.SaveChangesAsync();

                return AtomicTaskResult.Success;
            }
            catch (Exception ex)
            {
                await _categoryUnitOfWork.CancelTransactionAsync();
                return new AtomicTaskResult(AtomicTaskResultCodes.Failed, ex.Message);
            }
        }

        public async Task ActivateAsync(int id)
        {
            var foundCategory = await _categoryRepository.GetByIdAsync(id);

            if (foundCategory != null)
            {
                foundCategory.Status = EntityStatus.Active;
                await _categoryRepository.UpdateAsync(foundCategory);
            }
        }
    }
}
