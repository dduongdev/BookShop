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
    public class SqlServerFeedbackRepository : IFeedbackRepository
    {
        private readonly BookShopDbContext _context;
        private readonly IMapper _mapper;

        public SqlServerFeedbackRepository(BookShopDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task AddAsync(Entities.Feedback feedback)
        {
            var staredFeedback = _mapper.Map<Feedback>(feedback);
            await _context.Feedbacks.AddAsync(staredFeedback);
            await _context.SaveChangesAsync();
            feedback.Id = staredFeedback.Id;
        }

        public async Task DeleteAsync(int id)
        {
            var foundFeedback = await _context.Feedbacks.FirstOrDefaultAsync(_ => _.Id == id);
            if (foundFeedback != null)
            {
                _context.Feedbacks.Remove(foundFeedback);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Entities.Feedback>> GetAllAsync()
        {
            var storedFeedbacks = await _context.Feedbacks.ToListAsync();
            return _mapper.Map<IEnumerable<Entities.Feedback>>(storedFeedbacks);
        }

        public async Task<Entities.Feedback?> GetByIdAsync(int id)
        {
            var foundFeedback = await _context.Feedbacks.FirstOrDefaultAsync(_ => _.Id == id);
            return _mapper.Map<Entities.Feedback?>(foundFeedback);
        }

        public async Task UpdateAsync(Entities.Feedback feedback)
        {
            var existingFeedback = await _context.Feedbacks.FirstOrDefaultAsync(_ => _.Id == feedback.Id);
            if (existingFeedback != null)
            {
                _mapper.Map(feedback, existingFeedback);
                await _context.SaveChangesAsync();
            }
        }
    }
}
