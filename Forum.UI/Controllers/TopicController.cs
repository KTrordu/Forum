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

        //READ: List all topics
        public IActionResult Index(int communityId)
        {
            var community = _communityRepository.GetCommunity(communityId);

            var topics = _topicRepository.GetTopics();

            if (topics == null) return NotFound();

            var topicsList = topics
                .Select(t => new TopicViewModel
                {
                    Id = t.Id,
                    TopicName = t.TopicName,
                    CommunityId = (int)t.CommunityId!,
                    CommunityName = t.Communities.CommunityName,
                    CreatedAt = t.CreatedAt
                })
                .ToList();

            var model = new TopicListViewModel
            {
                CommunityId = communityId,
                CommunityName = community != null ? community.CommunityName : null,
                Topics = topicsList
            };

            return View(model);
        }

        //READ: List topics by the communityId
        [Route("Topic/Community/{communityId}")]
        public IActionResult IndexByCommunity(int communityId)
        {
            if (communityId == 0) return NotFound();

            var community = _communityRepository.GetCommunity(communityId);
            if (community == null) return NotFound();

            var topics = _topicRepository.GetTopicsByCommunity(community.Id);
            if (topics == null) return NotFound();

            var topicsList = topics
                .Select(t => new TopicViewModel
                {
                    Id= t.Id,
                    TopicName = t.TopicName,
                    CommunityId= (int)t.CommunityId!,
                    CommunityName= t.Communities.CommunityName,
                    CreatedAt = t.CreatedAt
                })
                .ToList();

            var model = new TopicListViewModel
            {
                CommunityId = community.Id,
                CommunityName = community.CommunityName,
                Topics = topicsList
            };

            return View("Index", model);
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

            var model = new TopicViewModel
            {
                Communities = communitiesList
            };

            return View(model);
        }

        //CREATE: POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(TopicViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var communities = _communityRepository.GetCommunities();
                if (communities == null) return NotFound();

                var communitiesList = communities
                    .Select(c => new SelectListItem
                    {
                        Value= c.Id.ToString(),
                        Text = c.CommunityName
                    })
                    .ToList();

                model.Communities = communitiesList;

                return View(model);
            }

            var community = _communityRepository.GetCommunity(model.CommunityId);
            if (community == null) return NotFound();

            _topicRepository.CreateTopic(model.CommunityId, model.TopicName);

            return RedirectToAction("IndexByCommunity", new { communityId = model.CommunityId });
        }

        //UPDATE: GET
        public IActionResult Edit(int topicId)
        {
            if (topicId == 0) return NotFound();

            var topic = _topicRepository.GetTopic(topicId);
            if (topic == null) return NotFound();

            var community = _communityRepository.GetCommunity((int)topic.CommunityId!);
            if (community == null) return NotFound();

            var communities = _communityRepository.GetSubscribedCommunities();
            if (communities == null) return NotFound();

            var communitiesList = communities
                .Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.CommunityName
                })
                .ToList();

            var model = new TopicViewModel
            {
                Id = topic.Id,
                TopicName = topic.TopicName,
                CommunityId = community.Id,
                CommunityName = community.CommunityName,
                Communities = communitiesList,
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

            return RedirectToAction("IndexByCommunity", new { communityId = model.CommunityId });
        }

        //DELETE: GET
        public IActionResult Delete(int topicId)
        {
            if (topicId == 0) return NotFound();

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

        //DELETE: POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(TopicViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var topic = _topicRepository.GetTopic(model.Id);
            if (topic == null) return NotFound();

            _topicRepository.DeleteTopic(topic.Id);

            return RedirectToAction("Index", new { id = topic.CommunityId });
        }
    }
}
