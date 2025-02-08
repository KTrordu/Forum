using Forum.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forum.DAL.Repositories
{
    public class HelperRepository
    {
        private readonly ApplicationDbContext _db;

        public HelperRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public void DeleteCascading(Community community, List<Topic> topics, List<Post> posts, Dictionary<int, PostContent> postContents)
        {
            foreach (var post in posts)
            {
                post.IsDeleted = true;
                post.DeletedAt = DateTime.Now;
                post.UpdatedAt = DateTime.Now;
                _db.Posts.Update(post);

                postContents[post.Id].IsDeleted = true;
                postContents[post.Id].DeletedAt = DateTime.Now;
                postContents[post.Id].UpdatedAt = DateTime.Now;
                _db.PostContents.Update(postContents[post.Id]);
            }

            foreach (var topic in topics)
            {
                topic.IsDeleted = true;
                topic.DeletedAt = DateTime.Now;
                topic.UpdatedAt = DateTime.Now;
                _db.Topics.Update(topic);
            }

            community.IsDeleted = true;
            community.DeletedAt = DateTime.Now;
            community.UpdatedAt = DateTime.Now;
            _db.Communities.Update(community);

            _db.SaveChanges();
        }
    }
}
