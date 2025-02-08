using Forum.DAL;
using Forum.DAL.Repositories;
using Forum.UI.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Forum.UI.Controllers
{
    public class TopicController : Controller
    {
        private readonly PostRepository _postRepository;
        private readonly TopicRepository _topicRepository;
        private readonly CommunityRepository _communityRepository;

        public TopicController(PostRepository postRepository, TopicRepository topicRepository, CommunityRepository communityRepository)
        {
            _postRepository = postRepository;
            _topicRepository = topicRepository;
            _communityRepository = communityRepository;
        }

        //READ: List topics by the communityId
        public IActionResult Index(int communityId)
        {
            var community = _communityRepository.GetCommunity(communityId);
            if (community == null) return NotFound();

            var topics = _topicRepository.GetTopicsByCommunity(community.Id);
            if (topics == null) return NotFound();

            var model = new TopicListViewModel
            {
                CommunityId = communityId,
                CommunityName = community.CommunityName,
                Topics = new List<TopicViewModel> { }
            };

            foreach (var topic in topics)
            {
                var topicViewModel = new TopicViewModel
                {
                    Id = topic.Id,
                    TopicName = topic.TopicName,
                    CommunityId = community.Id,
                    CommunityName = community.CommunityName,
                    CreatedAt = topic.CreatedAt
                };

                model.Topics.Add(topicViewModel);
            }

            return View("Index", model);
        }

        //CREATE: GET
        public IActionResult Create(int communityId)
        {
            var community = _communityRepository.GetCommunity(communityId);
            if (community == null) return NotFound();

            var communitiesList = new SelectListItem
            {
                Value = community.Id.ToString(),
                Text = community.CommunityName
            };

            var model = new TopicViewModel
            {
                CommunityId = community.Id,
                CommunityName = community.CommunityName
            };

            return View(model);
        }

        //CREATE: POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(TopicViewModel model)
        {
            if (!ModelState.IsValid) return View(model);
            
            var community = _communityRepository.GetCommunity(model.CommunityId);
            if (community == null) return NotFound();

            _topicRepository.CreateTopic(model.CommunityId, model.TopicName);
            TempData["Success"] = "Topic created successfully.";

            return RedirectToAction("Index", new { communityId = model.CommunityId });
        }

        //UPDATE: GET
        public IActionResult Edit(int topicId)
        {
            var topic = _topicRepository.GetTopic(topicId);
            if (topic == null) return NotFound();

            var community = _communityRepository.GetCommunity((int)topic.CommunityId!);
            if (community == null) return NotFound();

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
        public IActionResult Edit(TopicViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var topic = _topicRepository.GetTopic(model.Id);
            if (topic == null) return NotFound();

            _topicRepository.UpdateTopic(topic.Id, model.CommunityId, model.TopicName);
            TempData["Success"] = "Topic updated successfully.";

            return RedirectToAction("Index", new { communityId = model.CommunityId });
        }

        //DELETE: GET
        public IActionResult Delete()
        {
            return PartialView("_DeleteModal");
        }

        //DELETE: POST
        [HttpPost, ActionName("DeleteTopic")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteTopic(int topicId)
        {
            var topic = _topicRepository.GetTopic(topicId);
            if (topic == null) return NotFound();

            _topicRepository.DeleteTopic(topicId);
            return Json(new { success = true, message = "Topic deleted successfully." });
        }
    }
}
