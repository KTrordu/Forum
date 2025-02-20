using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forum.DAL.DTOs
{
    public class CommentDTO
    {
        public int? Id { get; set; }

        public string CommentText { get; set; }

        public int PostId { get; set; }
    }
}
