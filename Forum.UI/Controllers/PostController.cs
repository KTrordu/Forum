using Forum.DAL;
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
        public IActionResult Index(int? id)
        {   
            var community = _db.Communities
                .Where(c => c.Id == id)
                .Select(c => new { c.Id, c.CommunityName})
                .FirstOrDefault();

            if (community == null) return NotFound();

            var posts = _db.Posts
                .Where(p => p.Id == id)
                .Select(p => new PostViewModel
                {
                    Id = p.Id,
                    PostTitle = p.PostTitle,
                    IsLiked = p.IsLiked,
                    CommunityId = (int)p.CommunityId!,
                    CommunityName = community.CommunityName,
                    CreatedAt = p.CreatedAt
                })
                .ToList();

            var viewModel = new PostListViewModel
            {
                CommunityId = community.Id,
                CommunityName = community.CommunityName,
                Posts = posts
            };

            return View(viewModel);
        }
    }
}
