using Entities.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.SqlServer.Repositories.SqlServer.DataContext
{
    public class Publisher
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public EntityStatus Status { get; set; } = EntityStatus.Active;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public virtual ICollection<Book> Books { get; set; } = default!;
    }
}
