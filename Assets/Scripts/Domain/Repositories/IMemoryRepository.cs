// Assets/Scripts/Domain/Repositories/IMemoryRepository.cs

using System.Collections.Generic;
using System.Threading.Tasks;
using Domain.Models;

namespace Domain.Repositories
{
    /// <summary>
    /// Anı repository arayüzü. Veri erişim katmanını soyutlar.
    /// </summary>
    public interface IMemoryRepository
    {
        /// <summary>
        /// Yeni anı ekle
        /// </summary>
        Task<Memory> AddAsync(Memory memory);

        /// <summary>
        /// Anı güncelle
        /// </summary>
        Task<Memory> UpdateAsync(Memory memory);

        /// <summary>
        /// Anı sil
        /// </summary>
        Task<bool> DeleteAsync(string memoryId);

        /// <summary>
        /// ID ile anı getir
        /// </summary>
        Task<Memory> GetByIdAsync(string memoryId);

        /// <summary>
        /// Tüm anıları listele
        /// </summary>
        Task<List<Memory>> GetAllAsync();

        /// <summary>
        /// Anchor ID ile ilişkili anıları getir
        /// </summary>
        Task<List<Memory>> GetByAnchorIdAsync(string anchorId);
    }
}