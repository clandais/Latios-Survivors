using Latios;
using Latios.Psyshock;
using Latios.Transforms;
using Survivors.Play.Authoring.Enemies;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;

namespace Survivors.Play.Systems
{
	
	public partial struct EnemyCollisionLayer : ICollectionComponent
	{
		public CollisionLayer Layer;

		public JobHandle TryDispose(JobHandle inputDeps)
		{
			return Layer.IsCreated ? Layer.Dispose(inputDeps) : inputDeps;
		}
	}
	
	public partial struct BuildEnemyCollisionLayerSystem : ISystem, ISystemNewScene
	{
		LatiosWorldUnmanaged _world;
		BuildCollisionLayerTypeHandles _buildCollisionLayerTypeHandles;
		EntityQuery _query;
		
		[BurstCompile]
		public void OnCreate(ref SystemState state)
		{
			_world                          = state.GetLatiosWorldUnmanaged();
			_buildCollisionLayerTypeHandles = new BuildCollisionLayerTypeHandles(ref state);
			_query                          = state.Fluent().With<EnemyTag>(true).PatchQueryForBuildingCollisionLayer().Build();
		}

		[BurstCompile]
		public void OnUpdate(ref SystemState state)
		{

			_buildCollisionLayerTypeHandles.Update(ref state);
			
			
			var entities = _query.ToEntityArray(state.WorldUpdateAllocator);
			
			float3 min = float3.zero;
			float3 max = float3.zero;
			
			foreach (Entity entity in entities)
			{
				var collider  = SystemAPI.GetComponent<Collider>(entity);
				var transform = SystemAPI.GetComponent<WorldTransform>(entity);
				var aabb      = Physics.AabbFrom(collider, transform.worldTransform);
				min = math.min(aabb.min, min);
				max = math.max(aabb.max, max);
			}
			
			// add a small padding to the AABB
			min -= new float3(5f);
			max += new float3(5f);
			
			var settings = new CollisionLayerSettings { worldAabb = new Aabb(min, max), worldSubdivisionsPerAxis = new int3(1, 1, 8) };
			
			state.Dependency = Physics.BuildCollisionLayer(_query, in _buildCollisionLayerTypeHandles).WithSettings(settings)
				.ScheduleParallel(out CollisionLayer enemyCollisionLayer, Allocator.TempJob, state.Dependency);
			
			_world.sceneBlackboardEntity.SetCollectionComponentAndDisposeOld(new EnemyCollisionLayer { Layer = enemyCollisionLayer });
		}

		[BurstCompile]
		public void OnDestroy(ref SystemState state)
		{

		}

		[BurstCompile]
		public void OnNewScene(ref SystemState state)
		{
			_world.sceneBlackboardEntity.AddOrSetCollectionComponentAndDisposeOld<EnemyCollisionLayer>(default);
		}
	}
}