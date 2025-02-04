using Forum.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forum.DAL.Repositories
{
    public class TopicRepository
    {
        private readonly ApplicationDbContext _db;

        public TopicRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public Topic? GetTopic(int topicId)
        {
            return _db.Topics
                .Where(t => !t.IsDeleted && t.Id == topicId)
                .FirstOrDefault();
        }

        public List<Topic>? GetTopics()
        {
            return _db.Topics
                .Where(t => !t.IsDeleted)
                .OrderByDescending(t => t.UpdatedAt)
                .ToList();
        }

        public List<Topic>? GetTopicsByCommunity(int communityId)
        {
            return _db.Topics
                .Where(t => !t.IsDeleted && t.CommunityId == communityId)
                .OrderByDescending(t => t.UpdatedAt)
                .ToList();
        }

        public void CreateTopic (int communityId, string topicName)
        {
            var topic = new Topic
            {
                CommunityId = communityId,
                TopicName = topicName
            };

            _db.Topics.Add(topic);
            _db.SaveChanges();
        }

        public void UpdateTopic (int topicId, string newTopicName)
        {
            var topic = GetTopic(topicId);

            topic!.TopicName = newTopicName;
            topic.UpdatedAt = DateTime.Now;

            _db.Topics.Update(topic);
            _db.SaveChanges();
        }

        public void DeleteTopic (int topicId)
        {
            var topic = GetTopic(topicId);

            topic!.IsDeleted = true;
            topic.DeletedAt = DateTime.Now;
            topic.UpdatedAt = DateTime.Now;

            _db.Update(topic);
            _db.SaveChanges();
        }
    }
}
