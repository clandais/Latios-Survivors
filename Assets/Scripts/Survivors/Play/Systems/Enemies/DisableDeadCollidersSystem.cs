using Latios;
using Latios.Psyshock;
using Survivors.Play.Authoring.Enemies;
using Survivors.Play.Components;
using Unity.Burst;
using Unity.Entities;

namespace Survivors.Play.Systems.Enemies
{
	[RequireMatchingQueriesForUpdate]
	public partial struct DisableDeadCollidersSystem : ISystem
	{
		
		EntityQuery _query;
		LatiosWorldUnmanaged _world;
		
		[BurstCompile]
		public void OnCreate(ref SystemState state)
		{
			_query = state.Fluent()
				.With<EnemyTag>()
				.With<DeadTag>()
				.With<Collider>()
				.Build();
			
			_world = state.GetLatiosWorldUnmanaged();
		}

		[BurstCompile]
		public void OnUpdate(ref SystemState state)
		{

			var rcb = _world.syncPoint.CreateEntityCommandBuffer();
			
			state.Dependency = new RemoveCollidersJob
			{
				CommandBuffer = rcb.AsParallelWriter()
			}.ScheduleParallel(_query, state.Dependency);
		}

		[BurstCompile]
		public void OnDestroy(ref SystemState state)
		{

		}
	}
	
	[BurstCompile]
	partial struct RemoveCollidersJob : IJobEntity
	{
		public EntityCommandBuffer.ParallelWriter CommandBuffer;
		
		public void Execute(Entity entity, [EntityIndexInQuery] int index)
		{
			CommandBuffer.RemoveComponent<Collider>(index, entity);
		}
	}
}