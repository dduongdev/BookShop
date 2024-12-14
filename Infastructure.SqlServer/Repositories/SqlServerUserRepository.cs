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
    public class SqlServerUserRepository : IUserRepository
    {
        private readonly BookShopDbContext _context;
        private readonly IMapper _mapper;

        public SqlServerUserRepository(BookShopDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task AddAsync(Entities.User user)
        {
            var staredUser = _mapper.Map<User>(user);
            await _context.Users.AddAsync(staredUser);
            await _context.SaveChangesAsync();
            user.Id = staredUser.Id;
        }

        public async Task DeleteAsync(int id)
        {
            var foundUser = await _context.Users.FirstOrDefaultAsync(_ => _.Id == id);
            if (foundUser != null)
            {
                _context.Users.Remove(foundUser);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Entities.User>> GetAllAsync()
        {
            var storedUsers = await _context.Users.ToListAsync();
            return _mapper.Map<IEnumerable<Entities.User>>(storedUsers);
        }

        public async Task<Entities.User?> GetByIdAsync(int id)
        {
            var foundUser = await _context.Users.FirstOrDefaultAsync(_ => _.Id == id);
            return _mapper.Map<Entities.User?>(foundUser);
        }

        public async Task UpdateAsync(Entities.User user)
        {
            var existingUser = await _context.Users.FirstOrDefaultAsync(_ => _.Id == user.Id);
            if (existingUser != null)
            {
                _mapper.Map(user, existingUser);
                await _context.SaveChangesAsync();
            }
        }
    }
}
