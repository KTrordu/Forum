using System.ComponentModel.DataAnnotations;

namespace Forum.UI.ViewModels
{
    public class PostListViewModel
    {
        [Required]
        public int CommunityId { get; set; }

        [Required]
        public string CommunityName { get; set; }

        [Required]
        public List<PostViewModel> Posts { get; set; }
    }
}
