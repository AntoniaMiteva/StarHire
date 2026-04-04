using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarHire.Business.Services.Interfaces
{
    public interface IFavoriteService
    {
        Task AddAsync(Guid jobId, Guid userId);
        Task RemoveAsync(Guid jobId, Guid userId);
        Task<bool> IsFavoriteAsync(Guid jobId, Guid userId);
        Task<List<Guid>> GetFavoriteJobIdsAsync(Guid userId);
    }
}
