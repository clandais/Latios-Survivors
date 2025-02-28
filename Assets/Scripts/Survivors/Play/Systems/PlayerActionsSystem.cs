using Latios;
using Latios.Transforms;
using Survivors.Play.Authoring.Weapons;
using Survivors.Play.Components;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace Survivors.Play.Systems
{
	[RequireMatchingQueriesForUpdate]
	public partial struct PlayerActionsSystem : ISystem
	{
		EntityQuery _rightHandQuery;
		
		[BurstCompile]
		public void OnCreate(ref SystemState state)
		{
			_rightHandQuery = state.Fluent().WithEnabled<RightHandSlotThrowAxeTag>()
				.With<RightHandSlot>()
				.Build();
		}

		[BurstCompile]
		public void OnUpdate(ref SystemState state)
		{


			if (!_rightHandQuery.IsEmpty)
			{
				var                 singleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
				EntityCommandBuffer ecb       = singleton.CreateCommandBuffer(state.WorldUnmanaged);
				
				foreach (var entity in _rightHandQuery.ToEntityArray(Allocator.Temp))
				{
					var playerTransform     = SystemAPI.GetComponent<WorldTransform>(entity);
					var rHandSlot           = SystemAPI.GetComponent<RightHandSlot>(entity);
					var rHandSlostTransform = SystemAPI.GetComponent<WorldTransform>(rHandSlot.RightHandSlotEntity);
					
					var axeComponent = state.EntityManager.GetComponentData<AxeComponent>(rHandSlot.AxePrefab);
					
					var axeEntity = ecb.Instantiate(rHandSlot.AxePrefab);
					ecb.SetComponent(axeEntity, new WorldTransform { worldTransform = rHandSlostTransform.worldTransform });
					
					
					axeComponent.Direction = playerTransform.forwardDirection;
					ecb.SetComponent(axeEntity, axeComponent);
					ecb.SetComponentEnabled<RightHandSlotThrowAxeTag>(entity, false);
				}


				

			}
			
		}

		[BurstCompile]
		public void OnDestroy(ref SystemState state)
		{

		}

	}
}