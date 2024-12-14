using AutoMapper;
using Infastructure.SqlServer.Repositories.SqlServer.DataContext;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UseCases.Repositories;

namespace Infastructure.SqlServer.Repositories
{
    public class SqlServerBookRepository : IBookRepository
    {
        private readonly BookShopDbContext _context;
        private readonly IMapper _mapper;

        public SqlServerBookRepository(BookShopDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task AddAsync(Entities.Book book)
        {
            var staredBook = _mapper.Map<Book>(book);
            await _context.Books.AddAsync(staredBook);
            await _context.SaveChangesAsync();
            book.Id = staredBook.Id;
        }

        public async Task DeleteAsync(int id)
        {
            var foundBook = await _context.Books.FirstOrDefaultAsync(_ => _.Id == id);
            if (foundBook != null)
            {
                _context.Books.Remove(foundBook);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Entities.Book>> GetAllAsync()
        {
            var storedBooks = await _context.Books.ToListAsync();
            return _mapper.Map<IEnumerable<Entities.Book>>(storedBooks);
        }

        public async Task<Entities.Book?> GetByIdAsync(int id)
        {
            var foundBook = await _context.Books.FirstOrDefaultAsync(_ => _.Id == id);
            return _mapper.Map<Entities.Book?>(foundBook);
        }

        public async Task UpdateAsync(Entities.Book book)
        {
            var existingBook = await _context.Books.FirstOrDefaultAsync(_ => _.Id == book.Id);
            if (existingBook != null)
            {
                _mapper.Map(book, existingBook);
                await _context.SaveChangesAsync();
            }
        }
    }
}
