using Latios;
using Latios.Mimic.Addons.Mecanim;
using Latios.Transforms;
using Survivors.Play.Authoring;
using Survivors.Play.Components;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;

namespace Survivors.Play.Systems
{
	[RequireMatchingQueriesForUpdate]
	[BurstCompile]
	public partial struct PlayerControllerSystem : ISystem
	{

		private EntityQuery _query;
		
		[BurstCompile]
		public void OnCreate(ref SystemState state)
		{
			state.Fluent().With<PlayerTag>().With<PlayerInputState>().Build();
		}


		[BurstCompile]
		public void OnUpdate(ref SystemState state)
		{
			
			float dt = SystemAPI.Time.DeltaTime;
			
			foreach (var (inputState, worldTransform, mecanimAspect) in 
			         SystemAPI.Query<RefRO<PlayerInputState>, RefRW<WorldTransform>, MecanimAspect>().WithAll<PlayerTag>())
			{
				float2 dir = inputState.ValueRO.Direction;
				
				//	worldTransform.ValueRW.worldTransform.position += new float3(dir.x, 0, dir.y) * dt;
					
			//	mecanimAspect.speed
					mecanimAspect.SetBool("IsWalking", (math.lengthsq(dir) > 0.01f));
				
			}
		}
	}
}