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

        [StringLength(20)]
        [DisplayName("Topic Name")]
        public string TopicName { get; set; }

        [DisplayName("Community")]
        public int CommunityId { get; set; }

        [DisplayName("Community Name")]
        public string? CommunityName { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
