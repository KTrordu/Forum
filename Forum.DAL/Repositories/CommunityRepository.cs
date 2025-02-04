using Forum.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forum.DAL.Repositories
{
    public class CommunityRepository
    {
        private readonly ApplicationDbContext _db;

        public CommunityRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public Community? GetCommunity(int id)
        {
            return _db.Communities
                .Where(c => !c.IsDeleted && c.Id == id)
                .FirstOrDefault();
        }

        public Community? GetSubscribedCommunity(int id)
        {
            return _db.Communities
                .Where(c => !c.IsDeleted && c.IsSubscribed && c.Id == id)
                .FirstOrDefault();
        }

        public List<Community>? GetCommunities()
        {
            return _db.Communities
                .Where(c => !c.IsDeleted)
                .OrderByDescending(c => c.UpdatedAt)
                .ToList();
        }

        public List<Community>? GetSubscribedCommunities()
        {
            return _db.Communities
                .Where(c => !c.IsDeleted && c.IsSubscribed)
                .OrderByDescending(c => c.UpdatedAt)
                .ToList();
        }

        public void CreateCommunity(Community community)
        {
            _db.Communities.Add(community);
            _db.SaveChanges();
        }

        public void UpdateCommunity(int communityId, Community newCommunity)
        {
            var community = GetCommunity(communityId);

            community!.CommunityName = newCommunity.CommunityName;
            community.UpdatedAt = newCommunity.UpdatedAt;

            _db.Communities.Update(community);
            _db.SaveChanges();
        }

        public void DeleteCommunity(int communityId)
        {
            var community = GetCommunity(communityId);

            community!.IsDeleted = true;
            community.DeletedAt = DateTime.Now;
            community.UpdatedAt = DateTime.Now;

            _db.Communities.Update(community);
            _db.SaveChanges();
        }
    }
}
