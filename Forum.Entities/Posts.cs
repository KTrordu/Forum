using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forum.Entities
{
    internal class Posts : BaseEntity
    {
        [Key]
        public int PostId { get; set; }

        [Required]
        [StringLength(50)]
        public string PostTitle { get; set; }

        public int? CommunityId { get; set; }
        public Communities Communities { get; set; }
    }
}
