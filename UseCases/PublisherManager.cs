using Entities;
using Entities.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UseCases.Repositories;
using UseCases.TaskResults;
using UseCases.UnitOfWork;

namespace UseCases
{
    public class PublisherManager
    {
        private readonly IPublisherRepository _publisherRepository;
        private readonly IPublisherUnitOfWork _publisherUnitOfWork;

        public PublisherManager(IPublisherUnitOfWork publisherUnitOfWork, IPublisherRepository publisherRepository)
        {
            _publisherUnitOfWork = publisherUnitOfWork;
            _publisherRepository = publisherRepository;
        }

        public Task<IEnumerable<Publisher>> GetAllAsync()
        {
            return _publisherRepository.GetAllAsync();
        }

        public async Task<IEnumerable<Publisher>> GetAllActiveAsync()
        {
            var storedPublishers = await _publisherRepository.GetAllAsync();
            return storedPublishers.Where(_ => _.Status == EntityStatus.Active);
        }

        public Task<Publisher?> GetByIdAsync(int id)
        {
            return _publisherRepository.GetByIdAsync(id);
        }

        public Task AddAsync(Publisher publisher)
        {
            return _publisherRepository.AddAsync(publisher);
        }

        public Task UpdateAsync(Publisher publisher)
        {
            return _publisherRepository.UpdateAsync(publisher);
        }

        public async Task<AtomicTaskResult> SuspendAsync(int id)
        {
            try
            {
                await _publisherUnitOfWork.BeginTransactionAsync();

                var foundPublisher = await _publisherUnitOfWork.PublisherRepository.GetByIdAsync(id);

                if (foundPublisher == null)
                {
                    return new AtomicTaskResult(AtomicTaskResultCodes.Failed, "Publisher not found.");
                }

                var storedBooks = await _publisherUnitOfWork.BookRepository.GetAllAsync();
                foreach (var book in storedBooks)
                {
                    if (book.PublisherId == foundPublisher.Id)
                    {
                        book.Status = EntityStatus.Suspended;
                        await _publisherUnitOfWork.BookRepository.UpdateAsync(book);
                    }
                }

                foundPublisher.Status = EntityStatus.Suspended;
                await _publisherUnitOfWork.PublisherRepository.UpdateAsync(foundPublisher);

                await _publisherUnitOfWork.SaveChangesAsync();

                return AtomicTaskResult.Success;

            }
            catch (Exception ex)
            {
                await _publisherUnitOfWork.CancelTransactionAsync();
                return new AtomicTaskResult(AtomicTaskResultCodes.Failed, ex.Message);
            }
        }

        public async Task ActivateAsync(int id)
        {
            var foundPublisher = await _publisherRepository.GetByIdAsync(id);
            if (foundPublisher != null)
            {
                foundPublisher.Status = EntityStatus.Active;
                await _publisherRepository.UpdateAsync(foundPublisher);
            }
        }
    }
}
