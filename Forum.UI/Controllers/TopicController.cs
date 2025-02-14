using Forum.DAL;
using Forum.DAL.Repositories;
using Forum.Entities;
using Forum.UI.DTOs;
using Forum.UI.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Localization;

namespace Forum.UI.Controllers
{
    public class TopicController : Controller
    {
        private readonly PostRepository _postRepository;
        private readonly TopicRepository _topicRepository;
        private readonly CommunityRepository _communityRepository;
        private readonly HelperRepository _helperRepository;
        private readonly IStringLocalizer<TopicViewModel> _topicLocalizer;
        private readonly IStringLocalizer<TopicListViewModel> _topicListLocalizer;

        public TopicController(PostRepository postRepository, TopicRepository topicRepository, CommunityRepository communityRepository, 
            HelperRepository helperRepository, IStringLocalizer<TopicViewModel> topicLocalizer, IStringLocalizer<TopicListViewModel> topicListLocalizer)
        {
            _postRepository = postRepository;
            _topicRepository = topicRepository;
            _communityRepository = communityRepository;
            _helperRepository = helperRepository;
            _topicLocalizer = topicLocalizer;
            _topicListLocalizer = topicListLocalizer;
        }

        //READ: List topics by the communityId
        public IActionResult Index(int communityId)
        {
            var community = _communityRepository.GetCommunity(communityId);
            if (community == null) return NotFound();

            var topics = _topicRepository.GetTopicsByCommunity(community.Id);
            if (topics == null) return NotFound();

            var model = new TopicListViewModel(_topicListLocalizer)
            {
                CommunityId = communityId,
                CommunityName = community.CommunityName,
                Topics = new List<TopicViewModel> { }
            };

            foreach (var topic in topics)
            {
                var topicViewModel = new TopicViewModel(_topicLocalizer)
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

        //Get topics as a list
        public IActionResult GetTopicsList(int topicId)
        {
            var topic = _topicRepository.GetTopic(topicId);
            if (topic == null) return NotFound();

            var community = _communityRepository.GetCommunity((int)topic.CommunityId!);
            if (community == null) return NotFound();

            var topics = _topicRepository.GetTopicsByCommunity(community.Id);
            if (topics == null) return NotFound();

            topics.Remove(topic);

            var topicsList = topics
                .Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.TopicName
                })
                .ToList();

            return Json(topicsList);
        }

        //Get topics by community
        public IActionResult GetTopics(int communityId)
        {
            var topics = _topicRepository.GetTopicsByCommunity(communityId);
            if (topics == null) return NotFound();

            var topicsList = topics
                .Select(t => new TopicDTO
                {
                    Id = t.Id,
                    TopicName = t.TopicName,
                    CreatedAt = $"{t.CreatedAt.Day}.{t.CreatedAt.Month}.{t.CreatedAt.Year} {t.CreatedAt.DayOfWeek}"
                })
                .ToList();

            return Json(new { data = topicsList });
        }

        //Change the topic of posts
        public IActionResult ChangePostsTopic(int oldTopicId, int  newTopicId)
        {
            var oldTopic = _topicRepository.GetTopic(oldTopicId);
            var newTopic = _topicRepository.GetTopic(newTopicId);
            if (oldTopic == null || newTopic == null) return NotFound();

            var posts = _postRepository.GetPostsByTopic(oldTopic.Id);
            if (posts == null) return NotFound();

            foreach (var post in posts)
            {
                _postRepository.UpdatePostTopic(post, newTopicId);
            }

            return Json(new { success = true, message = "Topic of the posts are updated successfully." });
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

            var model = new TopicViewModel(_topicLocalizer)
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
            if (!ModelState.IsValid)
            {
                var viewModel = new TopicViewModel(_topicLocalizer)
                {
                    Id = model.Id,
                    TopicName = model.TopicName,
                    CommunityId = model.CommunityId,
                    CommunityName = model.CommunityName,
                    ParentId = model.ParentId,
                    SubtopicsIds = new()
                };

                return View(viewModel);
            }
            
            var community = _communityRepository.GetCommunity(model.CommunityId);
            if (community == null) return NotFound();

            _topicRepository.CreateTopic(model.CommunityId, model.TopicName, model.ParentId);
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

            var model = new TopicViewModel(_topicLocalizer)
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
            if (!ModelState.IsValid)
            {
                var viewModel = new TopicViewModel(_topicLocalizer)
                {
                    Id = model.Id,
                    TopicName = model.TopicName,
                    CommunityId = model.CommunityId,
                    CommunityName = model.CommunityName
                };

                return View(viewModel);
            }

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

        //Delete the topic and its subentities
        [HttpPost,ActionName("DeleteTopicCascading")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteTopicCascading(int topicId)
        {
            var topic = _topicRepository.GetTopic(topicId);
            if (topic == null) return NotFound();

            var posts = _postRepository.GetPostsByTopic(topic.Id);
            if (posts == null) return NotFound();

            var postContents = _postRepository.GetPostContents(posts);
            if (postContents == null) return NotFound();

            try
            {
                _helperRepository.DeleteTopicCascading(topic, posts, postContents);
                return Json(new { success = true, message = "Topic and its posts are deleted successfully." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex });
            }
        }
    }
}
