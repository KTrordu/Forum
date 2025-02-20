using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forum.DAL.DTOs
{
    public class PostContentDTO
    {
        public int Id { get; set; }
        
        public int PostId { get; set; }

        public string PostTitle { get; set; }

        public string PostDescription { get; set; }

        public string? ImagePath { get; set; }
    }
}
