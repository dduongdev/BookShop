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
    public class SqlServerCartItemRepository : ICartItemRepository
    {
        private readonly BookShopDbContext _context;
        private readonly IMapper _mapper;

        public SqlServerCartItemRepository(BookShopDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task AddAsync(Entities.CartItem cartItem)
        {
            var staredCartItem = _mapper.Map<CartItem>(cartItem);
            await _context.CartItems.AddAsync(staredCartItem);
            await _context.SaveChangesAsync();
            cartItem.Id = staredCartItem.Id;
        }

        public async Task DeleteAsync(int id)
        {
            var foundCartItem = await _context.CartItems.FirstOrDefaultAsync(_ => _.Id == id);
            if (foundCartItem != null)
            {
                _context.CartItems.Remove(foundCartItem);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Entities.CartItem>> GetAllAsync()
        {
            var storedCartItems = await _context.CartItems.ToListAsync();
            return _mapper.Map<IEnumerable<Entities.CartItem>>(storedCartItems);
        }

        public async Task<Entities.CartItem?> GetByIdAsync(int id)
        {
            var foundCartItem = await _context.CartItems.FirstOrDefaultAsync(_ => _.Id == id);
            return _mapper.Map<Entities.CartItem?>(foundCartItem);
        }

        public async Task UpdateAsync(Entities.CartItem cartItem)
        {
            var existingCartItem = await _context.CartItems.FirstOrDefaultAsync(_ => _.Id == cartItem.Id);
            if (existingCartItem != null)
            {
                _mapper.Map(cartItem, existingCartItem);
                await _context.SaveChangesAsync();
            }
        }
    }
}
