using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class CartItem
    {
        public int Id { get; set; }
        public int BookId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Số lượng phải lớn hơn 0.")]
        [Required(ErrorMessage = "Trường này là bắt buộc.")]
        public required int Quantity { get; set; }
        public int UserId { get; set; }
    }
}
