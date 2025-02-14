using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forum.Entities
{
    public class Topic : BaseEntity
    {
        public int? CommunityId { get; set; }
        public Community Communities { get; set; }

        public string TopicName { get; set; }

        [ForeignKey("Parent")]
        public int? ParentId { get; set; }

        public Topic? Parent { get; set; }

        public List<Topic>? Subtopics { get; set;} = new();
    }
}
