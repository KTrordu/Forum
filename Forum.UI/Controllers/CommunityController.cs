using Forum.DAL;
using Forum.DAL.Repositories;
using Forum.UI.DTOs;
using Forum.UI.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Localization;
using System.Linq;

namespace Forum.UI.Controllers
{
    public class CommunityController : Controller
    {
        private readonly PostRepository _postRepository;
        private readonly TopicRepository _topicRepository;
        private readonly CommunityRepository _communityRepository;
        private readonly HelperRepository _helperRepository;
        private readonly IStringLocalizer<CommunityViewModel> _localizer;

        public CommunityController(PostRepository postRepository, TopicRepository topicRepository, CommunityRepository communityRepository, HelperRepository helperRepository, IStringLocalizer<CommunityViewModel> localizer)
        {
            _postRepository = postRepository;
            _topicRepository = topicRepository;
            _communityRepository = communityRepository;
            _helperRepository = helperRepository;
            _localizer = localizer;
        }

        //READ: List the communities
        public IActionResult Index()
        {
            return View();
        }

        //Subscribe or Unsubscribe to Community
        public IActionResult SubscribeToCommunity(int communityId)
        {
            var community = _communityRepository.GetCommunity(communityId);
            if (community == null) return NotFound();

            _communityRepository.SubscribeCommunity(community);

            return Json(new { isSubscribed = community.IsSubscribed });
        }

        //Get communities list
        public IActionResult GetCommunities()
        {
            var communities = _communityRepository.GetCommunities();
            if (communities == null) return NotFound();

            var communitiesList = communities
                .Select(c => new CommunityDTO
                {
                    Id = c.Id,
                    CommunityName = c.CommunityName,
                    CreatedAt = $"{c.CreatedAt.Day}.{c.CreatedAt.Month}.{c.CreatedAt.Year} {c.CreatedAt.DayOfWeek}",
                    IsSubscribed = c.IsSubscribed
                })
                .ToList();

            return Json(new { data = communitiesList });
        }

        //Get other communities list
        public IActionResult GetOtherCommunities(int communityId)
        {
            var communities = _communityRepository.GetSubscribedCommunities();
            if (communities == null) return NotFound();

            var currentCommunity = communities
                .FirstOrDefault(c => c.Id == communityId);
            if (currentCommunity == null) return NotFound();

            communities.Remove(currentCommunity);

            var communitiesList = communities
                .Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.CommunityName
                })
                .ToList();

            return Json(communitiesList);
        }

        //Change the community of topics
        public IActionResult ChangeTopicsCommunity(int oldCommunityId, int newCommunityId)
        {
            var oldCommunity = _communityRepository.GetCommunity(oldCommunityId);
            var newCommunity = _communityRepository.GetCommunity(newCommunityId);
            if (oldCommunity == null || newCommunity == null) return NotFound();

            var topics = _topicRepository.GetTopicsByCommunity(oldCommunity.Id);
            if (topics == null) return NotFound();

            foreach (var topic in topics)
            {
                _topicRepository.UpdateTopic(topic.Id, newCommunityId, topic.TopicName);
            }

            return Json(new { success = true, message = "Community of the topics and posts are updated successfully." });
        }

        //CREATE: GET
        public IActionResult Create()
        {
            var viewModel = new CommunityViewModel(_localizer);
            return View(viewModel);
        }

        //CREATE: POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(CommunityViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var viewModel = new CommunityViewModel(_localizer)
                {
                    CommunityName = model.CommunityName
                };

                return View(viewModel);
            }
            _communityRepository.CreateCommunity(model.CommunityName);
            TempData["Success"] = "Community created successfully.";

            return RedirectToAction("Index");
        }

        //UPDATE: GET
        public IActionResult Edit(int communityId)
        {
            var community = _communityRepository.GetCommunity(communityId);
            if (community == null) return NotFound();

            var model = new CommunityViewModel(_localizer)
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
            if (!ModelState.IsValid)
            {
                var viewModel = new CommunityViewModel(_localizer)
                {
                    CommunityName = model.CommunityName
                };

                return View(viewModel);
            }

            var community = _communityRepository.GetCommunity(model.Id);
            if (community == null) return NotFound();

            _communityRepository.UpdateCommunity(community.Id, model.CommunityName);
            TempData["Success"] = "Community updated successfully.";

            return RedirectToAction("Index");
        }

        //DELETE: GET
        public IActionResult Delete()
        {
            return PartialView("_DeleteModal");
        }

        //DELETE: POST
        [HttpPost, ActionName("DeleteCommunity")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteCommunity(int communityId)
        {
            var community = _communityRepository.GetCommunity(communityId);
            if (community == null) return NotFound();

            _communityRepository.DeleteCommunity(community.Id);
            return Json(new { success = true, message = "Community deleted successfully." });
        }

        //DELETE: Delete the community and its subentities
        [HttpPost,ActionName("DeleteCommunityCascading")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteCommunityCascading(int communityId)
        {
            var community = _communityRepository.GetCommunity(communityId);
            if (community == null) return NotFound();

            var topics = _topicRepository.GetTopicsByCommunity(community.Id);
            if (topics == null) return NotFound();

            var postsList = _postRepository.InitializePostList();
            foreach (var topic in topics)
            {
                var posts = _postRepository.GetPostsByTopic(topic.Id);
                if (posts == null) return NotFound();

                postsList.AddRange(posts);
            }

            var postContents = _postRepository.GetPostContents(postsList.Select(p => p.Id).ToList());
            if (postContents == null) return NotFound();

            try
            {
                _helperRepository.DeleteCascading(community, topics, postsList, postContents);
                return Json(new { success = true, message = "Community, its topics and their posts are deleted successfully." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex });
            }
        }
    }
}
