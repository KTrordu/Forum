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
        [StringLength(50)]
        public string PostTitle { get; set; }

        [Required(ErrorMessage = "PostDescriptionRequired")]
        [StringLength(2000)]
        public string PostDescription { get; set; }

        public string? ImagePath { get; set; }

        public IFormFile? ImageFile { get; set; }
    }
}
