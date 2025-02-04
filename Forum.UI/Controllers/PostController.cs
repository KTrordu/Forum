using Forum.DAL;
using Forum.DAL.Repositories;
using Forum.UI.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Linq;

namespace Forum.UI.ViewModels
{
    public class PostController : Controller
    {
        private readonly PostRepository _postRepository;
        private readonly TopicRepository _topicRepository;
        private readonly CommunityRepository _communityRepository;

        public PostController(PostRepository postRepository, TopicRepository topicRepository, CommunityRepository communityRepository)
        {
            _postRepository = postRepository;
            _topicRepository = topicRepository;
            _communityRepository = communityRepository;
        }

        //READ: List all posts
        public IActionResult Index()
        {
            var posts = _postRepository.GetPosts();
            if (posts == null) return NotFound();

            var postIds = posts
                .Select(p => p.Id)
                .ToList();

            var postContents = _postRepository.GetPostContents(postIds);

            if (postContents == null) return NotFound();

            var postViewModels = new List<PostViewModel>();
            var postContentViewModels = new List<PostContentViewModel>();

            foreach (var post in posts)
            {
                var topic = _topicRepository.GetTopic(post.TopicId!.Value);

                if (topic == null) return NotFound();

                var community = _communityRepository.GetCommunity(topic.CommunityId!.Value);

                if (community == null) return NotFound();

                var postContentViewModel = new PostContentViewModel
                {
                    PostTitle = postContents[post.Id].PostTitle,
                    PostDescription = postContents[post.Id].PostDescription,
                    ImagePath = postContents[post.Id].ImagePath
                };

                var postViewModel = new PostViewModel
                {
                    Id = post.Id,
                    PostContent = postContentViewModel,
                    CommunityId = community.Id,
                    CommunityName = community.CommunityName,
                    TopicId = topic.Id,
                    TopicName = topic.TopicName,
                    IsLiked = post.IsLiked,
                    CreatedAt = post.CreatedAt
                };

                postViewModels.Add(postViewModel);
                postContentViewModels.Add(postContentViewModel);
            }

            var model = new PostListViewModel
            {
                Posts = postViewModels,
                Contents = postContentViewModels
            };

            return View(model);
        }

        //READ: List posts by topic
        public IActionResult IndexByTopic(int topicId)
        {
            var topic = _topicRepository.GetTopic(topicId);
            if (topic == null) return NotFound();

            var community = _communityRepository.GetCommunity(topic.CommunityId!.Value);
            if (community == null) return NotFound();

            var posts = _postRepository.GetPostsByTopic(topic.Id);
            if (posts == null) return NotFound();

            var postIds = posts
                .Select(p => p.Id)
                .ToList();

            var postContents = _postRepository.GetPostContents(postIds);
            if (postContents == null) return NotFound();

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
            var community = _communityRepository.GetCommunity(communityId);
            if (community == null) return NotFound();

            var topics = _topicRepository.GetTopicsByCommunity(community.Id);
            if (topics == null) return NotFound();

            var model = new PostListViewModel {};

            foreach (var topic in topics)
            {
                var posts = _postRepository.GetPostsByTopic(topic.Id);
                if (posts == null) return NotFound();

                var postIds = posts
                    .Select(p => p.Id)
                    .ToList();

                var postContents = _postRepository.GetPostContents(postIds);

                if (postContents == null) return NotFound();

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

        //Like or Unlike Post
        public IActionResult LikePost(int id)
        {
            var post = _postRepository.GetPost(id);
            if (post == null) return NotFound();

            post.IsLiked = !post.IsLiked;
            _postRepository.UpdatePost(post.Id, post, null);

            return Json(new { isLiked = post.IsLiked });
        }

        //Get Topics for AJAX
        public IActionResult GetTopics()
        {
            var topics = _topicRepository.GetTopics();
            if (topics == null) return NotFound();

            topics
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
            var communities = _communityRepository.GetSubscribedCommunities();
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
                model.Communities = _communityRepository.GetSubscribedCommunities()!
                    .Select(c => new SelectListItem
                    {
                        Value = c.Id.ToString(),
                        Text = c.CommunityName
                    })
                    .ToList();

                model.Topics = _topicRepository.GetTopicsByCommunity(model.CommunityId)!
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

            _postRepository.CreatePost(model.TopicId, model.PostContent.PostTitle, model.PostContent.PostDescription, model.PostContent.ImagePath);

            return RedirectToAction("Index", "Home");
        }

        //UPDATE: GET
        public IActionResult Edit(int? id)
        {
            if (id == null || id == 0) return NotFound();

            var post = _postRepository.GetPost((int)id);
            if (post == null) return NotFound();

            var postContent = _postRepository.GetPostContent(post.Id);
            if (postContent == null) return NotFound();

            var topic = _topicRepository.GetTopic(post.Id);
            if (topic == null) return NotFound();

            var community = _communityRepository.GetSubscribedCommunity((int)topic.CommunityId!);
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
                var post = _postRepository.GetPost(model.Id);
                if (post == null) return NotFound();

                var postContent = _postRepository.GetPostContent(model.Id);
                if (postContent == null) return NotFound();

                _postRepository.UpdatePost(post.Id, post, postContent);

                return RedirectToAction("Index", new { communityId = model.CommunityId });
            }

            return View(model);
        }

        //DELETE: GET
        public IActionResult Delete(int id)
        {
            var post = _postRepository.GetPost(id);
            if (post == null) return NotFound();

            var postContent = _postRepository.GetPostContent(id);
            if (postContent == null) return NotFound();

            var topic = _topicRepository.GetTopic((int)post.TopicId!);
            if (topic == null) return NotFound();

            var community = _communityRepository.GetCommunity((int)topic.CommunityId!);
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
            _postRepository.DeletePost(model.Id);

            return RedirectToAction("Index", new {id = model.CommunityId});
        }
    }
}
