// Assets/Scripts/Domain/Models/Memory.cs

using System;
using UnityEngine;

namespace Domain.Models
{
    /// <summary>
    /// Anı veri modeli. Bir anının tüm meta bilgilerini içerir.
    /// </summary>
    [Serializable]
    public class Memory
    {
        public string id;                    // Benzersiz ID (GUID)
        public string title;                 // Başlık (zorunlu)
        public string note;                  // Not (opsiyonel)
        public string photoPath;             // Fotoğraf dosya yolu (Application.persistentDataPath relative)
        public string audioPath;             // Ses dosya yolu
        public string anchorId;              // AR anchor ID referansı
        public SerializableVector3 localPosition;     // Anchor'a göre lokal pozisyon
        public SerializableQuaternion localRotation;  // Anchor'a göre lokal rotasyon
        public long createdTimestamp;        // Unix timestamp (oluşturulma)
        public long modifiedTimestamp;       // Unix timestamp (son güncelleme)

        public Memory()
        {
            id = Guid.NewGuid().ToString();
            createdTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            modifiedTimestamp = createdTimestamp;
            localPosition = new SerializableVector3(Vector3.zero);
            localRotation = new SerializableQuaternion(Quaternion.identity);
        }

        /// <summary>
        /// Başlık validasyonu - boş olamaz
        /// </summary>
        public bool IsValid()
        {
            return !string.IsNullOrWhiteSpace(title);
        }
    }

    /// <summary>
    /// Vector3 için JSON serileştirilebilir wrapper
    /// </summary>
    [Serializable]
    public class SerializableVector3
    {
        public float x, y, z;

        public SerializableVector3(Vector3 v)
        {
            x = v.x;
            y = v.y;
            z = v.z;
        }

        public Vector3 ToVector3() => new Vector3(x, y, z);
    }

    /// <summary>
    /// Quaternion için JSON serileştirilebilir wrapper
    /// </summary>
    [Serializable]
    public class SerializableQuaternion
    {
        public float x, y, z, w;

        public SerializableQuaternion(Quaternion q)
        {
            x = q.x;
            y = q.y;
            z = q.z;
            w = q.w;
        }

        public Quaternion ToQuaternion() => new Quaternion(x, y, z, w);
    }
}