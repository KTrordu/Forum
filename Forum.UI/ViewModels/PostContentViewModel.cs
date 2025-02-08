using System.ComponentModel;

namespace Forum.UI.ViewModels
{
    public class PostContentViewModel
    {
        [DisplayName("Post Title")]
        public string PostTitle { get; set; }

        [DisplayName("Post Description")]
        public string PostDescription { get; set; }

        public string? ImagePath { get; set; }

        public IFormFile? ImageFile { get; set; }

        public string? VideoPath { get; set; }

        public IFormFile? VideoFile { get; set; }
    }
}
