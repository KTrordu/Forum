using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Localization;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Forum.UI.ViewModels
{
    public class PostViewModel
    {
        private readonly IStringLocalizer<PostViewModel> _localizer;
        private readonly IStringLocalizer<PostContentViewModel> _contentLocalizer;

        public PostViewModel(IStringLocalizer<PostViewModel> localizer, IStringLocalizer<PostContentViewModel> contentLocalizer)
        {
            _localizer = localizer;
            _contentLocalizer = contentLocalizer;
            PostContent = new PostContentViewModel(contentLocalizer);
        }

        public PostViewModel() : this(null!, null!) { }

        public string? TopicIdLabel => _localizer?["TopicIdLabel"];
        public string? TopicNameLabel => _localizer?["TopicNameLabel"];
        public string? CommunityIdLabel => _localizer?["CommunityIdLabel"];
        public string? CommunityNameLabel => _localizer?["CommunityNameLabel"];

        public int Id { get; set; }

        [Required(ErrorMessage = "CommunityIdRequired")]
        public int CommunityId { get; set; }

        public List<SelectListItem>? Communities { get; set; }

        public string? CommunityName { get; set; }

        [Required(ErrorMessage = "TopicIdRequired")]
        public int TopicId { get; set; }

        public List<SelectListItem>? Topics { get; set; }
        
        public string? TopicName { get; set; }

        public PostContentViewModel PostContent { get; set; }

        public bool IsLiked { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
