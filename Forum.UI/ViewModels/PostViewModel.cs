﻿using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Forum.UI.ViewModels
{
    public class PostViewModel
    {
        [Required]
        public int Id { get; set; }

        [DisplayName("Post Title")]
        public string PostTitle { get; set; }

        public string PostDescription { get; set; }

        public string? ImagePath { get; set; }

        public bool IsLiked { get; set; } = false;

        public int CommunityId { get; set; }

        [DisplayName("Community Name")]
        public string CommunityName { get; set; }

        public int TopicId { get; set; }

        public string TopicName { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
