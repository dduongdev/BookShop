using Infrastructure.Models;

namespace Infrastructure.Services
{
    public class RequestScopedPaginationService<TEntity>
    {
        public int PageSize { get; private set; }
        public int TotalPages { get; private set; }
        public int TotalItemCount { get; private set; }
        public int CurrentPageIndex { get; private set; }
        public IEnumerable<TEntity> Source { get; private set; }
        public bool HasPrevious => CurrentPageIndex > 1;
        public bool HasNext => CurrentPageIndex < TotalPages;

        public RequestScopedPaginationService(IEnumerable<TEntity> source, int pageSize)
        {
            Source = source ?? throw new ArgumentNullException(nameof(source), "Source collection cannot be null.");
            TotalItemCount = source.Count();

            if (pageSize <= 0)
            {
                throw new ArgumentException("PageSize must be greater than 0.");
            }

            PageSize = pageSize;

            TotalPages = Math.Max((int)Math.Ceiling(TotalItemCount / (double)PageSize), 1);
        }

        public IEnumerable<TEntity> GetItemsByPage(int pageIndex)
        {
            if (pageIndex <= 0 || pageIndex > TotalPages)
            {
                throw new ArgumentOutOfRangeException(nameof(pageIndex), $"Page index must be between 1 and {TotalPages}.");
            }

            CurrentPageIndex = pageIndex;

            return Source.Skip((pageIndex - 1) * PageSize)
                .Take(PageSize).ToList();
        }

        public PaginationMetadata GetPaginationMetadata()
        {
            return new PaginationMetadata
            {
                PageSize = this.PageSize,
                CurrentPageIndex = this.CurrentPageIndex,
                TotalPages = this.TotalPages,
                HasPrevious = this.HasPrevious,
                HasNext = this.HasNext
            };
        }
    }
}
