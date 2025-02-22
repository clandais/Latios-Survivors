using Latios;
using Latios.Kinemation;
using Latios.Mimic.Addons.Mecanim;
using Latios.Transforms;
using Survivors.Play.Authoring;
using Survivors.Play.Components;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace Survivors.Play.Systems
{
	[RequireMatchingQueriesForUpdate]
	[BurstCompile]
	public partial struct PlayerAnimationSystem : ISystem
	{

		private EntityQuery _query;
		private float _perviousDeltaTime;
		
		[BurstCompile]
		public void OnCreate(ref SystemState state)
		{
			state.Fluent()
				.With<PlayerTag>()
				.With<PlayerInputState>()
				.Build();
		}


		[BurstCompile]
		public void OnUpdate(ref SystemState state)
		{

			float t = (float)SystemAPI.Time.ElapsedTime;
			float deltaTime = SystemAPI.Time.DeltaTime;
			
			state.Dependency = new AnimationJob
				{
					Et = t,
					DeltaTime = deltaTime
				}
				.ScheduleParallel(state.Dependency);
			
			_perviousDeltaTime = deltaTime;
		}
		
		[WithAll(typeof(PlayerTag))]
		[BurstCompile]
		private partial struct AnimationJob  : IJobEntity
		{
			[ReadOnly] public float Et;
			[ReadOnly] public float DeltaTime;

			[BurstCompile]
			public void Execute(
				OptimizedSkeletonAspect skeleton, 
				MecanimAspect mecanimAspect, 
				ref PlayerVelocity playerVelocity)
			{
				
				mecanimAspect.SetFloat("Velocity", math.length(playerVelocity.Value));
			}
		}
	}
	
}