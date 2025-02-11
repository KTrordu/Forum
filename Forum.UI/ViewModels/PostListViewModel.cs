using Microsoft.Extensions.Localization;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Forum.UI.ViewModels
{
    public class PostListViewModel
    {
        private readonly IStringLocalizer<PostListViewModel> _localizer;

        public PostListViewModel(IStringLocalizer<PostListViewModel> localizer)
        {
            _localizer = localizer;
        }

        public PostListViewModel()
        {

        }

        public int? CommunityId { get; set; }

        public string? CommunityName { get; set; }

        public int? TopicId { get; set; }

        public string? TopicName { get; set; }

        public List<PostViewModel> Posts { get; set; }

        public List<PostContentViewModel> Contents { get; set; }
    }
}
