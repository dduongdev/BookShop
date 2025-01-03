﻿using Entities.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.SqlServer.Repositories.SqlServer.DataContext
{
    public class Book
    {
        public int Id { get; set; }
        public required string Title { get; set; }
        public string? Author { get; set; }
        public string? Description { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public required decimal Price { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal DiscountPercentage { get; set; } = 0;
        public required int Stock { get; set; }
        public string? ImagesDirectory { get; set; }
        public int PublisherId { get; set; }
        public int CategoryId { get; set; }
        public EntityStatus Status { get; set; } = EntityStatus.Active;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public virtual Category Category { get; set; } = default!;
        public virtual ICollection<Feedback> Feedbacks { get; set; } = default!;
        public virtual Publisher Publisher { get; set; } = default!;
        public virtual ICollection<OrderItem> OrderItems { get; set; } = default!;
        public virtual ICollection<CartItem> CartItems { get; set; } = default!;
    }
}
