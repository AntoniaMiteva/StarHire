using StarHire.Business.Services.Interfaces;
using StarHire.Data;
using StarHire.Models.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace StarHire.Business.Services.Implementation
{
    public class FavoriteService : IFavoriteService
    {
        private readonly ApplicationDbContext _db;

        public FavoriteService(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task AddAsync(Guid jobId, Guid userId)
        {
            var exists = await _db.Favorites.AnyAsync(f => f.JobId == jobId && f.UserId == userId);
            if (!exists)
            {
                _db.Favorites.Add(new Favorite { Id = Guid.NewGuid(), JobId = jobId, UserId = userId });
                await _db.SaveChangesAsync();
            }
        }

        public async Task RemoveAsync(Guid jobId, Guid userId)
        {
            var fav = await _db.Favorites.FirstOrDefaultAsync(f => f.JobId == jobId && f.UserId == userId);
            if (fav != null)
            {
                _db.Favorites.Remove(fav);
                await _db.SaveChangesAsync();
            }
        }

        public async Task<bool> IsFavoriteAsync(Guid jobId, Guid userId)
        {
            return await _db.Favorites.AnyAsync(f => f.JobId == jobId && f.UserId == userId);
        }

        public async Task<List<Guid>> GetFavoriteJobIdsAsync(Guid userId)
        {
            return await _db.Favorites
                .Where(f => f.UserId == userId)
                .Select(f => f.JobId)
                .ToListAsync();
        }
    }
}
