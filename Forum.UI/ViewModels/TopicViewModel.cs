using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Forum.UI.ViewModels
{
    public class TopicViewModel
    {
        [Required]
        public int Id { get; set; }

        [Required]
        [DisplayName("Community")]
        public int CommunityId { get; set; }

        public List<SelectListItem>? Communities { get; set; }

        [DisplayName("Community Name")]
        public string? CommunityName { get; set; }

        [StringLength(20)]
        [DisplayName("Topic Name")]
        public string TopicName { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
