﻿using Forum.DAL;
using Forum.DAL.DTOs;
using Forum.DAL.Repositories;
using Forum.Entities;
using Forum.UI.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Localization;
using System.Linq;

namespace Forum.UI.ViewModels
{
    public class PostController : Controller
    {
        private readonly PostRepository _postRepository;
        private readonly TopicRepository _topicRepository;
        private readonly CommunityRepository _communityRepository;
        private readonly MediaHelper _mediaHelper;
        private readonly CommentRepository _commentRepository;
        private readonly IStringLocalizer<PostViewModel> _postLocalizer;
        private readonly IStringLocalizer<PostContentViewModel> _postContentLocalizer;
        private readonly IStringLocalizer<PostListViewModel> _postListLocalizer;
        private readonly IStringLocalizer<PostController> _localizer;
        private readonly IStringLocalizer<CommentViewModel> _commentLocalizer;

        public PostController(PostRepository postRepository, TopicRepository topicRepository, 
            CommunityRepository communityRepository, MediaHelper mediaHelper, 
            CommentRepository commentRepository, IStringLocalizer<PostViewModel> postLocalizer, 
            IStringLocalizer<PostContentViewModel> postContentLocalizer, IStringLocalizer<PostListViewModel> postListLocalizer,
            IStringLocalizer<PostController> localizer, IStringLocalizer<CommentViewModel> commentLocalizer)
        {
            _postRepository = postRepository;
            _topicRepository = topicRepository;
            _communityRepository = communityRepository;
            _mediaHelper = mediaHelper;
            _commentRepository = commentRepository;
            _postLocalizer = postLocalizer;
            _postContentLocalizer = postContentLocalizer;
            _postListLocalizer = postListLocalizer;
            _localizer = localizer;
            _commentLocalizer = commentLocalizer;
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
            var commentViewModels = new List<CommentViewModel>();

            foreach (var post in posts)
            {
                var topic = _topicRepository.GetTopic(post.TopicId!.Value);
                if (topic == null) return NotFound();

                var community = _communityRepository.GetCommunity(topic.CommunityId!.Value);
                if (community == null) return NotFound();

                var comments = _commentRepository.GetComments(post.Id);
                if (comments == null) return NotFound();

                foreach (var comment in comments)
                {
                    var commentViewModel = new CommentViewModel(_commentLocalizer)
                    {
                        Id = comment.Id,
                        CommentText = comment.CommentText,
                        PostId = post.Id
                    };

                    commentViewModels.Add(commentViewModel);
                }

                var postContentViewModel = new PostContentViewModel(_postContentLocalizer)
                {
                    PostTitle = postContents[post.Id].PostTitle,
                    PostDescription = postContents[post.Id].PostDescription,
                    ImagePath = postContents[post.Id].ImagePath
                };

                var postViewModel = new PostViewModel(_postLocalizer, _postContentLocalizer)
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

            var model = new PostListViewModel(_postListLocalizer)
            {
                Posts = postViewModels,
                Contents = postContentViewModels,
                Comments = commentViewModels
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

            var model = new PostListViewModel(_postListLocalizer)
            {
                CommunityId = community.Id,
                CommunityName = community.CommunityName,
                TopicId = topic.Id,
                TopicName = topic.TopicName,
                Posts = new List<PostViewModel> { },
                Contents = new List<PostContentViewModel> { },
                Comments = new List<CommentViewModel> { }
            };

            foreach (var post in posts)
            {
                var comments = _commentRepository.GetComments(post.Id);
                if (comments == null) return NotFound();

                foreach (var comment in comments)
                {
                    var commentViewModel = new CommentViewModel(_commentLocalizer)
                    {
                        Id = comment.Id,
                        CommentText = comment.CommentText,
                        PostId = post.Id
                    };

                    model.Comments.Add(commentViewModel);
                }

                var postContentViewModel = new PostContentViewModel(_postContentLocalizer)
                {
                    PostTitle = postContents[post.Id].PostTitle,
                    PostDescription = postContents[post.Id].PostDescription,
                    ImagePath = postContents[post.Id].ImagePath
                };

                var postViewModel = new PostViewModel(_postLocalizer, _postContentLocalizer)
                {
                    Id = post.Id,
                    CommunityId = community.Id,
                    CommunityName = community.CommunityName,
                    TopicId = topic.Id,
                    TopicName = topic.TopicName,
                    PostContent = postContentViewModel,
                    IsLiked = post.IsLiked,
                    CreatedAt = post.CreatedAt
                };

                model.Posts.Add(postViewModel);
                model.Contents.Add(postContentViewModel);
            }

            return View("Index", model);
        }

        //READ: List posts by community
        public IActionResult IndexByCommunity(int communityId)
        {
            var community = _communityRepository.GetCommunity(communityId);
            if (community == null) return NotFound();

            var topics = _topicRepository.GetTopicsByCommunity(community.Id);
            if (topics == null) return NotFound();

            var model = new PostListViewModel(_postListLocalizer)
            {
                CommunityId = community.Id,
                CommunityName = community.CommunityName,
                Posts = new List<PostViewModel> {},
                Contents = new List<PostContentViewModel> {},
                Comments = new List<CommentViewModel> { }
            };

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
                    var comments = _commentRepository.GetComments(post.Id);
                    if (comments == null) return NotFound();

                    foreach (var comment in comments)
                    {
                        var commentViewModel = new CommentViewModel(_commentLocalizer)
                        {
                            Id = comment.Id,
                            CommentText = comment.CommentText,
                            PostId = post.Id
                        };

                        model.Comments.Add(commentViewModel);
                    }

                    var postContentViewModel = new PostContentViewModel(_postContentLocalizer)
                    {
                        PostTitle = postContents[post.Id].PostTitle,
                        PostDescription = postContents[post.Id].PostDescription,
                        ImagePath = postContents[post.Id].ImagePath
                    };

                    var postViewModel = new PostViewModel(_postLocalizer, _postContentLocalizer)
                    {
                        Id = post.Id,
                        CommunityId = community.Id,
                        CommunityName = community.CommunityName,
                        TopicId = topic.Id,
                        TopicName = topic.TopicName,
                        PostContent = postContentViewModel,
                        IsLiked = post.IsLiked,
                        CreatedAt = post.CreatedAt
                    };

                    model.Contents.Add(postContentViewModel);
                    model.Posts.Add(postViewModel);
                }
            }

            return View("Index", model);
        }

