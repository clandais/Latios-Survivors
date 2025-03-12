using Latios;
using Latios.LifeFX;
using Latios.Transforms;
using Survivors.Play.Components;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;

namespace Survivors.Play.Systems.Debug
{
    [RequireMatchingQueriesForUpdate]
    [BurstCompile]
    public partial struct VFXPositionEventSpawnerSystem : ISystem 
    {
        LatiosWorldUnmanaged m_world;
        
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            m_world = state.GetLatiosWorldUnmanaged();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {

            var mailBox = m_world.worldBlackboardEntity.GetCollectionComponent<GraphicsEventPostal>(true)
                .GetMailbox<float3>();
            foreach (var (transform, spawner) in SystemAPI.Query<RefRO<WorldTransform>, RefRW<PositionEventSpawner>>())
            {
                ref var sp = ref spawner.ValueRW;
                sp.TimeUntilNextSpawn -= SystemAPI.Time.DeltaTime;

                if (sp.TimeUntilNextSpawn < 0f)
                {
                    sp.TimeUntilNextSpawn += sp.TimeBetweenSpawns;
                    mailBox.Send(transform.ValueRO.position, sp.EventTunnel);
                }
            }
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {

        }

    }
}