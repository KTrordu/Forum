using Forum.DAL;
using Forum.DAL.Repositories;
using Forum.UI.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace Forum.UI.Controllers
{
    public class CommunityController : Controller
    {
        private readonly PostRepository _postRepository;
        private readonly TopicRepository _topicRepository;
        private readonly CommunityRepository _communityRepository;

        public CommunityController(PostRepository postRepository, TopicRepository topicRepository, CommunityRepository communityRepository)
        {
            _postRepository = postRepository;
            _topicRepository = topicRepository;
            _communityRepository = communityRepository;
        }

        //READ: List the communities
        public IActionResult Index()
        {
            var communities = _communityRepository.GetCommunities();
            if (communities == null) return NotFound();

            var communitiesList = communities
                .Select(c => new CommunityViewModel
                {
                    Id = c.Id,
                    CommunityName = c.CommunityName,
                    CreatedAt = c.CreatedAt,
                    IsSubscribed = c.IsSubscribed
                })
                .ToList();

            return View(communitiesList);
        }

        //Subscribe or Unsubscribe to Community
        public IActionResult SubscribeToCommunity(int communityId)
        {
            var community = _communityRepository.GetCommunity(communityId);
            if (community == null) return NotFound();

            _communityRepository.SubscribeCommunity(community);

            return Json(new { isSubscribed = community.IsSubscribed });
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

            _communityRepository.CreateCommunity(model.CommunityName);
            TempData["Success"] = "Community created successfully.";

            return RedirectToAction("Index");
        }

        //UPDATE: GET
        public IActionResult Edit(int communityId)
        {
            var community = _communityRepository.GetCommunity(communityId);
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

            var community = _communityRepository.GetCommunity(model.Id);
            if (community == null) return NotFound();

            _communityRepository.UpdateCommunity(community.Id, model.CommunityName);
            TempData["Success"] = "Community updated successfully.";

            return RedirectToAction("Index");
        }

        //DELETE: GET
        public IActionResult Delete(int communityId)
        {
            var community = _communityRepository.GetCommunity(communityId);
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
        public IActionResult DeletePOST(int communityId)
        {
            var community = _communityRepository.GetCommunity(communityId);
            if (community == null) return NotFound();

            _communityRepository.DeleteCommunity(community.Id);
            TempData["Success"] = "Community deleted successfully.";

            return RedirectToAction("Index");
        }
    }
}
