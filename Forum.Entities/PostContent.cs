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

        public string PostTitle { get; set; }

        public string PostDescription { get; set; }

        public string? ImagePath { get; set; }

        public string? VideoPath { get; set; }
    }
}
