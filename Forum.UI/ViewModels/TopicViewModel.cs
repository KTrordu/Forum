using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Localization;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Forum.UI.ViewModels
{
    public class TopicViewModel
    {
        private readonly IStringLocalizer<TopicViewModel> _localizer;

        public TopicViewModel(IStringLocalizer<TopicViewModel> localizer)
        {
            _localizer = localizer;
        }

        public TopicViewModel()
        {

        }

        public string? TopicNameLabel => _localizer?["TopicNameLabel"];
        public string? CommunityIdLabel => _localizer?["CommunityIdLabel"];
        public string? CommunityNameLabel => _localizer?["CommunityNameLabel"];

        [Required]
        public int Id { get; set; }

        [StringLength(20)]
        [Required(ErrorMessage = "TopicNameRequired")]
        public string TopicName { get; set; }

        [Required(ErrorMessage = "CommunityIdRequired")]
        public int CommunityId { get; set; }

        public string? CommunityName { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
