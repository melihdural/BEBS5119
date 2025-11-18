// Assets/Scripts/Infrastructure/Persistence/JsonMemoryRepository.cs

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Domain.Models;
using Domain.Repositories;
using UnityEngine;

namespace Infrastructure.Persistence
{
    /// <summary>
    /// JSON dosya tabanlı Memory repository implementasyonu.
    /// Tüm anıları tek bir JSON dizisi olarak saklar.
    /// </summary>
    public class JsonMemoryRepository : IMemoryRepository
    {
        private readonly string _filePath;
        private MemoryCollection _cache;

        public JsonMemoryRepository()
        {
            _filePath = Path.Combine(UnityEngine.Application.persistentDataPath, "memories.json");
            _cache = new MemoryCollection();
            LoadFromDisk();
        }

        public async Task<Memory> AddAsync(Memory memory)
        {
            try
            {
                if (!memory.IsValid())
                {
                    Debug.LogError("[JsonMemoryRepository] Invalid memory: Title is required");
                    return null;
                }

                _cache.memories.Add(memory);
                await SaveToDiskAsync();
                Debug.Log($"[JsonMemoryRepository] Added memory: {memory.id}");
                return memory;
            }
            catch (Exception ex)
            {
                Debug.LogError($"[JsonMemoryRepository] AddAsync failed: {ex.Message}");
                return null;
            }
        }

        public async Task<Memory> UpdateAsync(Memory memory)
        {
            try
            {
                var existing = _cache.memories.FirstOrDefault(m => m.id == memory.id);
                if (existing == null)
                {
                    Debug.LogWarning($"[JsonMemoryRepository] Memory not found: {memory.id}");
                    return null;
                }

                memory.modifiedTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
                _cache.memories.Remove(existing);
                _cache.memories.Add(memory);
                await SaveToDiskAsync();
                Debug.Log($"[JsonMemoryRepository] Updated memory: {memory.id}");
                return memory;
            }
            catch (Exception ex)
            {
                Debug.LogError($"[JsonMemoryRepository] UpdateAsync failed: {ex.Message}");
                return null;
            }
        }

        public async Task<bool> DeleteAsync(string memoryId)
        {
            try
            {
                var memory = _cache.memories.FirstOrDefault(m => m.id == memoryId);
                if (memory == null)
                {
                    Debug.LogWarning($"[JsonMemoryRepository] Memory not found for deletion: {memoryId}");
                    return false;
                }

                // İlişkili medya dosyalarını sil
                DeleteMediaFiles(memory);

                _cache.memories.Remove(memory);
                await SaveToDiskAsync();
                Debug.Log($"[JsonMemoryRepository] Deleted memory: {memoryId}");
                return true;
            }
            catch (Exception ex)
            {
                Debug.LogError($"[JsonMemoryRepository] DeleteAsync failed: {ex.Message}");
                return false;
            }
        }

        public Task<Memory> GetByIdAsync(string memoryId)
        {
            var memory = _cache.memories.FirstOrDefault(m => m.id == memoryId);
            return Task.FromResult(memory);
        }

        public Task<List<Memory>> GetAllAsync()
        {
            return Task.FromResult(new List<Memory>(_cache.memories));
        }

        public Task<List<Memory>> GetByAnchorIdAsync(string anchorId)
        {
            var filtered = _cache.memories.Where(m => m.anchorId == anchorId).ToList();
            return Task.FromResult(filtered);
        }

        private void LoadFromDisk()
        {
            if (!File.Exists(_filePath))
            {
                _cache = new MemoryCollection();
                Debug.Log("[JsonMemoryRepository] No existing data file, starting fresh");
                return;
            }

            try
            {
                string json = File.ReadAllText(_filePath);
                _cache = JsonUtility.FromJson<MemoryCollection>(json) ?? new MemoryCollection();
                Debug.Log($"[JsonMemoryRepository] Loaded {_cache.memories.Count} memories from disk");
            }
            catch (Exception ex)
            {
                Debug.LogError($"[JsonMemoryRepository] LoadFromDisk failed: {ex.Message}");
                _cache = new MemoryCollection();
            }
        }

        private async Task SaveToDiskAsync()
        {
            try
            {
                string json = JsonUtility.ToJson(_cache, true);
                await File.WriteAllTextAsync(_filePath, json);
                Debug.Log("[JsonMemoryRepository] Saved to disk");
            }
            catch (Exception ex)
            {
                Debug.LogError($"[JsonMemoryRepository] SaveToDiskAsync failed: {ex.Message}");
            }
        }

        private void DeleteMediaFiles(Memory memory)
        {
            if (!string.IsNullOrEmpty(memory.photoPath))
            {
                string fullPath = Path.Combine(UnityEngine.Application.persistentDataPath, memory.photoPath);
                if (File.Exists(fullPath))
                {
                    File.Delete(fullPath);
                }
            }

            if (!string.IsNullOrEmpty(memory.audioPath))
            {
                string fullPath = Path.Combine(UnityEngine.Application.persistentDataPath, memory.audioPath);
                if (File.Exists(fullPath))
                {
                    File.Delete(fullPath);
                }
            }
        }

        [Serializable]
        private class MemoryCollection
        {
            public List<Memory> memories = new List<Memory>();
        }
    }
}