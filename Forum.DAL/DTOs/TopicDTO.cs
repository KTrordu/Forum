namespace Forum.DAL.DTOs
{
    public class TopicDTO
    {
        public int Id { get; set; }

        public string TopicName { get; set; }

        public string CreatedAt { get; set; }

        public int CommunityId { get; set; }

        public int? ParentId { get; set; }

        public List<TopicDTO> Subtopics { get; set; } = new();
    }
}
