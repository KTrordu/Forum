using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Forum.UI.Models;
using Forum.DAL;
using Forum.UI.ViewModels;
using Forum.DAL.Repositories;
using Microsoft.AspNetCore.Localization;

namespace Forum.UI.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly PostRepository _postRepository;
    private readonly TopicRepository _topicRepository;
    private readonly CommunityRepository _communityRepository;

    public HomeController(ILogger<HomeController> logger, PostRepository postRepository, TopicRepository topicRepository, CommunityRepository communityRepository)
    {
        _logger = logger;
        _postRepository = postRepository;
        _topicRepository = topicRepository;
        _communityRepository = communityRepository;
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

            var postContentViewModel = new PostContentViewModel
            {
                PostTitle = postContents[post.Id].PostTitle,
                PostDescription = postContents[post.Id].PostDescription,
                ImagePath = postContents[post.Id].ImagePath,
                VideoPath = postContents[post.Id].VideoPath
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

    [HttpPost]
    public IActionResult SetLanguage(string culture, string returnUrl)
    {
        Response.Cookies.Append(
            CookieRequestCultureProvider.DefaultCookieName,
            CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
            new CookieOptions { Expires = DateTimeOffset.Now.AddYears(1) }
        );

        return LocalRedirect(returnUrl);
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
