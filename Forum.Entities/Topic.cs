using System;
using System.Collections.Generic;
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
    }
}
