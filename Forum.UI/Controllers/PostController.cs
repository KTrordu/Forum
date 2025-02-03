using Forum.DAL;
using Forum.Entities;
using Forum.UI.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Linq;

namespace Forum.UI.ViewModels
{
    public class PostController : Controller
    {
        private readonly ApplicationDbContext _db;

        public PostController(ApplicationDbContext db)
        {
            _db = db;
        }

        //READ: List all posts
        public IActionResult Index()
        {
            var posts = _db.Posts
                .Where(p => !p.IsDeleted)
                .OrderByDescending(p => p.UpdatedAt)
                .ToList();

            var postIds = posts
                .Select(p => p.Id)
                .ToList();

            var postContents = _db.PostContents
                .Where(pc => !pc.IsDeleted && postIds.Contains(pc.PostId!.Value))
                .ToDictionary(pc => pc.PostId!.Value, pc => new { pc.PostTitle, pc.PostDescription, pc.ImagePath });

            if (posts == null || postContents == null) return NotFound();

            var model = new PostListViewModel();

            foreach (var post in posts)
            {
                var topic = _db.Topics
                    .Where(t => !t.IsDeleted && t.Id == post.TopicId)
                    .FirstOrDefault();

                if (topic == null) return NotFound();

                var community = _db.Communities
                    .Where(c => !c.IsDeleted && c.Id == topic.CommunityId)
                    .FirstOrDefault();

                if (community == null) return NotFound();

                var postViewModel = new PostViewModel
                {
                    Id = post.Id,
                    CommunityId = community.Id,
                    CommunityName = community.CommunityName,
                    TopicId = topic.Id,
                    TopicName = topic.TopicName,
                    IsLiked = post.IsLiked,
                    CreatedAt = post.CreatedAt
                };

                var postContentViewModel = new PostContentViewModel
                {
                    PostTitle = postContents[post.Id].PostTitle,
                    PostDescription = postContents[post.Id].PostDescription,
                    ImagePath = postContents[post.Id].ImagePath
                };

                model.Posts.Add(postViewModel);
                model.Contents.Add(postContentViewModel);
            }

            return View(model);
        }

        //READ: List posts by topic
        public IActionResult IndexByTopic(int topicId)
        {
            var topic = _db.Topics
                .Where(t => !t.IsDeleted && t.Id == topicId)
                .FirstOrDefault();

            if (topic == null) return NotFound();

            var community = _db.Communities
                .Where(c => !c.IsDeleted && c.Id == topic.CommunityId)
                .FirstOrDefault();

            if (community == null) return NotFound();

            var posts = _db.Posts
                .Where(p => !p.IsDeleted && p.TopicId == topicId)
                .OrderByDescending(p => p.UpdatedAt)
                .ToList();

            var postIds = posts
                .Select(p => p.Id)
                .ToList();

            var postContents = _db.PostContents
                .Where(pc => !pc.IsDeleted && postIds.Contains(pc.PostId!.Value))
                .ToDictionary(pc => pc.PostId!.Value, pc => new { pc.PostTitle, pc.PostDescription, pc.ImagePath });

            if (posts == null || postContents == null) return NotFound();

            var model = new PostListViewModel {};

            foreach (var post in posts)
            {
                var postViewModel = new PostViewModel
                {
                    Id = post.Id,
                    CommunityId = community.Id,
                    CommunityName = community.CommunityName,
                    TopicId = topic.Id,
                    TopicName = topic.TopicName,
                    IsLiked = post.IsLiked,
                    CreatedAt = post.CreatedAt
                };

                var postContentViewModel = new PostContentViewModel
                {
                    PostTitle = postContents[post.Id].PostTitle,
                    PostDescription = postContents[post.Id].PostDescription,
                    ImagePath = postContents[post.Id].ImagePath
                };

                model.Posts.Add(postViewModel);
                model.Contents.Add(postContentViewModel);
            }

            return View(model);
        }

