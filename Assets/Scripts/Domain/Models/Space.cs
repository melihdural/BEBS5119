// Assets/Scripts/Domain/Models/Space.cs

using System;
using System.Collections.Generic;

namespace Domain.Models
{
    /// <summary>
    /// Mekân/bahçe modeli. Birden fazla bahçe yönetimi için (gelecek iterasyon).
    /// MVP'de tek bir default space kullanılır.
    /// </summary>
    [Serializable]
    public class Space
    {
        public string id;
        public string name;
        public string description;
        public List<string> memoryIds;       // Bu space'e ait anı ID'leri
        public long createdTimestamp;

        public Space()
        {
            id = Guid.NewGuid().ToString();
            memoryIds = new List<string>();
            createdTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        }
    }
}