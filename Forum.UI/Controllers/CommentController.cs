using Forum.DAL.DTOs;
using Forum.DAL.Repositories;
using Forum.UI.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace Forum.UI.Controllers
{
    public class CommentController : Controller
    {
        private readonly CommentRepository _commentRepository;
        private readonly PostRepository _postRepository;
        private readonly IStringLocalizer<CommentViewModel> _commentLocalizer;

        public CommentController(CommentRepository commentRepository, PostRepository postRepository, IStringLocalizer<CommentViewModel> commentLocalizer)
        {
            _commentLocalizer = commentLocalizer;
            _postRepository = postRepository;
            _commentRepository = commentRepository;
        }

        //CREATE: GET
        public IActionResult Create(int postId)
        {
            var post = _postRepository.GetPost(postId);
            if (post == null) return NotFound();

            var model = new CommentViewModel(_commentLocalizer)
            {
                PostId = post.Id
            };

            return View(model);
        }

        //CREATE: POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(CommentViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var viewModel = new CommentViewModel(_commentLocalizer)
                {
                    CommentText = model.CommentText,
                    PostId = model.PostId
                };

                return View(viewModel);
            }

            var dto = new CommentDTO
            {
                CommentText = model.CommentText,
                PostId = model.PostId
            };

            _commentRepository.CreateComment(dto);
            TempData["Success"] = "Comment created successfully.";

            return RedirectToAction("Index", "Home");
        }

        //public IActionResult Edit(int id)
        //{

        //}
    }
}