        //READ: List posts by community
        public IActionResult IndexByCommunity(int communityId)
        {
            var community = _db.Communities
                .Where(c => !c.IsDeleted && c.Id == communityId)
                .FirstOrDefault();

            if (community == null) return NotFound();

            var topics = _db.Topics
                .Where(t => !t.IsDeleted && t.CommunityId == communityId)
                .OrderByDescending(t => t.UpdatedAt)
                .ToList();

            var model = new PostListViewModel {};

            foreach (var topic in topics)
            {
                var posts = _db.Posts
                    .Where(p => !p.IsDeleted && p.Id == topic.Id)
                    .OrderByDescending (p => p.UpdatedAt)
                    .ToList();

                var postIds = posts
                    .Select(p => p.Id)
                    .ToList();

                var postContents = _db.PostContents
                .Where(pc => !pc.IsDeleted && postIds.Contains(pc.PostId!.Value))
                .ToDictionary(pc => pc.PostId!.Value, pc => new { pc.PostTitle, pc.PostDescription, pc.ImagePath });

                if (posts == null || postContents == null) return NotFound();

                foreach (var post in posts)
                {
                    var postViewModel = new PostViewModel
                    {
                        Id = post.Id,
                        CommunityId = community.Id,
                        CommunityName = community.CommunityName,
                        TopicId = topic.Id,
                        TopicName = topic.TopicName,
                        IsLiked = post.IsLiked,
                        CreatedAt = post.CreatedAt
                    };

                    var postContentViewModel = new PostContentViewModel
                    {
                        PostTitle = postContents[post.Id].PostTitle,
                        PostDescription = postContents[post.Id].PostDescription,
                        ImagePath = postContents[post.Id].ImagePath
                    };

                    model.Posts.Add(postViewModel);
                    model.Contents.Add(postContentViewModel);
                }
            }

            return View(model);
        }

        //Get Topics for AJAX
        public IActionResult GetTopics()
        {
            var topics = _db.Topics
                .Where(t => !t.IsDeleted)
                .OrderByDescending(t => t.UpdatedAt)
                .Select(t => new SelectListItem
                {
                    Value = t.Id.ToString(),
                    Text = t.TopicName
                })
                .ToList();

            return Json(topics);
        }

        //CREATE: GET
        public IActionResult Create()
        {
            var communities = _db.Communities
                .Where(c => !c.IsDeleted && c.IsSubscribed)
                .OrderByDescending(c => c.UpdatedAt)
                .ToList();

            if (communities == null) return NotFound();

            var communitiesList = communities
                .Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.CommunityName
                })
                .ToList();

            var model = new PostViewModel
            {
                Communities = communitiesList,
                Topics = new List<SelectListItem>()
            };

