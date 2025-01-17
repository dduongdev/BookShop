using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using UseCases.TaskResults;
using UseCases;
using UseCases.UnitOfWork;
using Microsoft.AspNetCore.Authorization;

namespace Infrastructure.Areas.Admin.Pages.Publishers
{
    [Authorize(Roles = "Admin")]
    public class IndexModel : PageModel
    {
        private readonly PublisherManager _publisherManager;
        private readonly IPublisherUnitOfWork _publisherUnitOfWork;
        public IndexModel(PublisherManager publisherManager, IPublisherUnitOfWork publisherUnitOfWork)
        {
            _publisherManager = publisherManager;
            _publisherUnitOfWork = publisherUnitOfWork;
        }

        public IEnumerable<Entities.Publisher> Publishers { get; set; }

        public async Task OnGetAsync()
        {
            Publishers = await _publisherManager.GetAllAsync();
        }

        public async Task<IActionResult> OnPostSuspendAsync(int id)
        {
            var result = await _publisherManager.SuspendAsync(id);

            if (result.ResultCode == AtomicTaskResultCodes.Success)
            {
                Publishers = await _publisherManager.GetAllAsync();
                return RedirectToPage("Index");
            }

            TempData["ErrorMessage"] = result.Message;
            return Page();
        }
        public async Task<IActionResult> OnPostActivateAsync(int id)
        {
            await _publisherManager.ActivateAsync(id);
            return RedirectToPage("Index");
        }

    }
}
