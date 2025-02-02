using Forum.DAL;
using Forum.Entities;
using Forum.UI.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace Forum.UI.Controllers
{
    public class CommunityController : Controller
    {
        private readonly ApplicationDbContext _db;

        public CommunityController(ApplicationDbContext db)
        {
            _db = db;
        }

        //READ: List the communities
        public IActionResult Index()
        {
            var communities = _db.Communities
                .Where(c => !c.IsDeleted)
                .OrderByDescending(c => c.UpdatedAt)
                .Select(c => new CommunityViewModel
                {
                    Id = c.Id,
                    CommunityName = c.CommunityName,
                    CreatedAt = c.CreatedAt,
                    IsSubscribed = c.IsSubscribed
                })
                .ToList();

            return View(communities);
        }

        //CREATE: GET
        public IActionResult Create()
        {
            return View();
        }

        //CREATE: POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(CommunityViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var community = new Community
            {
                CommunityName = model.CommunityName
            };

            _db.Communities.Add(community);
            _db.SaveChanges();

            return RedirectToAction("Index");
        }

        //UPDATE: GET
        public IActionResult Edit(int? id)
        {
            if (id == null || id == 0) return NotFound();

            var community = _db.Communities
                .Where(c => !c.IsDeleted)
                .FirstOrDefault(c => c.Id == id);

            if (community == null) return NotFound();

            var model = new CommunityViewModel
            {
                Id = community.Id,
                CommunityName = community.CommunityName,
                CreatedAt = community.CreatedAt,
                IsSubscribed = community.IsSubscribed
            };

            return View(model);
        }

        //UPDATE: POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(CommunityViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var community = _db.Communities
                .Where(c => !c.IsDeleted)
                .FirstOrDefault(c => c.Id == model.Id);

            if (community == null) return NotFound();

            community.CommunityName = model.CommunityName;
            community.UpdatedAt = DateTime.Now;

            _db.SaveChanges();
            return RedirectToAction("Index");
        }

        //DELETE: GET
        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0) return NotFound();

            var community = _db.Communities
                .Where(c => !c.IsDeleted)
                .FirstOrDefault(c => c.Id == id);

            if (community == null) return NotFound();

            var model = new CommunityViewModel
            {
                Id = community.Id,
                CommunityName = community.CommunityName,
                CreatedAt = community.CreatedAt,
                IsSubscribed = community.IsSubscribed
            };

            return View(model);
        }

        //DELETE: POST
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeletePOST(int? id)
        {
            if (id == null || id == 0) return NotFound();

            var community = _db.Communities
                .Where(c => !c.IsDeleted)
                .FirstOrDefault (c => c.Id == id);

            if (community == null) return NotFound();

            community.IsDeleted = true;
            community.DeletedAt = DateTime.Now;
            community.UpdatedAt = DateTime.Now;

            _db.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
