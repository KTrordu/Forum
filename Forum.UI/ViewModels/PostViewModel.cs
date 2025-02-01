using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Forum.UI.ViewModels
{
    public class PostViewModel
    {
        [Required]
        public int Id { get; set; }

        [DisplayName("Community")]
        public int CommunityId { get; set; }

        public List<SelectListItem>? Communities { get; set; }

        [DisplayName("Community Name")]
        public string? CommunityName { get; set; }

        [DisplayName("Topic")]
        public int TopicId { get; set; }

        public List<SelectListItem>? Topics { get; set; }

        [DisplayName("Topic Name")]
        public string? TopicName { get; set; }

        [DisplayName("Post Title")]
        public string PostTitle { get; set; }

        [DisplayName("Post Description")]
        public string PostDescription { get; set; }

        [DisplayName("Image")]
        public string? ImagePath { get; set; }

        public IFormFile? ImageFile { get; set; }

        public bool IsLiked { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
