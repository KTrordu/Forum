using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forum.Entities
{
    internal class PostContents : BaseEntity
    {
        [Key]
        public int PostContentId { get; set; }

        public string? ImagePath { get; set; }

        [Required]
        [StringLength(200)]
        public string PostDescription { get; set; }

        public int? PostId { get; set; }
        public Posts Posts { get; set; }
    }
}
