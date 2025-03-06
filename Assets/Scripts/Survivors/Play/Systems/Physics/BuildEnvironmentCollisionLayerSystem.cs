using Latios;
using Latios.Psyshock;
using Latios.Transforms;
using Survivors.Play.Authoring;
using Survivors.Play.Authoring.Level;
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

	[RequireMatchingQueriesForUpdate]
	[BurstCompile]
	public partial struct BuildEnvironmentCollisionLayerSystem : ISystem, ISystemNewScene
	{
		private LatiosWorldUnmanaged _world;
		private BuildCollisionLayerTypeHandles _typeHandles;
		private EntityQuery _query;

		[BurstCompile]
		public void OnCreate(ref SystemState state)
		{
			_world       = state.GetLatiosWorldUnmanaged();
			_typeHandles = new BuildCollisionLayerTypeHandles(ref state);
			_query       = state.Fluent().With<LevelTag>(true).PatchQueryForBuildingCollisionLayer().Build();
		}

		[BurstCompile]
		public void OnNewScene(ref SystemState state)
		{
			_world.sceneBlackboardEntity.AddOrSetCollectionComponentAndDisposeOld<EnvironmentCollisionLayer>(default);
		}

		[BurstCompile]
		public void OnUpdate(ref SystemState state)
		{
			
			state.Enabled = false;
			
			_typeHandles.Update(ref state);
		
			var entities = _query.ToEntityArray(state.WorldUpdateAllocator);
			float3 min = float3.zero;
			float3 max = float3.zero;

			foreach (Entity entity in entities)
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

			var settings = new CollisionLayerSettings { 
				worldAabb                = new Aabb(min, max), 
				worldSubdivisionsPerAxis = new int3(1, 1, 8) };

			state.Dependency = Physics.BuildCollisionLayer(_query, in _typeHandles).WithSettings(settings)
				.ScheduleParallel(out CollisionLayer environmentCollisionLayer, Allocator.Persistent, state.Dependency);

			_world.sceneBlackboardEntity.SetCollectionComponentAndDisposeOld(new EnvironmentCollisionLayer { Layer = environmentCollisionLayer });
		}

		[BurstCompile]
		public void OnDestroy(ref SystemState state)
		{

		}


	}
}