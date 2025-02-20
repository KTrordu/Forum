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

        public Comment? GetComment(int id)
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

        public List<Comment>? GetComments(int postId)
        {
            return _db.Comments
                .Where(c => !c.IsDeleted && c.PostId == postId)
                .ToList();
        }

        public Dictionary<int, Comment>? GetComments(List<int> postIds)
        {
            return _db.Comments
                .Where(c => !c.IsDeleted && postIds.Contains(c.PostId!.Value))
                .ToDictionary(c => c.PostId!.Value, c => c);
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

        public void UpdateComment(CommentDTO dto)
        {
            var comment = GetComment(dto.Id);

            comment!.CommentText = dto.CommentText;
            comment.UpdatedAt = DateTime.Now;

            _db.Comments.Update(comment);
            _db.SaveChanges();
        }

        public void DeleteComment(int id)
        {
            var comment = GetComment(id);

            comment!.IsDeleted = true;
            comment.DeletedAt = DateTime.Now;
            comment.UpdatedAt= DateTime.Now;

            _db.Comments.Update(comment);
            _db.SaveChanges();
        }
    }
}
