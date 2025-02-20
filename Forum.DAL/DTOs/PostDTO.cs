using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forum.DAL.DTOs
{
    public class PostDTO
    {
        public int Id { get; set; }

        public int TopicId { get; set; }

        public bool IsDeleted { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }
        
        public DateTime DeletedAt { get; set; }
    }
}
