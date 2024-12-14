using AutoMapper;
using Infastructure.SqlServer.Repositories;
using Infastructure.SqlServer.Repositories.SqlServer.DataContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UseCases.Repositories;
using UseCases.UnitOfWork;

namespace Infastructure.SqlServer.UnitOfWork
{
    public class SqlServerCategoryUnitOfWork : ICategoryUnitOfWork
    {
        private readonly BookShopDbContext _context;

        public SqlServerCategoryUnitOfWork(BookShopDbContext context, IMapper mapper)
        {
            _context = context;

            CategoryRepository = new SqlServerCategoryRepository(context, mapper);
            BookRepository = new SqlServerBookRepository(context, mapper);
        }

        public ICategoryRepository CategoryRepository { get; }

        public IBookRepository BookRepository { get; }

        public Task BeginTransactionAsync()
        {
            return _context.Database.BeginTransactionAsync();
        }

        public Task CancelTransactionAsync()
        {
            return _context.Database.RollbackTransactionAsync();
        }

        public Task SaveChangesAsync()
        {
            return _context.Database.CommitTransactionAsync();
        }
    }
}
