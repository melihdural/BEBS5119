// Assets/Scripts/Application/ListMemoriesUseCase.cs

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Models;
using Domain.Repositories;

namespace Application
{
    /// <summary>
    /// Anıları listeleme ve filtreleme use case
    /// </summary>
    public class ListMemoriesUseCase
    {
        private readonly IMemoryRepository _memoryRepository;

        public ListMemoriesUseCase(IMemoryRepository memoryRepository)
        {
            _memoryRepository = memoryRepository;
        }

        /// <summary>
        /// Tüm anıları getir, en yeni önce sıralı
        /// </summary>
        public async Task<List<Memory>> ExecuteAsync()
        {
            var memories = await _memoryRepository.GetAllAsync();
            return memories.OrderByDescending(m => m.createdTimestamp).ToList();
        }

        /// <summary>
        /// Belirli bir anchor'a bağlı anıları getir
        /// </summary>
        public async Task<List<Memory>> GetByAnchorAsync(string anchorId)
        {
            return await _memoryRepository.GetByAnchorIdAsync(anchorId);
        }
    }
}