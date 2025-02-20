using Forum.DAL.DTOs;
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

        public void CreateCommunity(CommunityDTO dto)
        {
            var community = new Community
            {
                CommunityName = dto.CommunityName
            };

            _db.Communities.Add(community);
            _db.SaveChanges();
        }

        public void UpdateCommunity(CommunityDTO dto)
        {
            var community = GetCommunity(dto.Id);

            community!.CommunityName = dto.CommunityName;
            community.UpdatedAt = DateTime.Now;

            _db.Communities.Update(community);
            _db.SaveChanges();
        }

        public void SubscribeCommunity(Community community)
        {
            community.IsSubscribed = !community.IsSubscribed;
            community.UpdatedAt = DateTime.Now;
            _db.Update(community);

            _db.SaveChanges();
        }

        public void DeleteCommunity(CommunityDTO dto)
        {
            var community = GetCommunity(dto.Id);

            community!.IsDeleted = true;
            community.DeletedAt = DateTime.Now;
            community.UpdatedAt = DateTime.Now;

            _db.Communities.Update(community);
            _db.SaveChanges();
        }
    }
}
