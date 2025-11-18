// Assets/Scripts/Infrastructure/ServiceLocator.cs

using Application;
using Domain.Repositories;

namespace Infrastructure.Persistence
{
    /// <summary>
    /// Basit service locator pattern. Dependency injection yerine.
    /// </summary>
    public class ServiceLocator
    {
        private static ServiceLocator _instance;
        public static ServiceLocator Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new ServiceLocator();
                    _instance.Initialize();
                }
                return _instance;
            }
        }

        // Repositories
        public IMemoryRepository MemoryRepository { get; private set; }
        public IAnchorRepository AnchorRepository { get; private set; }

        // Use Cases
        public AddMemoryUseCase AddMemoryUseCase { get; private set; }
        public ListMemoriesUseCase ListMemoriesUseCase { get; private set; }
        public UpdateMemoryUseCase UpdateMemoryUseCase { get; private set; }
        public DeleteMemoryUseCase DeleteMemoryUseCase { get; private set; }
        public RelocalizeUseCase RelocalizeUseCase { get; private set; }

        private void Initialize()
        {
            // Repository'leri oluştur
            MemoryRepository = new JsonMemoryRepository();
            AnchorRepository = new JsonAnchorRepository();

            // Use case'leri oluştur
            AddMemoryUseCase = new AddMemoryUseCase(MemoryRepository);
            ListMemoriesUseCase = new ListMemoriesUseCase(MemoryRepository);
            UpdateMemoryUseCase = new UpdateMemoryUseCase(MemoryRepository);
            DeleteMemoryUseCase = new DeleteMemoryUseCase(MemoryRepository);
            RelocalizeUseCase = new RelocalizeUseCase(AnchorRepository);
        }

        public void Reset()
        {
            _instance = null;
        }
    }
}