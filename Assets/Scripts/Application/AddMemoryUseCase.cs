// Assets/Scripts/Application/AddMemoryUseCase.cs

using System;
using System.Threading.Tasks;
using Domain.Models;
using Domain.Repositories;
using UnityEngine;

namespace Application
{
    /// <summary>
    /// Yeni anı ekleme use case. İş kurallarını ve validasyonları içerir.
    /// </summary>
    public class AddMemoryUseCase
    {
        private readonly IMemoryRepository _memoryRepository;

        public AddMemoryUseCase(IMemoryRepository memoryRepository)
        {
            _memoryRepository = memoryRepository;
        }

        /// <summary>
        /// Yeni anı ekle. Başlık kontrolü ve dosya boyutu validasyonu yapar.
        /// </summary>
        public async Task<Memory> ExecuteAsync(Memory memory, byte[] photoBytes = null, byte[] audioBytes = null)
        {
            try
            {
                // İş kuralı: Başlık zorunlu
                if (string.IsNullOrWhiteSpace(memory.title))
                {
                    Debug.LogError("[AddMemoryUseCase] Title is required");
                    return null;
                }

                // İş kuralı: Fotoğraf boyutu 2MB'ı geçemez
                if (photoBytes != null && photoBytes.Length > 2 * 1024 * 1024)
                {
                    Debug.LogError("[AddMemoryUseCase] Photo size exceeds 2MB limit");
                    return null;
                }

                // İş kuralı: Ses boyutu 1.5MB'ı geçemez
                if (audioBytes != null && audioBytes.Length > 1.5 * 1024 * 1024)
                {
                    Debug.LogError("[AddMemoryUseCase] Audio size exceeds 1.5MB limit");
                    return null;
                }

                // Medya dosyalarını kaydet
                if (photoBytes != null)
                {
                    memory.photoPath = await Infrastructure.Persistence.FileSystemHelper.SavePhotoAsync(photoBytes, memory.id);
                }

                if (audioBytes != null)
                {
                    memory.audioPath = await Infrastructure.Persistence.FileSystemHelper.SaveAudioAsync(audioBytes, memory.id);
                }

                // Repository'e kaydet
                var result = await _memoryRepository.AddAsync(memory);
                Debug.Log($"[AddMemoryUseCase] Memory added successfully: {result.id}");
                return result;
            }
            catch (Exception ex)
            {
                Debug.LogError($"[AddMemoryUseCase] ExecuteAsync failed: {ex.Message}");
                return null;
            }
        }
    }
}