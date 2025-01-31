using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Forum.UI.ViewModels
{
    public class PostListViewModel
    {
        [Required]
        public int CommunityId { get; set; }

        [DisplayName("Community Name")]
        public string CommunityName { get; set; }

        public List<PostViewModel> Posts { get; set; }
    }
}
