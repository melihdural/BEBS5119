// Assets/Scripts/Application/DeleteMemoryUseCase.cs

using System;
using System.Threading.Tasks;
using Domain.Repositories;
using UnityEngine;

namespace Application
{
    /// <summary>
    /// Anı silme use case. İlişkili medya dosyalarını da siler.
    /// </summary>
    public class DeleteMemoryUseCase
    {
        private readonly IMemoryRepository _memoryRepository;

        public DeleteMemoryUseCase(IMemoryRepository memoryRepository)
        {
            _memoryRepository = memoryRepository;
        }

        public async Task<bool> ExecuteAsync(string memoryId)
        {
            try
            {
                // Repository içinde medya dosyaları da silinir
                var result = await _memoryRepository.DeleteAsync(memoryId);
                
                if (result)
                {
                    Debug.Log($"[DeleteMemoryUseCase] Memory deleted: {memoryId}");
                }
                else
                {
                    Debug.LogWarning($"[DeleteMemoryUseCase] Memory not found: {memoryId}");
                }

                return result;
            }
            catch (Exception ex)
            {
                Debug.LogError($"[DeleteMemoryUseCase] ExecuteAsync failed: {ex.Message}");
                return false;
            }
        }
    }
}