            return View(model);
        }

        //CREATE: POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(PostViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Communities = _db.Communities
                    .Where(c => !c.IsDeleted && c.IsSubscribed)
                    .OrderByDescending(c => c.UpdatedAt)
                    .Select(c => new SelectListItem
                    {
                        Value = c.Id.ToString(),
                        Text = c.CommunityName
                    })
                    .ToList();

                model.Topics = _db.Topics
                    .Where(t => t.CommunityId == model.CommunityId && !t.IsDeleted)
                    .OrderByDescending(t =>  t.UpdatedAt)
                    .Select(t => new SelectListItem
                    {
                        Value = t.Id.ToString(),
                        Text = t.TopicName
                    })
                    .ToList();

                return View(model);
            }

            string? imagePath = model.PostContent.ImageFile != null
                ? "/uploads/" + ImageHelper.SaveImage(model.PostContent.ImageFile)
                : null;

            var post = new Post
                {
                    TopicId = model.TopicId
                };

                _db.Posts.Add(post);
                _db.SaveChanges();

            var postContent = new PostContent
            {
                PostId = post.Id,
                PostTitle = model.PostContent.PostTitle,
                PostDescription = model.PostContent.PostDescription,
                ImagePath = imagePath
            };

            _db.PostContents.Add(postContent);
            _db.SaveChanges();

            return RedirectToAction("Index", new {communityId = model.CommunityId});
        }

        //UPDATE: GET
        public IActionResult Edit(int? id)
        {
            if (id == null || id == 0) return NotFound();

            var post = _db.Posts
                .Where(p => !p.IsDeleted)
                .FirstOrDefault(p => p.Id == id);

            if (post == null) return NotFound();

            var postContent = _db.PostContents
                .Where(pc => !pc.IsDeleted && pc.PostId == post.Id)
                .FirstOrDefault();

            if (postContent == null) return NotFound();

            var topic = _db.Topics
                .Where(t => !t.IsDeleted && post.TopicId == t.Id)
                .FirstOrDefault();

            if (topic == null) return NotFound();

            var community = _db.Communities
                .Where(c => !c.IsDeleted && c.IsSubscribed && c.Id == topic.CommunityId)
                .FirstOrDefault();

            if (community == null) return NotFound();

            var postContentViewModel = new PostContentViewModel
            {
                PostTitle = postContent.PostTitle,
                PostDescription = postContent.PostDescription,
                ImagePath = postContent.ImagePath
            };

            var postViewModel = new PostViewModel
            {
                Id = post.Id,
                IsLiked = post.IsLiked,
                CommunityId = community.Id,
                CommunityName = community.CommunityName,
                TopicId = topic.Id,
                TopicName = topic.TopicName,
                PostContent = postContentViewModel,
                CreatedAt = post.CreatedAt
            };

            return View(postViewModel);
        }

        //UPDATE: POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(PostViewModel model)
        {
            if (ModelState.IsValid)
            {
                var post = _db.Posts
                    .Where(p => !p.IsDeleted)
                    .FirstOrDefault(p => p.Id == model.Id);

                if (post == null) return NotFound();

                var postContent = _db.PostContents
                    .Where(p => !p.IsDeleted)
                    .FirstOrDefault(p => p.Id == post.Id);

                if (postContent == null) return NotFound();

                post.UpdatedAt = DateTime.Now;
                _db.Posts.Update(post);

                postContent.PostTitle = model.PostContent.PostTitle;
                postContent.PostDescription = model.PostContent.PostDescription;
                postContent.ImagePath = model.PostContent.ImagePath;
                postContent.UpdatedAt = DateTime.Now;
                _db.PostContents.Update(postContent);

                _db.SaveChanges();

                return RedirectToAction("Index", new { communityId = model.CommunityId });
            }

            return View(model);
        }

        //DELETE: GET
        public IActionResult Delete(int id)
        {
            var post = _db.Posts
                .Where(p => !p.IsDeleted)
                .FirstOrDefault(p => p.Id == id);

            if (post == null) return NotFound();

            var postContent = _db.PostContents
                .Where(p => !p.IsDeleted)
                .FirstOrDefault(p => p.PostId == post.Id);

            if (postContent == null) return NotFound();

            var topic = _db.Topics
                .Where(t => !t.IsDeleted)
                .FirstOrDefault(t => t.Id == post.TopicId);

            if (topic == null) return NotFound();

            var community = _db.Communities
                .Where(c => !c.IsDeleted)
                .FirstOrDefault(c => c.Id == topic.CommunityId);

            if (community == null) return NotFound();

            var postContentViewModel = new PostContentViewModel
            {
                PostTitle = postContent.PostTitle,
                PostDescription = postContent.PostDescription,
                ImagePath = postContent.ImagePath
            };

            var model = new PostViewModel
            {
                Id = post.Id,
                IsLiked = post.IsLiked,
                CommunityId = community.Id,
                CommunityName = community.CommunityName,
                TopicId = topic.Id,
                TopicName = topic.TopicName,
                PostContent = postContentViewModel,
                CreatedAt = post.CreatedAt
            };

            return View(model);
        }

        //DELETE: POST
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeletePOST(PostViewModel model)
        {
            var post = _db.Posts
                .Where(p => !p.IsDeleted)
                .FirstOrDefault(p => p.Id == model.Id);

            if (post == null) return NotFound();

            var postContent = _db.PostContents
                .Where(p => !p.IsDeleted)
                .FirstOrDefault(p => p.PostId == post.Id);

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

            return RedirectToAction("Index", new {id = model.CommunityId});
        }
    }
}
