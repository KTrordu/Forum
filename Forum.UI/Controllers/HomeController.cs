using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Forum.UI.Models;
using Forum.DAL;
using Forum.UI.ViewModels;
using Forum.DAL.Repositories;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.Rendering;
using Forum.UI.DTOs;
using Forum.Entities;
using Microsoft.Extensions.Localization;

namespace Forum.UI.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly PostRepository _postRepository;
    private readonly TopicRepository _topicRepository;
    private readonly CommunityRepository _communityRepository;
    private readonly CommentRepository _commentRepository;
    private readonly IStringLocalizer<PostViewModel> _postLocalizer;
    private readonly IStringLocalizer<PostContentViewModel> _postContentLocalizer;
    private readonly IStringLocalizer<PostListViewModel> _postListLocalizer;
    private readonly IStringLocalizer<CommentViewModel> _commentLocalizer;

    public HomeController(ILogger<HomeController> logger, PostRepository postRepository, 
        TopicRepository topicRepository, CommunityRepository communityRepository,
        CommentRepository commentRepository, IStringLocalizer<PostViewModel> postLocalizer,
        IStringLocalizer<PostContentViewModel> postContentLocalizer, IStringLocalizer<PostListViewModel> postListLocalizer,
        IStringLocalizer<CommentViewModel> commentLocalizer)
    {
        _logger = logger;
        _postRepository = postRepository;
        _topicRepository = topicRepository;
        _communityRepository = communityRepository;
        _commentRepository = commentRepository;
        _postLocalizer = postLocalizer;
        _postContentLocalizer = postContentLocalizer;
        _postListLocalizer = postListLocalizer;
        _commentLocalizer = commentLocalizer;
    }

    public IActionResult Index()
    {
        var communities = _communityRepository.GetSubscribedCommunities();
        if (communities == null) return NotFound();

        var topicIds = new List<int>();

        foreach (var community in communities)
        {
            var topics = _topicRepository.GetTopicsByCommunity(community.Id);
            if (topics == null) return NotFound();

            foreach (var topic in topics)
            {
                topicIds.Add(topic.Id);
            }
        }

        var postIds = new List<int>();

        foreach (var topicId in topicIds)
        {
            var posts = _postRepository.GetPostsByTopic(topicId);
            if (posts == null) return NotFound();

            foreach (var post in posts)
            {
                postIds.Add(post.Id);
            }
        }

        var postContents = _postRepository.GetPostContents(postIds);
        if (postContents == null) return NotFound();

        var postViewModels = new List<PostViewModel>();
        var postContentViewModels = new List<PostContentViewModel>();
        var commentViewModels = new List<CommentViewModel>();

        Random random = new Random();
        int postCount = postIds.Count;

        var postList = _postRepository.GetPostsByIds(postIds);
        if (postList == null) return NotFound();

        for (int i = 0; i < postCount; i++)
        {
            int index = random.Next(postIds.Count);
            int idToDisplay = postIds[index];
            postIds.Remove(idToDisplay);

            var post = postList
                .Where(p => p.Id == idToDisplay)
                .FirstOrDefault();
            if (post == null) return NotFound();

            var topic = _topicRepository.GetTopic((int)post.TopicId!);
            if (topic == null) return NotFound();

            var community = _communityRepository.GetCommunity((int)topic.CommunityId!);
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
                ImagePath = postContents[post.Id].ImagePath,
                VideoPath = postContents[post.Id].VideoPath
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

    [HttpPost]
    public IActionResult SetLanguage(string culture)
    {
        try
        {
            Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                new CookieOptions { Expires = DateTimeOffset.Now.AddYears(1) }
            );

            return Json(new { success = true, message = "Language changed succesfully." });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = ex.Message });
        }
    }

    public IActionResult GetCommunitiesTopics()
    {
        try
        {
            var communitiesList = _communityRepository.GetSubscribedCommunities();
            if (communitiesList == null) return NotFound();

            var communitiesSelectList = communitiesList
                .Select(c => new
                {
                    c.Id,
                    c.CommunityName
                })
                .ToList();

            var topicsList = _topicRepository.GetSubscribedTopics(communitiesList);
            if (topicsList == null) return NotFound();

            var topicDtoList = topicsList
                .Select(t => new TopicDTO
                {
                    Id = t.Id,
                    TopicName = t.TopicName,
                    CommunityId = (int)t.CommunityId!,
                    ParentId = t.ParentId,
                    Subtopics = new List<TopicDTO>(),
                    CreatedAt = t.CreatedAt.ToString("yyyy-MM-dd")
                })
                .ToList();

            var rootTopics = topicDtoList
                .Where(t => t.ParentId == null)
                .ToList();

            foreach (var topic in rootTopics)
            {
                BuildTopicHierarchy(topic, topicDtoList);
            }

            return Json(new { success = true, message = "Communities and Topics loaded successfully.", communities = communitiesSelectList, topics = rootTopics });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = ex.Message });
        }
    }

    private void BuildTopicHierarchy(TopicDTO parent, List<TopicDTO> topics)
    {
        var subtopics = topics
            .Where(t => t.ParentId == parent.Id)
            .ToList();

        foreach (var subtopic in subtopics)
        {
            BuildTopicHierarchy(subtopic, topics);
            parent.Subtopics.Add(subtopic);
        }
    }

    public IActionResult ListPosts(int topicId)
    {
        var topic = _topicRepository.GetTopic(topicId);
        if (topic == null) return NotFound();

        var community = _communityRepository.GetCommunity((int)topic.CommunityId!);
        if (community == null) return NotFound();

        var posts = _postRepository.GetPostsByTopic(topic.Id);
        if (posts == null) return NotFound();

        var postContents = _postRepository.GetPostContents(posts);
        if (postContents == null) return NotFound();

        var model = new PostListViewModel(_postListLocalizer)
        {
            TopicId = topic.Id,
            TopicName = topic.TopicName,
            Posts = new List<PostViewModel>(),
            Contents = new List<PostContentViewModel>(),
            Comments = new List<CommentViewModel>()
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
                ImagePath = postContents[post.Id].ImagePath,
                VideoPath = postContents[post.Id].VideoPath
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

        return PartialView("_NavbarPosts", model);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
