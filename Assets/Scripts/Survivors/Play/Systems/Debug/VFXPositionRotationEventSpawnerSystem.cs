using Latios;
using Latios.LifeFX;
using Latios.Transforms;
using Survivors.Play.Components;
using Survivors.Play.Components.VFXTunnels;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Survivors.Play.Systems.Debug
{

    [RequireMatchingQueriesForUpdate]
    [BurstCompile]
    public partial struct VFXPositionRotationEventSpawnerSystem : ISystem
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
            var mailBoxe = m_world.worldBlackboardEntity.GetCollectionComponent<GraphicsEventPostal>(true)
                .GetMailbox<PositionRotationEventInput>();

            var dcb = m_world.syncPoint.CreateDestroyCommandBuffer();

            foreach (var (transform, eventSpawner, entity) in SystemAPI.Query<WorldTransform, RefRW<PositionRotationEventSpawner>>().WithEntityAccess())
            {
                ref var sp = ref eventSpawner.ValueRW;
                sp.TimeUntilNextSpawn -= SystemAPI.Time.DeltaTime;
                
                if (sp.TimeUntilNextSpawn < 0f)
                {
                    var rotation = math.Euler(transform.rotation);
                    rotation.z *= 10f;
                    mailBoxe.Send(new PositionRotationEventInput
                    {
                        Position = transform.position,
                        Rotation = rotation,
                    }, sp.EventTunnel);
                    
                 
                    dcb.Add(entity);
                }
            }
        }
    }
}
