namespace Forum.UI.ViewModels
{
    public class CommunityViewModel
    {
        public int CommunityId { get; set; }

        public string CommunityName { get; set; }

        public bool IsSubscribed { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
