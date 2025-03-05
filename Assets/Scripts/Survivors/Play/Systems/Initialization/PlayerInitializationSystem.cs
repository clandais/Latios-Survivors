using Latios;
using Survivors.Play.Authoring.Weapons;
using Survivors.Play.Components;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

namespace Survivors.Play.Systems.Initialization
{
	[RequireMatchingQueriesForUpdate]
	public partial struct PlayerInitializationSystem : ISystem
	{
		LatiosWorldUnmanaged _world;
		
		[BurstCompile]
		public void OnCreate(ref SystemState state)
		{
			state.RequireForUpdate<PlayerTag>();
			_world = state.GetLatiosWorldUnmanaged();
		}
		

		[BurstCompile]
		public void OnUpdate(ref SystemState state)
		{
			state.Enabled = false;
			
			foreach (var (_, entity) in SystemAPI.Query<RefRO<RightHandSlot>>().WithAll<RightHandSlotThrowAxeTag>().WithEntityAccess())
			{
				SystemAPI.SetComponentEnabled<RightHandSlotThrowAxeTag>(entity, false);
			}
		}

		[BurstCompile]
		public void OnDestroy(ref SystemState state)
		{

		}


	}
}