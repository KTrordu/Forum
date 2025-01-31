using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Forum.UI.ViewModels
{
    public class PostViewModel
    {
        [Required]
        public int Id { get; set; }

        [Required]
        [DisplayName("Post Title")]
        public string PostTitle { get; set; }

        public bool IsLiked { get; set; } = false;

        public int CommunityId { get; set; }

        [DisplayName("Community Name")]
        public string CommunityName { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }
}
