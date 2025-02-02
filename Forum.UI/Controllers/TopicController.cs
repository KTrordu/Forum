using Forum.DAL;
using Forum.Entities;
using Forum.UI.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

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
        [Route("Topic/Community/{communityId}")]
        public IActionResult IndexByCommunity(int? communityId)
        {
            if (communityId == null || communityId == 0) return NotFound();

            var community = _db.Communities
                .Where(t => !t.IsDeleted && t.Id == communityId)
                .FirstOrDefault(t => t.Id == communityId);

            if (community == null) return NotFound();

            var topics = _db.Topics
                .Where(t => t.CommunityId == communityId && !t.IsDeleted)
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

            return View("Index", topics);
        }

        //CREATE: GET
        public IActionResult Create(int? communityId)
        {
            var communities = _db.Communities
                .Where(c => !c.IsDeleted && c.IsSubscribed)
                .Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.CommunityName
                })
                .ToList();

            if (communities == null) return NotFound();

            var model = new TopicViewModel
            {
                Communities = communities
            };

            return View(model);
        }

        //CREATE: POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(TopicViewModel model, int? communityId)
        {
            if (!ModelState.IsValid) return View(model);

            //if (communityId == null || communityId == 0) return NotFound();

            var community = _db.Communities
                .Where(c => !c.IsDeleted)
                .FirstOrDefault(c => c.Id == model.CommunityId);

            if (community == null) return NotFound();

            var topic = new Topic
            {
                Id = model.Id,
                TopicName = model.TopicName,
                CommunityId = model.CommunityId
            };

            _db.Topics.Add(topic);
            _db.SaveChanges();

            return RedirectToAction("IndexByCommunity", new { communityId = model.CommunityId });
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

            var communities = _db.Communities
                .Where (c => !c.IsDeleted && c.IsSubscribed)
                .Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.CommunityName
                })
                .ToList();


            if (topic == null || community == null || communities == null) return NotFound();

            var model = new TopicViewModel
            {
                Id = topic.Id,
                TopicName = topic.TopicName,
                CommunityId = community.Id,
                CommunityName = community.CommunityName,
                Communities = communities,
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

            topic.CommunityId = model.CommunityId;
            topic.TopicName = model.TopicName;
            topic.UpdatedAt = DateTime.Now;

            _db.SaveChanges();
            return RedirectToAction("IndexByCommunity", new { communityId = model.CommunityId });
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
