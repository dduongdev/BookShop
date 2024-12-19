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
    public class SqlServerOrderItemRepository : IOrderItemRepository
    {
        private readonly BookShopDbContext _context;
        private readonly IMapper _mapper;

        public SqlServerOrderItemRepository(BookShopDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task AddAsync(Entities.OrderItem orderItem)
        {
            var staredOrderItem = _mapper.Map<OrderItem>(orderItem);
            await _context.OrderItems.AddAsync(staredOrderItem);
            await _context.SaveChangesAsync();
            orderItem.Id = staredOrderItem.Id;
        }

        public async Task DeleteAsync(int id)
        {
            var foundOrderItem = await _context.OrderItems.FirstOrDefaultAsync(_ => _.Id == id);
            if (foundOrderItem != null)
            {
                _context.OrderItems.Remove(foundOrderItem);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Entities.OrderItem>> GetAllAsync()
        {
            var storedOrderItems = await _context.OrderItems.ToListAsync();
            return _mapper.Map<IEnumerable<Entities.OrderItem>>(storedOrderItems);
        }

        public async Task<Entities.OrderItem?> GetByIdAsync(int id)
        {
            var foundOrderItem = await _context.OrderItems.FirstOrDefaultAsync(_ => _.Id == id);
            return _mapper.Map<Entities.OrderItem?>(foundOrderItem);
        }

        public async Task UpdateAsync(Entities.OrderItem orderItem)
        {
            var existingOrderItem = await _context.OrderItems.FirstOrDefaultAsync(_ => _.Id == orderItem.Id);
            if (existingOrderItem != null)
            {
                _mapper.Map(orderItem, existingOrderItem);
                await _context.SaveChangesAsync();
            }
        }
    }
}
