using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forum.Entities
{
    public class Community : BaseEntity
    {
        [Key]
        public int CommunityId { get; set; }

        [Required]
        public string CommunityName { get; set; }

        public bool IsSubscribed { get; set; } = false;
    }
}
