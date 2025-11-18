// Assets/Scripts/Domain/Models/AnchorRef.cs

using System;
using UnityEngine;

namespace Domain.Models
{
    /// <summary>
    /// AR anchor referans bilgisi. İlişkili anılarla anchor'ı ilişkilendirir.
    /// </summary>
    [Serializable]
    public class AnchorRef
    {
        public string anchorId;              // Benzersiz anchor ID
        public SerializableVector3 worldPosition;    // Dünya pozisyonu (son bilinen)
        public SerializableQuaternion worldRotation; // Dünya rotasyonu
        public string sessionIdentifier;     // ARWorldMap session ID (iOS için)
        public long lastSeenTimestamp;       // Son görülme zamanı

        public AnchorRef()
        {
            anchorId = Guid.NewGuid().ToString();
            worldPosition = new SerializableVector3(Vector3.zero);
            worldRotation = new SerializableQuaternion(Quaternion.identity);
            lastSeenTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        }

        public AnchorRef(string id, Vector3 position, Quaternion rotation)
        {
            anchorId = id;
            worldPosition = new SerializableVector3(position);
            worldRotation = new SerializableQuaternion(rotation);
            lastSeenTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        }
    }
}