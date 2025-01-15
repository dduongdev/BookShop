using Entities;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Threading.Tasks;
using UseCases;

namespace Areas.Admin.Pages.Publishers
{
    public class IndexModel : PageModel
    {
        private readonly PublisherManager _publisherManager;

        public IndexModel(PublisherManager publisherManager)
        {
            _publisherManager = publisherManager;
        }

        public IEnumerable<Publisher> Publishers { get; set; }

        public async Task OnGetAsync()
        {
            Publishers = await _publisherManager.GetAllAsync();
        }
    }
}
