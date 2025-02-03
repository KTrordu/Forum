using System.ComponentModel;

namespace Forum.UI.ViewModels
{
    public class TopicListViewModel
    {
        public int? CommunityId { get; set; }

        [DisplayName("Community Name")]
        public string? CommunityName { get; set; }

        public List<TopicViewModel> Topics { get; set; }
    }
}
