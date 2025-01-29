using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forum.Entities
{
    internal class Communities : BaseEntity
    {
        [Key]
        public int CommunityId { get; set; }

        [Required]
        public string CommunityName { get; set; }
    }
}
