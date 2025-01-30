using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Forum.UI.ViewModels
{
    public class CommunityViewModel
    {
        [Required]
        public int Id { get; set; }

        [Required]
        [DisplayName("Community Name")]
        public string CommunityName { get; set; }

        [Required]
        public bool IsSubscribed { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }
    }
}
