using Entities;
using Entities.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UseCases.Repositories;
using UseCases.TaskResults;

namespace UseCases
{
    public class BookManager
    {
        private readonly IBookRepository _bookRepository;

        public BookManager(IBookRepository bookRepository)
        {
            _bookRepository = bookRepository;
        }

        public Task<IEnumerable<Book>> GetAllAsync()
        {
            return _bookRepository.GetAllAsync();
        }

        public async Task<IEnumerable<Book>> GetAllActivateAsync()
        {
            var storedBooks = await _bookRepository.GetAllAsync();
            return storedBooks.Where(_ => _.Status == EntityStatus.Active);
        }

        public Task<Book?> GetByIdAsync(int id)
        {
            return _bookRepository.GetByIdAsync(id);
        }

        public Task AddAsync(Book book)
        {
            return _bookRepository.AddAsync(book);
        }

        public Task UpdateAsync(Book book)
        {
            return _bookRepository.UpdateAsync(book);
        }

        public async Task SuspendAsync(int id)
        {
            var foundBook = await _bookRepository.GetByIdAsync(id);
            if (foundBook != null)
            {
                foundBook.Status = EntityStatus.Suspended;
                await _bookRepository.UpdateAsync(foundBook);
            }
        }

        public async Task ActivateAsync(int id)
        {
            var foundBook = await _bookRepository.GetByIdAsync(id);
            if (foundBook != null)
            {
                foundBook.Status = EntityStatus.Active;
                await _bookRepository.UpdateAsync(foundBook);
            }
        }
    }
}
