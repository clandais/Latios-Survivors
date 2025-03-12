using Latios;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;

namespace Survivors.Play.Components
{

    public partial struct SFXSpawnQueue : ICollectionComponent
    {
        public struct SFXSpawnData
        {
            public EntityWith<Prefab> SFXPrefab;
            public float3 Position;
        }
        
        public NativeQueue<SFXSpawnData> SFXSQueue;
        
        public JobHandle TryDispose(JobHandle inputDeps)
        {
            if (!SFXSQueue.IsCreated)
                return inputDeps;
            
            return SFXSQueue.Dispose(inputDeps);
        }
    }
}
