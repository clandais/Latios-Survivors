using Latios;
using Latios.Psyshock;
using Latios.Transforms;
using Survivors.Play.Authoring;
using Survivors.Play.Authoring.Enemies;
using Survivors.Play.Authoring.Level;
using Unity.Burst;
using Unity.Entities;

namespace Survivors.Play.Systems.Debug
{
	[RequireMatchingQueriesForUpdate]
	public partial struct PhysicsDebugSystem : ISystem
	{

		[BurstCompile]
		public void OnCreate(ref SystemState state)
		{
			state.RequireForUpdate<WallsTag>();
		}

		[BurstCompile]
		public void OnUpdate(ref SystemState state)
		{

			
			
			// var grid = state.GetLatiosWorldUnmanaged().sceneBlackboardEntity.GetCollectionComponent<FloorGrid>();
			// FloorGrid.Draw(grid);
			
			var layer = state.GetLatiosWorldUnmanaged().sceneBlackboardEntity.GetCollectionComponent<EnvironmentCollisionLayer>();
			state.Dependency = PhysicsDebug.DrawLayer(layer.Layer).ScheduleParallel(state.Dependency);
			
			var floorLayer = state.GetLatiosWorldUnmanaged().sceneBlackboardEntity.GetCollectionComponent<FloorClollisionLayer>();
			state.Dependency = PhysicsDebug.DrawLayer(floorLayer.Layer).ScheduleParallel(state.Dependency);
			
			
			var enemyLayer = state.GetLatiosWorldUnmanaged().sceneBlackboardEntity.GetCollectionComponent<EnemyCollisionLayer>();
			if (enemyLayer.Layer.IsCreated)
				state.Dependency = PhysicsDebug.DrawLayer(enemyLayer.Layer).ScheduleParallel(state.Dependency);

			// foreach (var (collider, transformAspect) in SystemAPI.Query<RefRO<Collider>, TransformAspect>().WithAll<WallsTag>())
			// {
			// 	var transform = transformAspect.worldTransform;
			// 	PhysicsDebug.DrawCollider( in collider.ValueRO, in transform, UnityEngine.Color.cyan);
			// }
			//
			// foreach (var (collider, transformAspect) in SystemAPI.Query<RefRO<Collider>, TransformAspect>().WithAll<FloorTag>())
			// {
			// 	var transform = transformAspect.worldTransform;
			// 	PhysicsDebug.DrawCollider( in collider.ValueRO, in transform, UnityEngine.Color.green);
			// }
			//
			// foreach (var (collider, transformAspect) in SystemAPI.Query<RefRO<Collider>, TransformAspect>().WithAll<EnemyTag>())
			// {
			// 	var transform = transformAspect.worldTransform;
			// 	PhysicsDebug.DrawCollider( in collider.ValueRO, in transform, UnityEngine.Color.blue);
			// }
			//
			//
		}

		[BurstCompile]
		public void OnDestroy(ref SystemState state)
		{

		}
	}
}