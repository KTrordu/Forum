using Microsoft.Extensions.Localization;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Forum.UI.ViewModels
{
    public class CommunityViewModel
    {
        private readonly IStringLocalizer<CommunityViewModel> _localizer;

        public CommunityViewModel(IStringLocalizer<CommunityViewModel> localizer)
        {
            _localizer = localizer;
        }

        public CommunityViewModel()
        {
            
        }

        public string? CommunityNameLabel => _localizer?["CommunityNameLabel"];

        [Required]
        public int Id { get; set; }

        [Required(ErrorMessage = "CommunityNameRequired")]
        public string CommunityName { get; set; }

        public bool IsSubscribed { get; set; } = false;

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
