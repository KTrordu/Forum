using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forum.Entities
{
    public class PostContent : BaseEntity
    {
        public string? ImagePath { get; set; }

        [Required]
        [StringLength(200)]
        public string PostDescription { get; set; }

        public int? PostId { get; set; }
        public Post Posts { get; set; }
    }
}
