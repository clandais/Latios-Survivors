using Latios;
using Latios.Psyshock;
using Latios.Transforms;
using Survivors.Play.Authoring.Level;
using Survivors.Play.Authoring.Weapons;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;

namespace Survivors.Play.Systems
{
    public partial struct EnvironmentCollisionLayer : ICollectionComponent
    {
        public CollisionLayer Layer;

        public JobHandle TryDispose(JobHandle inputDeps)
        {
            return Layer.IsCreated ? Layer.Dispose(inputDeps) : inputDeps;
        }
    }
    
    public partial struct FloorClollisionLayer : ICollectionComponent
    {
        public CollisionLayer Layer;

        public JobHandle TryDispose(JobHandle inputDeps)
        {
            return Layer.IsCreated ? Layer.Dispose(inputDeps) : inputDeps;
        }
    }

    [RequireMatchingQueriesForUpdate]
    [BurstCompile]
    public partial struct BuildEnvironmentCollisionLayerSystem : ISystem, ISystemNewScene
    {
        LatiosWorldUnmanaged _world;
        BuildCollisionLayerTypeHandles _typeHandles;
        EntityQuery _wallsQuery;
        EntityQuery _floorQuery;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            _world       = state.GetLatiosWorldUnmanaged();
            _typeHandles = new BuildCollisionLayerTypeHandles(ref state);
            _wallsQuery  = state.Fluent().With<WallsTag>(true).PatchQueryForBuildingCollisionLayer().Build();
            _floorQuery  = state.Fluent().With<FloorTag>(true).PatchQueryForBuildingCollisionLayer().Build();
        }

        [BurstCompile]
        public void OnNewScene(ref SystemState state)
        {
            _world.sceneBlackboardEntity.AddOrSetCollectionComponentAndDisposeOld<EnvironmentCollisionLayer>(default);
            _world.sceneBlackboardEntity.AddOrSetCollectionComponentAndDisposeOld<FloorClollisionLayer>(default);
            
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            state.Enabled = false;

            _typeHandles.Update(ref state);

            var entities = _wallsQuery.ToEntityArray(state.WorldUpdateAllocator);
            var min = float3.zero;
            var max = float3.zero;

            foreach (var entity in entities)
            {
                var collider = SystemAPI.GetComponent<Collider>(entity);
                var transform = SystemAPI.GetComponent<WorldTransform>(entity);

                var aabb = Physics.AabbFrom(collider, transform.worldTransform);
                min = math.min(aabb.min, min);
                max = math.max(aabb.max, max);
            }


            // add some offset to the aabb to make sure the colliders are not at the edge of the world
            min -= new float3(50, 50, 50);
            max += new float3(50, 50, 50);

            var levelAABB = new Aabb(min, max);


            var settings = new CollisionLayerSettings
            {
                worldAabb                = levelAABB,
                worldSubdivisionsPerAxis = new int3(1, 1, 8)
            };

            //state.Dependency =
            var h1 = Physics.BuildCollisionLayer(_wallsQuery, in _typeHandles).WithSettings(settings)
                .ScheduleParallel(out var environmentCollisionLayer, Allocator.Persistent, state.Dependency);

            var h2 = Physics.BuildCollisionLayer(_floorQuery, in _typeHandles).WithSettings(settings)
                .ScheduleParallel(out var floorCollisionLayer, Allocator.Persistent, state.Dependency);

            state.Dependency = JobHandle.CombineDependencies(h1, h2);
            
            _world.sceneBlackboardEntity.SetComponentData(new LevelAABB
            {
                AABB = levelAABB
            });
            _world.sceneBlackboardEntity.SetCollectionComponentAndDisposeOld(new EnvironmentCollisionLayer
                { Layer = environmentCollisionLayer });
            
            _world.sceneBlackboardEntity.SetCollectionComponentAndDisposeOld(new FloorClollisionLayer { Layer = floorCollisionLayer });
            
            _world.sceneBlackboardEntity.AddOrSetCollectionComponentAndDisposeOld<FloorGrid>(default);
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {
        }
    }
}