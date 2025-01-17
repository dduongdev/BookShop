using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.SqlServer.Repositories.SqlServer.DataContext
{
    public class CartItem
    {
        public int Id { get; set; }
        public int BookId { get; set; }
        public int UserId { get; set; }

        public virtual Book Book { get; set; } = default!;
        public virtual User User { get; set; } = default!;
    }
}
