﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forum.Entities
{
    public class Comment : BaseEntity
    {
        [StringLength(200)]
        public string CommentText { get; set; }

        public int? PostId { get; set; }
        public Post Posts { get; set; }

        public bool IsLiked { get; set; }
    }
}
