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
    public class SqlServerPublisherRepository : IPublisherRepository
    {
        private readonly BookShopDbContext _context;
        private readonly IMapper _mapper;

        public SqlServerPublisherRepository(BookShopDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task AddAsync(Entities.Publisher publisher)
        {
            var staredPublisher = _mapper.Map<Publisher>(publisher);
            await _context.Publishers.AddAsync(staredPublisher);
            await _context.SaveChangesAsync();
            publisher.Id = staredPublisher.Id;
        }

        public async Task DeleteAsync(int id)
        {
            var foundPublisher = await _context.Publishers.FirstOrDefaultAsync(_ => _.Id == id);
            if (foundPublisher != null)
            {
                _context.Publishers.Remove(foundPublisher);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Entities.Publisher>> GetAllAsync()
        {
            var storedPublishers = await _context.Publishers.ToListAsync();
            return _mapper.Map<IEnumerable<Entities.Publisher>>(storedPublishers);
        }

        public async Task<Entities.Publisher?> GetByIdAsync(int id)
        {
            var foundPublisher = await _context.Publishers.FirstOrDefaultAsync(_ => _.Id == id);
            return _mapper.Map<Entities.Publisher?>(foundPublisher);
        }

        public async Task UpdateAsync(Entities.Publisher publisher)
        {
            var existingPublisher = await _context.Publishers.FirstOrDefaultAsync(_ => _.Id == publisher.Id);
            if (existingPublisher != null)
            {
                _mapper.Map(publisher, existingPublisher);
                await _context.SaveChangesAsync();
            }
        }
    }
}
