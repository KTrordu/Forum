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

        //UPDATE: GET
        public IActionResult Edit(int commentId)
        {
            var comment = _commentRepository.GetComment(commentId);
            if (comment == null) return NotFound();

            var model = new CommentViewModel(_commentLocalizer)
            {
                Id = comment.Id,
                CommentText = comment.CommentText,
                PostId = (int)comment.PostId!
            };

            return View(model);
        }

        //UPDATE: POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(CommentViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var viewModel = new CommentViewModel(_commentLocalizer)
                {
                    Id= model.Id,
                    CommentText = model.CommentText,
                    PostId = model.PostId
                };

                return View(viewModel);
            }

            var comment = _commentRepository.GetComment(model.Id);
            if (comment == null) return NotFound();

            var dto = new CommentDTO
            {
                Id = comment.Id,
                CommentText = model.CommentText,
                PostId = model.PostId
            };

            _commentRepository.UpdateComment(dto);
            TempData["Success"] = "Comment updated successfully.";

            return RedirectToAction("Index", "Home");
        }

        //DELETE: GET
        public IActionResult Delete()
        {
            return PartialView("_DeleteCommentModal");
        }

        //DELETE: POST
        [HttpPost, ActionName("DeleteComment")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteComment(int commentId)
        {
            var comment = _commentRepository.GetComment(commentId);
            if (comment == null) return NotFound();

            _commentRepository.DeleteComment(comment.Id);
            return Json(new { success = true, message = "Comment deleted successfully." });
        }
    }
}
