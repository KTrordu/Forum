using Microsoft.Extensions.Localization;
using System.ComponentModel;

namespace Forum.UI.ViewModels
{
    public class TopicListViewModel
    {
        private readonly IStringLocalizer<TopicListViewModel> _localizer;

        public TopicListViewModel(IStringLocalizer<TopicListViewModel> localizer)
        {
            _localizer = localizer;
        }

        public TopicListViewModel()
        {

        }

        public string? CommunityNameLabel => _localizer["CommunityNameLabel"];

        public int? CommunityId { get; set; }
        
        public string? CommunityName { get; set; }

        public List<TopicViewModel> Topics { get; set; }
    }
}
