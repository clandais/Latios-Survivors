using Latios;
using Latios.Transforms;
using Survivors.Play.Components;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

namespace Survivors.Play.Systems.SFX
{
    public partial struct SFXSpawnQueueSystem : ISystem, ISystemNewScene
    {
        LatiosWorldUnmanaged _world;
        
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            _world = state.GetLatiosWorldUnmanaged();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {

            var spawnQueue = _world.sceneBlackboardEntity.GetCollectionComponent<SFXSpawnQueue>();
            var icb = _world.syncPoint.CreateInstantiateCommandBuffer<WorldTransform>();

            while (!spawnQueue.SFXSQueue.IsEmpty())
            {
                if (spawnQueue.SFXSQueue.TryDequeue(out var sfx))
                {
                    var transform = new WorldTransform
                    {
                        worldTransform = TransformQvvs.identity
                    };

                    transform.worldTransform.position = sfx.Position;

                    icb.Add(sfx.SFXPrefab, transform);
                }
            }
            
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {

        }

        public void OnNewScene(ref SystemState state)
        {
            _world.sceneBlackboardEntity.AddOrSetCollectionComponentAndDisposeOld( new SFXSpawnQueue
            {
                SFXSQueue = new NativeQueue<SFXSpawnQueue.SFXSpawnData>(Allocator.Persistent)
            });
        }
    }
}
