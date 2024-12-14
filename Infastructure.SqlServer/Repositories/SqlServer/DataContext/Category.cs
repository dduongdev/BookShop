using Entities.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infastructure.SqlServer.Repositories.SqlServer.DataContext
{
    public class Category
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public EntityStatus Status { get; set; } = EntityStatus.Active;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public virtual ICollection<Book> Books { get; set; } = default!;
    }
}
