// Assets/Scripts/Application/UpdateMemoryUseCase.cs

using System;
using System.Threading.Tasks;
using Domain.Models;
using Domain.Repositories;
using UnityEngine;

namespace Application
{
    /// <summary>
    /// Mevcut anıyı güncelleme use case
    /// </summary>
    public class UpdateMemoryUseCase
    {
        private readonly IMemoryRepository _memoryRepository;

        public UpdateMemoryUseCase(IMemoryRepository memoryRepository)
        {
            _memoryRepository = memoryRepository;
        }

        public async Task<Memory> ExecuteAsync(Memory memory, byte[] newPhotoBytes = null, byte[] newAudioBytes = null)
        {
            try
            {
                if (!memory.IsValid())
                {
                    Debug.LogError("[UpdateMemoryUseCase] Invalid memory data");
                    return null;
                }

                // Yeni medya varsa kaydet
                if (newPhotoBytes != null && newPhotoBytes.Length > 0)
                {
                    memory.photoPath = await Infrastructure.Persistence.FileSystemHelper.SavePhotoAsync(newPhotoBytes, memory.id);
                }

                if (newAudioBytes != null && newAudioBytes.Length > 0)
                {
                    memory.audioPath = await Infrastructure.Persistence.FileSystemHelper.SaveAudioAsync(newAudioBytes, memory.id);
                }

                var result = await _memoryRepository.UpdateAsync(memory);
                Debug.Log($"[UpdateMemoryUseCase] Memory updated: {result?.id}");
                return result;
            }
            catch (Exception ex)
            {
                Debug.LogError($"[UpdateMemoryUseCase] ExecuteAsync failed: {ex.Message}");
                return null;
            }
        }
    }
}