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
    public class SqlServerPaymentTransactionUnitOfWork : IPaymentTransactionUnitOfWork
    {
        private readonly BookShopDbContext _context;

        public SqlServerPaymentTransactionUnitOfWork(BookShopDbContext context, IMapper mapper)
        {
            _context = context;

            PaymentTransactionRepository = new SqlServerPaymentTransactionRepository(context, mapper);
            OrderRepository = new SqlServerOrderRepository(context, mapper);
        }

        public IPaymentTransactionRepository PaymentTransactionRepository { get; }

        public IOrderRepository OrderRepository { get; }

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
