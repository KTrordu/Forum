using System.ComponentModel.DataAnnotations;

namespace Forum.UI.ViewModels
{
    public class PostViewModel
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public string PostTitle { get; set; }

        [Required]
        public bool IsLiked { get; set; } = false;

        [Required]
        public int CommunityId { get; set; }

        [Required]
        public string CommunityName { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }
    }
}
