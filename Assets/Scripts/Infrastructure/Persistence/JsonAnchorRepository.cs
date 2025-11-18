// Assets/Scripts/Infrastructure/Persistence/JsonAnchorRepository.cs

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
    /// JSON dosya tabanlı Anchor repository implementasyonu
    /// </summary>
    public class JsonAnchorRepository : IAnchorRepository
    {
        private readonly string _filePath;
        private AnchorCollection _cache;

        public JsonAnchorRepository()
        {
            _filePath = Path.Combine(UnityEngine.Application.persistentDataPath, "anchors.json");
            _cache = new AnchorCollection();
            LoadFromDisk();
        }

        public async Task<AnchorRef> AddAsync(AnchorRef anchorRef)
        {
            try
            {
                _cache.anchors.Add(anchorRef);
                await SaveToDiskAsync();
                return anchorRef;
            }
            catch (Exception ex)
            {
                Debug.LogError($"[JsonAnchorRepository] AddAsync failed: {ex.Message}");
                return null;
            }
        }

        public async Task<AnchorRef> UpdateAsync(AnchorRef anchorRef)
        {
            try
            {
                var existing = _cache.anchors.FirstOrDefault(a => a.anchorId == anchorRef.anchorId);
                if (existing != null)
                {
                    _cache.anchors.Remove(existing);
                }
                _cache.anchors.Add(anchorRef);
                await SaveToDiskAsync();
                return anchorRef;
            }
            catch (Exception ex)
            {
                Debug.LogError($"[JsonAnchorRepository] UpdateAsync failed: {ex.Message}");
                return null;
            }
        }

        public async Task<bool> DeleteAsync(string anchorId)
        {
            try
            {
                var anchor = _cache.anchors.FirstOrDefault(a => a.anchorId == anchorId);
                if (anchor == null) return false;

                _cache.anchors.Remove(anchor);
                await SaveToDiskAsync();
                return true;
            }
            catch (Exception ex)
            {
                Debug.LogError($"[JsonAnchorRepository] DeleteAsync failed: {ex.Message}");
                return false;
            }
        }

        public Task<AnchorRef> GetByIdAsync(string anchorId)
        {
            var anchor = _cache.anchors.FirstOrDefault(a => a.anchorId == anchorId);
            return Task.FromResult(anchor);
        }

        public Task<List<AnchorRef>> GetAllAsync()
        {
            return Task.FromResult(new List<AnchorRef>(_cache.anchors));
        }

        private void LoadFromDisk()
        {
            if (!File.Exists(_filePath))
            {
                _cache = new AnchorCollection();
                return;
            }

            try
            {
                string json = File.ReadAllText(_filePath);
                _cache = JsonUtility.FromJson<AnchorCollection>(json) ?? new AnchorCollection();
            }
            catch (Exception ex)
            {
                Debug.LogError($"[JsonAnchorRepository] LoadFromDisk failed: {ex.Message}");
                _cache = new AnchorCollection();
            }
        }

        private async Task SaveToDiskAsync()
        {
            try
            {
                string json = JsonUtility.ToJson(_cache, true);
                await File.WriteAllTextAsync(_filePath, json);
            }
            catch (Exception ex)
            {
                Debug.LogError($"[JsonAnchorRepository] SaveToDiskAsync failed: {ex.Message}");
            }
        }

        [Serializable]
        private class AnchorCollection
        {
            public List<AnchorRef> anchors = new List<AnchorRef>();
        }
    }
}