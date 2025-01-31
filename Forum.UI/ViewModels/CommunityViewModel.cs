using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Forum.UI.ViewModels
{
    public class CommunityViewModel
    {
        [Required]
        public int Id { get; set; }

        [DisplayName("Community Name")]
        public string CommunityName { get; set; }

        public bool IsSubscribed { get; set; } = false;

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }
}
