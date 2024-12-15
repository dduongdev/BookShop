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
    public class SqlServerCategoryRepository : ICategoryRepository
    {
        private readonly BookShopDbContext _context;
        private readonly IMapper _mapper;

        public SqlServerCategoryRepository(BookShopDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task AddAsync(Entities.Category category)
        {
            var staredCategory = _mapper.Map<Category>(category);
            await _context.Categories.AddAsync(staredCategory);
            await _context.SaveChangesAsync();
            category.Id = staredCategory.Id;
        }

        public async Task DeleteAsync(int id)
        {
            var foundCategory = await _context.Categories.FirstOrDefaultAsync(_ => _.Id == id);
            if (foundCategory != null)
            {
                _context.Categories.Remove(foundCategory);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Entities.Category>> GetAllAsync()
        {
            var storedCategories = await _context.Categories.ToListAsync();
            return _mapper.Map<IEnumerable<Entities.Category>>(storedCategories);
        }

        public async Task<Entities.Category?> GetByIdAsync(int id)
        {
            var foundCategory = await _context.Categories.FirstOrDefaultAsync(_ => _.Id == id);
            return _mapper.Map<Entities.Category?>(foundCategory);
        }

        public async Task UpdateAsync(Entities.Category category)
        {
            var existingCategory = await _context.Categories.FirstOrDefaultAsync(_ => _.Id == category.Id);
            if (existingCategory != null)
            {
                _mapper.Map(category, existingCategory);
                await _context.SaveChangesAsync();
            }
        }
    }
}
