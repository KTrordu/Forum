using Microsoft.Extensions.Localization;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Forum.UI.ViewModels
{
    public class PostContentViewModel
    {
        private readonly IStringLocalizer<PostContentViewModel> _localizer;

        public PostContentViewModel(IStringLocalizer<PostContentViewModel> localizer)
        {
            _localizer = localizer;
        }

        public PostContentViewModel() : this(null!) { }

        public string? PostTitleLabel => _localizer?["PostTitleLabel"];
        public string? PostDescriptionLabel => _localizer?["PostDescriptionLabel"];
        public string? ImagePathLabel => _localizer?["ImagePathLabel"];

        [Required(ErrorMessage = "PostTitleRequired")]
        public string PostTitle { get; set; }

        [Required(ErrorMessage = "PostDescriptionRequired")]
        public string PostDescription { get; set; }

        public string? ImagePath { get; set; }

        public IFormFile? ImageFile { get; set; }

        public string? VideoPath { get; set; }

        public IFormFile? VideoFile { get; set; }
    }
}
