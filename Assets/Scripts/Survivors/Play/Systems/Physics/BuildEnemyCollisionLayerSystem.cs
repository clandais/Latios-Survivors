using Latios;
using Latios.Psyshock;
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
			
			var settings = new CollisionLayerSettings { worldAabb = new Aabb(-100f, 100f), worldSubdivisionsPerAxis = new int3(1, 1, 8) };
			
			state.Dependency = Physics.BuildCollisionLayer(_query, in _buildCollisionLayerTypeHandles).WithSettings(settings)
				.ScheduleParallel(out CollisionLayer enemyCollisionLayer, Allocator.Persistent, state.Dependency);
			
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