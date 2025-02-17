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

        public List<Topic>? GetSubtopics(Topic topic)
        {
            return _db.Topics
                .Where(t => !t.IsDeleted && t.ParentId == topic.Id)
                .OrderByDescending (t => t.UpdatedAt)
                .ToList();
        }

        public List<Topic>? GetSubscribedTopics(List<Community> subscribedCommunities)
        {
            var topicsList = new List<Topic>();

            foreach (var community in subscribedCommunities)
            {
                var topics = _db.Topics
                    .Where(t => !t.IsDeleted && t.CommunityId == community.Id)
                    .ToList();

                topicsList.AddRange(topics);
            }

            return topicsList;
        }

        public List<Topic>? GetTopicsByCommunity(int communityId)
        {
            return _db.Topics
                .Where(t => !t.IsDeleted && t.CommunityId == communityId)
                .OrderByDescending(t => t.UpdatedAt)
                .ToList();
        }

        public void CreateTopic (int communityId, string topicName, int? parentId)
        {
            var topic = new Topic
            {
                CommunityId = communityId,
                TopicName = topicName,
                ParentId = parentId,
                Subtopics = new()
            };

            if (parentId != null)
            {
                try
                {
                    var superTopic = GetTopic((int)parentId);
                    if (superTopic == null || superTopic.Subtopics == null) throw new NullReferenceException();

                    superTopic.Subtopics.Add(topic);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }

            _db.Topics.Add(topic);
            _db.SaveChanges();
        }

        public void UpdateTopic (int topicId, int newCommunityId, string newTopicName)
        {
            var topic = GetTopic(topicId);

            topic!.TopicName = newTopicName;
            topic.CommunityId = newCommunityId;
            topic.UpdatedAt = DateTime.Now;

            _db.Topics.Update(topic);
            _db.SaveChanges();
        }

        public void UpdateTopic(int topicId, int newCommunityId, string newTopicName, int? parentId)
        {
            var topic = GetTopic(topicId);

            topic!.TopicName = newTopicName;
            topic.CommunityId = newCommunityId;
            topic.ParentId = parentId;
            topic.UpdatedAt = DateTime.Now;

            if (parentId != null)
            {
                try
                {
                    var superTopic = GetTopic((int)parentId);
                    if (superTopic == null || superTopic.Subtopics == null) throw new NullReferenceException();

                    superTopic.Subtopics.Add(topic);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }

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

        public void DeleteTopics (List<Topic> topics)
        {
            foreach (var topic in topics)
            {
                topic!.IsDeleted = true;
                topic.DeletedAt = DateTime.Now;
                topic.UpdatedAt = DateTime.Now;

                _db.Update(topic);
            }
            
            _db.SaveChanges();
        }
    }
}
