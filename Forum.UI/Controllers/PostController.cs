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
        public IActionResult Index(int? id)
        {
            var community = _db.Communities
                .Where(c => c.Id == id)
                .Select(c => new { c.Id, c.CommunityName})
                .FirstOrDefault();

            if (community == null) return NotFound();

            var posts = _db.Posts
                .Where(p => p.CommunityId == id
                    && p.IsDeleted == false)
                .Select(p => new PostViewModel
                {
                    Id = p.Id,
                    //PostTitle = p.PostTitle,
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

        //CREATE: GET
        public IActionResult Create(int? id)
        {
            if (id == null || id == 0) return NotFound();

            var community = _db.Communities
                .FirstOrDefault(c => c.Id == id);

            if (community == null) return NotFound();

            var model = new PostViewModel
            {
                CommunityId = community.Id,
                CommunityName = community.CommunityName
            };

            return View(model);
        }

        //CREATE: POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(PostViewModel model)
        {
            if (ModelState.IsValid)
            {
                var community = _db.Communities
                    .FirstOrDefault(c => c.Id == model.CommunityId);

                var post = new Post
                {
                    //PostTitle = model.PostTitle,
                    CommunityId = model.CommunityId
                };

                _db.Posts.Add(post);
                _db.SaveChanges();

                return RedirectToAction("Index", new {id = model.CommunityId});
            }

            return View(model);
        }

        //UPDATE: GET
        public IActionResult Edit(int? id, int? communityId)
        {
            if (id == null || id == 0) return NotFound();

            var community = _db.Communities
                .FirstOrDefault(c => c.Id == communityId);

            var post = _db.Posts
                .FirstOrDefault(p => p.Id == id);

            if (community == null || post == null) return NotFound();

            var model = new PostViewModel
            {
                Id = post.Id,
                //PostTitle = post.PostTitle,
                IsLiked = post.IsLiked,
                CommunityId = community.Id,
                CommunityName = community.CommunityName,
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
                    .FirstOrDefault(c => c.Id == model.CommunityId);

                var post = _db.Posts
                    .FirstOrDefault(p => p.Id == model.Id);

                if (community == null || post == null) return NotFound();

                //post.PostTitle = model.PostTitle;
                _db.SaveChanges();

                return RedirectToAction("Index", new { id = model.CommunityId });
            }

            return View(model);
        }

        //DELETE: GET
        public IActionResult Delete(int? id, int? communityId)
        {
            if (id == null || id == 0) return NotFound();

            var community = _db.Communities
                .FirstOrDefault(c => c.Id == communityId);

            var post = _db.Posts
                .FirstOrDefault(p => p.Id == id);

            if (community == null || post == null) return NotFound();

            var model = new PostViewModel
            {
                Id = post.Id,
                //PostTitle = post.PostTitle,
                IsLiked = post.IsLiked,
                CommunityId = community.Id,
                CommunityName = community.CommunityName,
                CreatedAt = post.CreatedAt
            };

            return View(model);
        }

        //DELETE: POST
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeletePOST(int? id, int? communityId)
        {
            if (id == null || id == 0) return NotFound();

            var post = _db.Posts
                .FirstOrDefault (p => p.Id == id);

            if (post == null) return NotFound();

            post.IsDeleted = true;
            post.DeletedAt = DateTime.Now;
            post.UpdatedAt = DateTime.Now;

            _db.SaveChanges();
            return RedirectToAction("Index", new {id = communityId});
        }
    }
}
