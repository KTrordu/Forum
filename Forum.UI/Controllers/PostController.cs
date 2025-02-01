using Forum.DAL;
using Forum.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Forum.UI.ViewModels
{
    public class PostController : Controller
    {
        private readonly ApplicationDbContext _db;

        public PostController(ApplicationDbContext db)
        {
            _db = db;
        }

        //READ: GET
        public IActionResult Index(int? communityId)
        {
            var community = _db.Communities
                .Where(c => c.Id == communityId && !c.IsDeleted)
                .Select(c => new { c.Id, c.CommunityName})
                .FirstOrDefault();

            var posts = _db.Posts
                .Where(p => p.CommunityId == communityId
                    && !p.IsDeleted)
                .Select(p => new PostViewModel
                {
                    Id = p.Id,
                    IsLiked = p.IsLiked,
                    CommunityId = (int)p.CommunityId!,
                    CommunityName = community.CommunityName,
                    CreatedAt = p.CreatedAt
                })
                .ToList();

            var postIds = posts.Select(p => p.Id).ToList();
            var postContents = _db.PostContents
                .Where(pc => postIds.Contains(pc.PostId.Value))
                .ToDictionary(pc => pc.PostId, pc => new { pc.PostTitle, pc.PostDescription, pc.ImagePath });

            if (community == null || posts == null || postIds == null || postContents == null) return NotFound();

            foreach (var post in posts)
            {
                if (postContents.TryGetValue(post.Id, out var content))
                {
                    post.PostTitle = content.PostTitle;
                    post.PostDescription = content.PostDescription;
                    post.ImagePath = content.ImagePath;
                }
            }

            var postListViewModel = new PostListViewModel
            {
                CommunityId = community.Id,
                CommunityName = community.CommunityName,
                Posts = posts
            };

            return View(postListViewModel);
        }

        //CREATE: GET
        public IActionResult Create(int? communityId, int? topicId)
        {
            if (communityId == null || communityId == 0) return NotFound();

            var community = _db.Communities
                .Where(c => !c.IsDeleted)
                .FirstOrDefault(c => c.Id == communityId);

            var topic = _db.Topics
                .Where(t => !t.IsDeleted)
                .FirstOrDefault(t => t.Id == topicId);

            if (community == null || topic == null) return NotFound();

            var model = new PostViewModel
            {
                CommunityId = community.Id,
                CommunityName = community.CommunityName,
                TopicId = topic.Id,
                TopicName = topic.TopicName
            };

            return View(model);
        }

        //CREATE: POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(PostViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var community = _db.Communities
                .Where(c => !c.IsDeleted)
                .FirstOrDefault(c => c.Id == model.CommunityId);

            var topic = _db.Topics
                .Where(t => !t.IsDeleted)
                .FirstOrDefault(t => t.Id == model.TopicId);

            if (community == null || topic == null) return NotFound();

            var post = new Post
                {
                    CommunityId = model.CommunityId,
                    TopicId = model.Id,
                };

                _db.Posts.Add(post);
                _db.SaveChanges();

            var postContent = new PostContent
            {
                PostId = post.Id,
                PostTitle = model.PostTitle,
                PostDescription = model.PostDescription,
                ImagePath = model.ImagePath
            };

            _db.PostContents.Add(postContent);
            _db.SaveChanges();

            return RedirectToAction("Index", new {communityId = model.CommunityId});
        }

        //UPDATE: GET
        public IActionResult Edit(int? id, int? communityId, int? topicId)
        {
            if (id == null || id == 0) return NotFound();

            var community = _db.Communities
                .Where(c => !c.IsDeleted)
                .FirstOrDefault(c => c.Id == communityId);

            var topic = _db.Topics
                .Where(t => !t.IsDeleted)
                .FirstOrDefault(t => t.Id == topicId);

            var post = _db.Posts
                .Where(p => !p.IsDeleted)
                .FirstOrDefault(p => p.Id == id);

            var postContent = _db.PostContents
                .Where(p => !p.IsDeleted)
                .FirstOrDefault(p => p.PostId == post!.Id);

            if (community == null || topic == null || post == null || postContent == null) return NotFound();

            var model = new PostViewModel
            {
                Id = post.Id,
                PostTitle = postContent.PostTitle,
                PostDescription = postContent.PostDescription,
                ImagePath = postContent.ImagePath,
                IsLiked = post.IsLiked,
                CommunityId = community.Id,
                CommunityName = community.CommunityName,
                TopicId = topic.Id,
                TopicName = topic.TopicName,
                CreatedAt = post.CreatedAt
            };

            return View(model);
        }

        //UPDATE: POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(PostViewModel model)
        {
            if (ModelState.IsValid)
            {
                var community = _db.Communities
                    .Where(c => !c.IsDeleted)
                    .FirstOrDefault(c => c.Id == model.CommunityId);

                var topic = _db.Topics
                    .Where(t => !t.IsDeleted)
                    .FirstOrDefault(t => t.Id == model.TopicId);

                var post = _db.Posts
                    .Where(p => !p.IsDeleted)
                    .FirstOrDefault(p => p.Id == model.Id);

                if (community == null || topic == null || post == null) return NotFound();

                var postContent = _db.PostContents
                    .Where(p => !p.IsDeleted)
                    .FirstOrDefault(p => p.Id == post.Id);

                if (postContent == null) return NotFound();

                post.CommunityId = community.Id;
                post.TopicId = topic.Id;
                post.UpdatedAt = DateTime.Now;
                _db.Posts.Update(post);

                postContent.PostTitle = model.PostTitle;
                postContent.PostDescription = model.PostDescription;
                postContent.ImagePath = model.ImagePath;
                _db.PostContents.Update(postContent);

                _db.SaveChanges();

                return RedirectToAction("Index", new { communityId = model.CommunityId });
            }

            return View(model);
        }

        //DELETE: GET
        public IActionResult Delete(int? id, int? communityId, int? topicId)
        {
            if (id == null || id == 0 || communityId == null || communityId == 0 || topicId == null || topicId == 0) return NotFound();

            var community = _db.Communities
                .Where(c => !c.IsDeleted)
                .FirstOrDefault(c => c.Id == communityId);

            var topic = _db.Topics
                .Where(t => !t.IsDeleted)
                .FirstOrDefault(t => t.Id == topicId);

            var post = _db.Posts
                .Where(p => !p.IsDeleted)
                .FirstOrDefault(p => p.Id == id);

            if (community == null || topic == null || post == null) return NotFound();

            var postContent = _db.PostContents
                .Where(p => !p.IsDeleted)
                .FirstOrDefault(p => p.Id == post.Id);

            if (postContent == null) return NotFound();

            var model = new PostViewModel
            {
                Id = post.Id,
                PostTitle = postContent.PostTitle,
                PostDescription = postContent.PostDescription,
                ImagePath = postContent.ImagePath,
                IsLiked = post.IsLiked,
                CommunityId = community.Id,
                CommunityName = community.CommunityName,
                TopicId = topic.Id,
                TopicName = topic.TopicName,
                CreatedAt = post.CreatedAt
            };

            return View(model);
        }

        //DELETE: POST
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeletePOST(int? id, int? communityId, int? topicId)
        {
            if (id == null || id == 0 || communityId == null || communityId == 0 || topicId == null || topicId == 0) return NotFound();

            var post = _db.Posts
                .Where(p => !p.IsDeleted)
                .FirstOrDefault(p => p.Id == id);

            if (post == null) return NotFound();

            var postContent = _db.PostContents
                .Where(p => !p.IsDeleted)
                .FirstOrDefault(p => p.Id == post.Id);

            if (postContent == null) return NotFound();

            post.IsDeleted = true;
            post.DeletedAt = DateTime.Now;
            post.UpdatedAt = DateTime.Now;
            _db.Posts.Update(post);

            postContent.IsDeleted = true;
            postContent.DeletedAt = DateTime.Now;
            postContent.UpdatedAt = DateTime.Now;
            _db.PostContents.Update(postContent);

            _db.SaveChanges();

            return RedirectToAction("Index", new {id = communityId});
        }
    }
}
