using Latios;
using Survivors.Play.Authoring.Weapons;
using Survivors.Play.Components;
using Unity.Burst;
using Unity.Entities;

namespace Survivors.Play.Systems.Initialization
{
	public partial struct PlayerInitializationSystem : ISystem
	{
		[BurstCompile]
		public void OnCreate(ref SystemState state)
		{
			state.RequireForUpdate<PlayerTag>();
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