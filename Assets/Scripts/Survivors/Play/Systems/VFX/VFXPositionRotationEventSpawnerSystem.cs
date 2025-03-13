using Latios;
using Latios.LifeFX;
using Latios.Transforms;
using Survivors.Play.Components;
using Survivors.Play.Components.VFXTunnels;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;

namespace Survivors.Play.Systems.VFX
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

            foreach (var (positionInitialVelocity, eventSpawner, entity) 
                     in SystemAPI.Query<RefRO<PositionInitialVelocityVFX>, RefRW<OneShotPositionRotationEventSpawner>>()
                         .WithEntityAccess())
            {
                ref var sp = ref eventSpawner.ValueRW;

                mailBoxe.Send(new PositionRotationEventInput
                {
                    Position = positionInitialVelocity.ValueRO.Position,
                    Rotation = positionInitialVelocity.ValueRO.InitialVelocity,
                }, sp.EventTunnel);
                
             
                dcb.Add(entity);
                
            }
        }
    }
}
