using Forum.DAL;
using Forum.Entities;
using Forum.UI.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Forum.UI.Controllers
{
    public class TopicController : Controller
    {
        private readonly ApplicationDbContext _db;

        public TopicController(ApplicationDbContext db)
        {
            _db = db;
        }

        //READ: List all topics
        public IActionResult Index()
        {
            var topics = _db.Topics
                .Where(t => !t.IsDeleted)
                .OrderByDescending(t => t.UpdatedAt)
                .Select(t => new TopicViewModel
                {
                    Id = t.Id,
                    TopicName = t.TopicName,
                    CommunityId = (int)t.CommunityId!,
                    CommunityName = t.Communities.CommunityName,
                    CreatedAt = t.CreatedAt
                })
                .ToList();

            return View(topics);
        }

        //READ: List topics by the communityId
        public IActionResult Index(int? communityId)
        {
            if (communityId == null || communityId == 0) return NotFound();

            var community = _db.Communities
                .Where(t => !t.IsDeleted && t.Id == communityId)
                .FirstOrDefault(t => t.Id == communityId);

            if (community == null) return NotFound();

            var topics = _db.Topics
                .Where(t => t.CommunityId == communityId
                && !t.IsDeleted)
                .OrderByDescending (t => t.UpdatedAt)
                .Select(t => new TopicViewModel
                {
                    Id= t.Id,
                    TopicName = t.TopicName,
                    CommunityId= (int)t.CommunityId!,
                    CommunityName= t.Communities.CommunityName,
                    CreatedAt = t.CreatedAt
                })
                .ToList();

            return View(topics);
        }

        //CREATE: GET
        public IActionResult Create()
        {
            return View();
        }

        //CREATE: POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(TopicViewModel model, int? communityId)
        {
            if (!ModelState.IsValid) return View(model);

            if (communityId == null || communityId == 0) return NotFound();

            var community = _db.Communities
                .Where(c => !c.IsDeleted)
                .FirstOrDefault(c => c.Id == communityId);

            if (community == null) return NotFound();

            var topic = new Topic
            {
                Id = model.Id,
                TopicName = model.TopicName,
                CommunityId = community.Id
            };

            _db.Topics.Add(topic);
            _db.SaveChanges();

            return RedirectToAction("Index", new {id = communityId});
        }

        //UPDATE: GET
        public IActionResult Edit(int? topicId, int? communityId)
        {
            if (topicId == null || topicId == 0) return NotFound();

            var topic = _db.Topics
                .Where(t => !t.IsDeleted)
                .FirstOrDefault(t => t.Id == topicId);

            var community = _db.Communities
                .Where(c => !c.IsDeleted)
                .FirstOrDefault(c => c.Id == communityId);

            if (topic == null || community == null) return NotFound();

            var model = new TopicViewModel
            {
                Id = topic.Id,
                TopicName = topic.TopicName,
                CommunityId = community.Id,
                CommunityName = community.CommunityName,
                CreatedAt = topic.CreatedAt
            };

            return View(model);
        }

        //UPDATE: POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(TopicViewModel model, int? communityId)
        {
            if (!ModelState.IsValid) return View(model);

            if (communityId == null || communityId == 0) return NotFound();

            var topic = _db.Topics
                .Where (t => !t.IsDeleted)
                .FirstOrDefault(t => t.Id == model.Id);

            if (topic == null) return NotFound();

            topic.TopicName = model.TopicName;
            topic.UpdatedAt = DateTime.Now;

            _db.SaveChanges();
            return RedirectToAction("Index", new {id = communityId});
        }

        //DELETE: GET
        public IActionResult Delete(int? topicId, int? communityId)
        {
            if (topicId == null || topicId == 0) return NotFound();

            var topic = _db.Topics
                .Where(t => !t.IsDeleted)
                .FirstOrDefault(t => t.Id == topicId);

            var community = _db.Communities
                .Where(c => !c.IsDeleted)
                .FirstOrDefault(c => c.Id == communityId);

            if (topic == null || community == null) return NotFound();

            var model = new TopicViewModel
            {
                Id = topic.Id,
                TopicName = topic.TopicName,
                CommunityId = community.Id,
                CommunityName = community.CommunityName,
                CreatedAt = topic.CreatedAt
            };

            return View(model);
        }

        //DELETE: POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(TopicViewModel model, int? communityId)
        {
            if (!ModelState.IsValid) return View(model);

            if (communityId == null || communityId == 0) return NotFound();

            var topic = _db.Topics
                .Where(t => !t.IsDeleted)
                .FirstOrDefault(t => t.Id == model.Id);

            if (topic == null) return NotFound();

            topic.IsDeleted = true;
            topic.DeletedAt = DateTime.Now;
            topic.UpdatedAt = DateTime.Now;

            _db.SaveChanges();
            return RedirectToAction("Index", new { id = communityId });
        }
    }
}
