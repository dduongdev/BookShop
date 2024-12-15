using AutoMapper;
using Infrastructure.SqlServer.Repositories.SqlServer.DataContext;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UseCases.Repositories;

namespace Infrastructure.SqlServer.Repositories
{
    public class SqlServerOrderRepository : IOrderRepository
    {
        private readonly BookShopDbContext _context;
        private readonly IMapper _mapper;

        public SqlServerOrderRepository(BookShopDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task AddAsync(Entities.Order order)
        {
            var staredOrder = _mapper.Map<Order>(order);
            await _context.Orders.AddAsync(staredOrder);
            await _context.SaveChangesAsync();
            order.Id = staredOrder.Id;
        }

        public async Task DeleteAsync(int id)
        {
            var foundOrder = await _context.Orders.FirstOrDefaultAsync(_ => _.Id == id);
            if (foundOrder != null)
            {
                _context.Orders.Remove(foundOrder);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Entities.Order>> GetAllAsync()
        {
            var storedOrders = await _context.Orders.ToListAsync();
            return _mapper.Map<IEnumerable<Entities.Order>>(storedOrders);
        }

        public async Task<Entities.Order?> GetByIdAsync(int id)
        {
            var foundOrder = await _context.Orders.FirstOrDefaultAsync(_ => _.Id == id);
            return _mapper.Map<Entities.Order?>(foundOrder);
        }

        public async Task UpdateAsync(Entities.Order order)
        {
            var existingOrder = await _context.Orders.FirstOrDefaultAsync(_ => _.Id == order.Id);
            if (existingOrder != null)
            {
                _mapper.Map(order, existingOrder);
                await _context.SaveChangesAsync();
            }
        }
    }
}
