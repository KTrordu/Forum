using Forum.DAL.DTOs;
using Forum.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forum.DAL.Repositories
{
    public class CommentRepository
    {
        private readonly ApplicationDbContext _db;

        public CommentRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public Comment? GetCommunity(int id)
        {
            return _db.Comments
                .Where(c => !c.IsDeleted &&  c.Id == id)
                .FirstOrDefault();
        }

        public List<Comment>? GetComments()
        {
            return _db.Comments
                .Where(c => !c.IsDeleted)
                .ToList();
        }

        public List<Comment>? GetComments(List<int> postIds)
        {
            var commentsList = new List<Comment>();

            foreach (var postId in postIds)
            {
                var comments = _db.Comments
                    .Where(c => !c.IsDeleted && c.PostId == postId)
                    .ToList();

                commentsList.AddRange(comments);
            }

            return commentsList;
        }

        public void CreateComment(CommentDTO dto)
        {
            var comment = new Comment
            {
                CommentText = dto.CommentText,
                PostId = dto.PostId
            };

            _db.Comments.Add(comment);
            _db.SaveChanges();
        }
    }
}
