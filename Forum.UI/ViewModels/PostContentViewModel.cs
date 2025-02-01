using System.ComponentModel.DataAnnotations;

namespace Forum.UI.ViewModels
{
    public class PostContentViewModel
    {
        public int Id { get; set; }

        public string PostTitle { get; set; }

        public string? ImagePath { get; set; }

        [StringLength(200)]
        public string PostDescription { get; set; }

        public bool IsLiked { get; set; }

        public int PostId { get; set; }

        public int CommunityId { get; set; }

        public int TopicId { get; set; }
    }
}
