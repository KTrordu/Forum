﻿using Microsoft.Extensions.Localization;
using System.ComponentModel.DataAnnotations;

namespace Forum.UI.ViewModels
{
    public class CommentViewModel
    {
        private readonly IStringLocalizer<CommentViewModel> _localizer;

        public CommentViewModel(IStringLocalizer<CommentViewModel> localizer)
        {
            _localizer = localizer;
        }

        public CommentViewModel()
        {

        }

        public string? CommentTextLabel => _localizer?["CommentTextLabel"];

        [Required]
        public int Id { get; set; }

        public string CommentText { get; set; }

        public int PostId { get; set; }

        public bool IsLiked { get; set; } = false;

        public DateTime CreatedAt { get; set; }
    }
}
