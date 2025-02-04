using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Forum.UI.Models;
using Forum.DAL;
using Forum.UI.ViewModels;
using Forum.Entities;

namespace Forum.UI.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly ApplicationDbContext _db;

    public HomeController(ILogger<HomeController> logger, ApplicationDbContext db)
    {
        _logger = logger;
        _db = db;
    }

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

        var postViewModels = new List<PostViewModel>();
        var postContentViewModels = new List<PostContentViewModel>();

        Random random = new Random();

        for (int i = 0; i < posts.Count; i++)
        {
            int index = random.Next(postIds.Count);
            int idToDisplay = postIds[index];
            postIds.Remove(idToDisplay);

            var post = posts
                .Where(p => p.Id == idToDisplay)
                .FirstOrDefault();

            if (post == null) return NotFound();

            var topic = _db.Topics
                .Where(t => !t.IsDeleted && t.Id == post.TopicId)
                .FirstOrDefault();

            if (topic == null) return NotFound();

            var community = _db.Communities
                .Where(c => !c.IsDeleted && c.Id == topic.CommunityId)
                .FirstOrDefault();

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
