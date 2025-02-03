using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forum.Entities
{
    public class Post : BaseEntity
    {
        public int? TopicId { get; set; }
        public Topic Topics { get; set; }

        public bool IsLiked { get; set; } = false;
    }
}
