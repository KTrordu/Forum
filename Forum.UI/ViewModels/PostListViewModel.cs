using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Forum.UI.ViewModels
{
    public class PostListViewModel
    {
        public int CommunityId { get; set; }

        [DisplayName("Community Name")]
        public string CommunityName { get; set; }

        public int TopicId { get; set; }

        [DisplayName("Topic Name")]
        public string TopicName { get; set; }

        public List<PostViewModel> Posts { get; set; }
    }
}
