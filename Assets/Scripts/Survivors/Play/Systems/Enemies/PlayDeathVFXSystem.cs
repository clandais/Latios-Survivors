using Latios;
using Latios.Psyshock;
using Latios.Transforms;
using Survivors.Play.Authoring.Enemies;
using Survivors.Play.Components;
using Unity.Burst;
using Unity.Entities;

namespace Survivors.Play.Systems.Enemies
{
    [RequireMatchingQueriesForUpdate]
    [BurstCompile]
    public partial struct PlayDeathVFXSystem : ISystem
    {
        LatiosWorldUnmanaged _world;
        EntityQuery _entityQuery;
        
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            _entityQuery = state.Fluent()
                .With<EnemyTag>()
                .With<DeadTag>()
                .With<HitInfos>()
                .With<SkeletonDestroyVfx>()
                // we want to play the SFX just before the collider is removed
                .With<Collider>()
                .Build();
            
            _world = state.GetLatiosWorldUnmanaged();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {

            var icb = _world.syncPoint.CreateInstantiateCommandBuffer<PositionInitialVelocityVFX>();
            state.Dependency = new SpawnDeathVFXJob
            {
                CommandBuffer = icb.AsParallelWriter()
            }.ScheduleParallel(_entityQuery, state.Dependency);
            
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {

        }
        
        
        [BurstCompile]
        partial struct SpawnDeathVFXJob : IJobEntity
        {
            public InstantiateCommandBuffer<PositionInitialVelocityVFX>.ParallelWriter CommandBuffer;
            
            public void Execute( Entity _, [EntityIndexInQuery] int idx, in SkeletonDestroyVfx vfx,  in HitInfos hits)
            {
                CommandBuffer.Add(vfx.Prefab, new PositionInitialVelocityVFX
                {
                    Position        = hits.Position,
                    InitialVelocity = hits.Normal,
                }, idx);
            }
        }
    }
}