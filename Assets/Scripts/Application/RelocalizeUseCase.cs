// Assets/Scripts/Application/RelocalizeUseCase.cs

using System.Threading.Tasks;
using Domain.Repositories;
using UnityEngine;

namespace Application
{
    /// <summary>
    /// Relocalization (anıları yeniden bulma) use case.
    /// iOS: ARWorldMap yükle
    /// Android: Kullanıcıya yeniden tarama yaptır
    /// </summary>
    public class RelocalizeUseCase
    {
        private readonly IAnchorRepository _anchorRepository;

        public RelocalizeUseCase(IAnchorRepository anchorRepository)
        {
            _anchorRepository = anchorRepository;
        }

        /// <summary>
        /// Kaydedilmiş anchor'ları getir
        /// </summary>
        public async Task<bool> ExecuteAsync()
        {
            var anchors = await _anchorRepository.GetAllAsync();
            
            if (anchors == null || anchors.Count == 0)
            {
                Debug.Log("[RelocalizeUseCase] No anchors to relocalize");
                return false;
            }

            Debug.Log($"[RelocalizeUseCase] Found {anchors.Count} anchors");

            // iOS: ARWorldMapManager üzerinden world map yükle
            // Android: Kullanıcıya bildirim göster (yeniden tarama gerekli)
            
            // TODO: Platform-spesifik relocalization mantığı
            // Bu use case, AR katmanındaki ARWorldMapManager ile koordine olacak

            return true;
        }
    }
}