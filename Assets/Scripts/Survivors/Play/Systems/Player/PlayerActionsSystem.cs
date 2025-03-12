using Latios;
using Latios.Transforms;
using Survivors.Play.Authoring.Weapons;
using Survivors.Play.Components;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace Survivors.Play.Systems.Player
{
	[RequireMatchingQueriesForUpdate]
	public partial struct PlayerActionsSystem : ISystem, ISystemNewScene
	{
		EntityQuery _rightHandQuery;
		LatiosWorldUnmanaged _world;
		
		[BurstCompile]
		public void OnCreate(ref SystemState state)
		{
			_rightHandQuery = state.Fluent().WithEnabled<RightHandSlotThrowAxeTag>()
				.With<RightHandSlot>()
				.Build();
			
			_world = state.GetLatiosWorldUnmanaged();
			

		}

		[BurstCompile]
		public void OnUpdate(ref SystemState state)
		{

			// if (!_world.worldBlackboardEntity.HasCollectionComponent<AxeSpawnQueue>())
			// {
			// 	_world.worldBlackboardEntity.AddOrSetCollectionComponentAndDisposeOld(new AxeSpawnQueue
			// 	{
			// 		AxesQueue = new NativeQueue<AxeSpawnQueue.AxeSpawnData>(Allocator.Persistent)
			// 	});
			// }


			if (!_rightHandQuery.IsEmpty)
			{
				var                 singleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
				EntityCommandBuffer ecb       = singleton.CreateCommandBuffer(state.WorldUnmanaged);



				var mouse = _world.sceneBlackboardEntity.GetComponentData<SceneMouse>();
				

				var spawnQueue = _world.worldBlackboardEntity.GetCollectionComponent<AxeSpawnQueue>().AxesQueue;

				
				foreach (var entity in _rightHandQuery.ToEntityArray(Allocator.Temp))
				{
					var playerTransform     = SystemAPI.GetComponent<WorldTransform>(entity);
					var rHandSlot           = SystemAPI.GetComponent<RightHandSlot>(entity);
					var rHandSlotTransform  = SystemAPI.GetComponent<WorldTransform>(rHandSlot.RightHandSlotEntity);
					
					
					// get the direction of the axe
					float2 direction2D = math.normalizesafe( mouse.WorldPosition - playerTransform.position.xz);
					float3 direction =  new float3(direction2D.x, 0f, direction2D.y);
					
					spawnQueue.Enqueue( new AxeSpawnQueue.AxeSpawnData()
					{
						AxePrefab = rHandSlot.AxePrefab,
						Direction = direction,
						Position = rHandSlotTransform.worldTransform.position,
					});
					
					ecb.SetComponentEnabled<RightHandSlotThrowAxeTag>(entity, false);
				}
	
			}
			
		}

		[BurstCompile]
		public void OnDestroy(ref SystemState state)
		{

		}


		public void OnNewScene(ref SystemState state)
		{
			_world.worldBlackboardEntity.AddOrSetCollectionComponentAndDisposeOld(new AxeSpawnQueue
			{
				AxesQueue = new NativeQueue<AxeSpawnQueue.AxeSpawnData>(Allocator.Persistent)
			});
		}
	}
}