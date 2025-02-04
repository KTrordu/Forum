using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Forum.UI.ViewModels
{
    public class PostListViewModel
    {
        public int? CommunityId { get; set; }

        public string? CommunityName { get; set; }

        public int? TopicId { get; set; }

        public string? TopicName { get; set; }

        public List<PostViewModel> Posts { get; set; }

        public List<PostContentViewModel> Contents { get; set; }
    }
}
