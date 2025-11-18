// Assets/Scripts/Domain/Repositories/IAnchorRepository.cs

using System.Collections.Generic;
using System.Threading.Tasks;
using Domain.Models;

namespace Domain.Repositories
{
    /// <summary>
    /// AR anchor repository arayüzü
    /// </summary>
    public interface IAnchorRepository
    {
        Task<AnchorRef> AddAsync(AnchorRef anchorRef);
        Task<AnchorRef> UpdateAsync(AnchorRef anchorRef);
        Task<bool> DeleteAsync(string anchorId);
        Task<AnchorRef> GetByIdAsync(string anchorId);
        Task<List<AnchorRef>> GetAllAsync();
    }
}