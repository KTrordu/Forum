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
        public int? PostId { get; set; }
        public Post Posts { get; set; }

        [StringLength(50)]
        public string PostTitle { get; set; }

        [StringLength(2000)]
        public string PostDescription { get; set; }

        [StringLength(255)]
        public string? ImagePath { get; set; }
    }
}