        //Like or Unlike Post
        public IActionResult LikePost(int id)
        {
            var post = _postRepository.GetPost(id);
            if (post == null) return NotFound();

            _postRepository.LikePost(post);

            return Json(new { isLiked = post.IsLiked });
        }

        //Get Topics for AJAX
        public IActionResult GetTopics(int communityId)
        {
            var topics = _topicRepository.GetTopicsByCommunity(communityId);
            if (topics == null) return NotFound();

            var topicsList = topics
                .Select(t => new SelectListItem
                {
                    Value = t.Id.ToString(),
                    Text = t.TopicName
                })
                .ToList();

            return Json(topicsList);
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

            var model = new PostViewModel(_postLocalizer, _postContentLocalizer)
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

                var viewModel = new PostViewModel(_postLocalizer, _postContentLocalizer)
                {
                    Communities = model.Communities,
                    Topics = model.Topics
                };

                return View(viewModel);
            }

            if (model.PostContent.ImageFile != null)
            {
                try
                {
                    model.PostContent.ImagePath = _mediaHelper.SaveImage(model.PostContent.ImageFile);
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("ImageFile", ex.Message);
                    return View(model);
                }
            }

            var postDto = new PostDTO
            {
                TopicId = model.TopicId
            };

            var postContentDto = new PostContentDTO
            {
                PostTitle = model.PostContent.PostTitle,
                PostDescription = model.PostContent.PostDescription,
                ImagePath = model.PostContent.ImagePath
            };

            _postRepository.CreatePost(postDto, postContentDto);
            TempData["Success"] = "Post created successfully.";

            return RedirectToAction("Index", "Home");
        }

        //UPDATE: GET
        public IActionResult Edit(int id)
        {
            var post = _postRepository.GetPost(id);
            if (post == null) return NotFound();

            var postContent = _postRepository.GetPostContent(post.Id);
            if (postContent == null) return NotFound();

            var topic = _topicRepository.GetTopic((int)post.TopicId!);
            if (topic == null) return NotFound();

            var community = _communityRepository.GetSubscribedCommunity((int)topic.CommunityId!);
            if (community == null) return NotFound();

            var postContentViewModel = new PostContentViewModel(_postContentLocalizer)
            {
                PostTitle = postContent.PostTitle,
                PostDescription = postContent.PostDescription,
                ImagePath = postContent.ImagePath
            };

            var postViewModel = new PostViewModel(_postLocalizer, _postContentLocalizer)
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

                var postDto = new PostDTO
                {
                    Id = post.Id
                };

                var postContentDto = new PostContentDTO
                {
                    PostTitle = model.PostContent.PostTitle,
                    PostDescription = model.PostContent.PostDescription,
                    ImagePath = model.PostContent.ImagePath
                };

                _postRepository.UpdatePost(postDto, postContentDto);
                TempData["Success"] = "Post updated successfully.";

                return RedirectToAction("Index", new { communityId = model.CommunityId });
            }

            var viewModel = new PostViewModel(_postLocalizer, _postContentLocalizer)
            {
                Id = model.Id,
                CommunityId = model.CommunityId,
                Communities = model.Communities,
                CommunityName = model.CommunityName,
                TopicId = model.TopicId,
                Topics = model.Topics,
                TopicName = model.TopicName,
                PostContent = model.PostContent,
                IsLiked = model.IsLiked,
                CreatedAt = model.CreatedAt
            };

            return View(viewModel);
        }

        //DELETE: GET
        public IActionResult Delete()
        {
             return PartialView("_DeleteModal");
        }

        //DELETE: POST
        [HttpPost, ActionName("DeletePost")]
        [ValidateAntiForgeryToken]
        public IActionResult DeletePost(int id)
        {
            var post = _postRepository.GetPost(id);
            if (post == null) return NotFound();

            _postRepository.DeletePost(id);
            return Json(new { success = true, message = "Post deleted successfully." });
        }
    }
}
