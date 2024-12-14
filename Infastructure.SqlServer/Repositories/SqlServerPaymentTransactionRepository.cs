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
    public class SqlServerPaymentTransactionRepository : IPaymentTransactionRepository
    {
        private readonly BookShopDbContext _context;
        private readonly IMapper _mapper;

        public SqlServerPaymentTransactionRepository(BookShopDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task AddAsync(Entities.PaymentTransaction transaction)
        {
            var staredTransaction = _mapper.Map<PaymentTransaction>(transaction);
            await _context.PaymentTransactions.AddAsync(staredTransaction);
            await _context.SaveChangesAsync();
            transaction.Id = staredTransaction.Id;
        }

        public Task DeleteAsync(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<Entities.PaymentTransaction>> GetAllAsync()
        {
            var storedPaymentTransactions = await _context.PaymentTransactions.ToListAsync();
            return _mapper.Map<IEnumerable<Entities.PaymentTransaction>>(storedPaymentTransactions);
        }

        public async Task<Entities.PaymentTransaction?> GetByIdAsync(int id)
        {
            var foundPaymentTransaction = await _context.PaymentTransactions.FirstOrDefaultAsync(_ => _.Id == id);
            return _mapper.Map<Entities.PaymentTransaction>(foundPaymentTransaction);
        }

        public Task UpdateAsync(Entities.PaymentTransaction entity)
        {
            throw new NotImplementedException();
        }
    }
}
