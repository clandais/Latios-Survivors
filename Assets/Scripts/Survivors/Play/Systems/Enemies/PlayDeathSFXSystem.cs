
using Latios;
using Latios.Psyshock;
using Latios.Transforms;
using Survivors.Play.Authoring.Enemies;
using Survivors.Play.Authoring.Weapons;
using Survivors.Play.Components;
using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Entities;


namespace Survivors.Play.Systems.Enemies
{
    [RequireMatchingQueriesForUpdate]
    public partial struct PlayDeathSFXSystem : ISystem, ISystemNewScene
    {
        LatiosWorldUnmanaged _world;
        EntityQuery _entityQuery;
        
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            _entityQuery = state.Fluent()
                .With<EnemyTag>()
                .With<DeadTag>()
                .With<DeathClipsBufferElement>()
                .With<WorldTransform>()
                // we wa,t to play the SFX just before the collider is removed
                .With<Collider>()
                .Build();
            
            _world = state.GetLatiosWorldUnmanaged();
        }

        
        public void OnNewScene(ref SystemState state)
        {
            state.InitSystemRng("PlayDeathSFXSystem");
        }
        
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {

            var icb = _world.syncPoint.CreateInstantiateCommandBuffer<WorldTransform>();
            state.Dependency = new SpawnDeathSFXJob
            {
                Rng = state.GetJobRng(),
                CommandBuffer = icb.AsParallelWriter()
            }.ScheduleParallel(_entityQuery, state.Dependency);

        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {

        }
    }
    
    
    [WithAll(typeof(DeadTag))]
    [BurstCompile]
    partial struct SpawnDeathSFXJob : IJobEntity, IJobEntityChunkBeginEnd
    {
        public SystemRng Rng;
        public InstantiateCommandBuffer<WorldTransform>.ParallelWriter CommandBuffer;

        public void Execute( [EntityIndexInQuery] int idx,  in WorldTransform transform, in DynamicBuffer<DeathClipsBufferElement> buffer)
        {
            
            CommandBuffer.Add(buffer[Rng.NextInt(0, buffer.Length)].AudioPrefab, transform, idx);
        }
        

        public bool OnChunkBegin(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
        {
            Rng.BeginChunk(unfilteredChunkIndex);
            return true;
        }

        public void OnChunkEnd(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask, bool chunkWasExecuted)
        {
        }
    }
}
