using Forum.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forum.DAL.Repositories
{
    public class PostRepository
    {
        private readonly ApplicationDbContext _db;

        public PostRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public Post? GetPost(int id)
        {
            return _db.Posts
                .FirstOrDefault(p => !p.IsDeleted && p.Id == id);
        }

        public List<Post>? GetPosts()
        {
            return _db.Posts
                .Where(p => !p.IsDeleted)
                .OrderByDescending(p => p.UpdatedAt)
                .ToList();
        }

        public List<Post>? GetPostsByIds(List<int> postIds)
        {
            var posts = new List<Post>();

            foreach (var postId in postIds)
            {
                var post = _db.Posts
                    .FirstOrDefault(p => p.Id == postId);

                posts.Add(post!);
            }

            return posts;
        }

        public List<Post>? GetPostsByTopic(int topicId)
        {
            return _db.Posts
                .Where(p => !p.IsDeleted && p.TopicId == topicId)
                .ToList();
        }

        public PostContent? GetPostContent(int postId)
        {
            return _db.PostContents
                .Where(pc => !pc.IsDeleted && pc.PostId == postId)
                .FirstOrDefault();
        }

        public Dictionary<int, PostContent>? GetPostContents(List<int> postIds)
        {
            return _db.PostContents
                .Where(pc => !pc.IsDeleted && postIds.Contains(pc.PostId!.Value))
                .ToDictionary(pc => pc.PostId!.Value, pc => pc);
        }

        public void CreatePost(int topicId, string postTitle, string postDescription, string? imagePath)
        {
            var post = new Post
            {
                TopicId = topicId
            };
            _db.Posts.Add(post);

            var postContent = new PostContent
            {
                PostId = post.Id,
                PostTitle = postTitle,
                PostDescription = postDescription,
                ImagePath = imagePath
            };
            _db.PostContents.Add(postContent);

            _db.SaveChanges();
        }

        public void UpdatePost(int id, string newPostTitle, string newPostDescription, string? newImagePath)
        {
            var post = GetPost(id);
            var postContent = GetPostContent(post!.Id);

            post.UpdatedAt = DateTime.Now;
            _db.Posts.Update(post);

            postContent!.PostTitle = newPostTitle;
            postContent.PostDescription = newPostDescription;
            postContent.ImagePath = newImagePath;
            postContent.UpdatedAt = DateTime.Now;
            _db.Posts.Update(post);
            
            _db.SaveChanges();
        }

        public void LikePost(Post post)
        {
            post.IsLiked = !post.IsLiked;
            post.UpdatedAt = DateTime.Now;
            _db.Posts.Update(post);

            _db.SaveChanges();
        }

        public void DeletePost(int id)
        {
            var post = GetPost(id);
            var postContent = GetPostContent(id);

            post!.IsDeleted = true;
            post.DeletedAt = DateTime.Now;
            post.UpdatedAt = DateTime.Now;
            _db.Posts.Update(post);

            postContent!.IsDeleted = true;
            postContent.DeletedAt = DateTime.Now;
            postContent.UpdatedAt = DateTime.Now;
            _db.PostContents.Update(postContent);

            _db.SaveChanges();
        }
    }
}
