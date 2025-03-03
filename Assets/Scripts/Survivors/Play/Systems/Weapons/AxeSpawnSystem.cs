using Latios;
using Latios.Transforms;
using Survivors.Play.Authoring.Weapons;
using Survivors.Play.Components;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;

namespace Survivors.Play.Systems.Weapons
{
	//[RequireMatchingQueriesForUpdate]
	public partial struct AxeSpawnSystem : ISystem
	{
		LatiosWorldUnmanaged _world;
		
		[BurstCompile]
		public void OnCreate(ref SystemState state)
		{
			_world = state.GetLatiosWorldUnmanaged();
		}

		[BurstCompile]
		public void OnUpdate(ref SystemState state)
		{

			var spawnQueue = _world.worldBlackboardEntity.GetCollectionComponent<AxeSpawnQueue>();
			var axeConfig  = _world.sceneBlackboardEntity.GetComponentData<AxeConfigComponent>();
			var icb        = _world.syncPoint.CreateInstantiateCommandBuffer<AxeComponent, WorldTransform>();
			

			while (!spawnQueue.AxesQueue.IsEmpty())
			{
				if (spawnQueue.AxesQueue.TryDequeue(out var axe))
				{

					var transform = new WorldTransform
					{
						worldTransform = TransformQvvs.identity,
					};
					
					
					transform.worldTransform.position = axe.Position;
					transform.worldTransform.rotation = quaternion.LookRotation(axe.Direction, math.up());
					
					icb.Add(axe.AxePrefab, new AxeComponent
					{
						Direction = axe.Direction,
						Speed = axeConfig.Speed,
						RotationSpeed = axeConfig.RotationSpeed,
					}, transform);
				}
				
			}
		}

		[BurstCompile]
		public void OnDestroy(ref SystemState state)
		{

		}
		
	}
}