using Latios;
using Latios.Psyshock;
using Survivors.Play.Authoring;
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
			_typeHandles.Update(ref state);

			var settings = new CollisionLayerSettings { worldAabb = new Aabb(-100f, 100f), worldSubdivisionsPerAxis = new int3(1, 1, 8) };

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