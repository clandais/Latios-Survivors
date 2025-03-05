using Latios;
using Latios.Kinemation;
using Survivors.Play.Components;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;

namespace Survivors.Play.Systems
{
	
	[RequireMatchingQueriesForUpdate]
	public partial struct FourDirectionsAnimationSystem : ISystem
	{
		
		LatiosWorldUnmanaged m_latiosWorldUnmanaged;
		EntityQuery m_query;
		
		[BurstCompile]
		public void OnCreate(ref SystemState state)
		{
			m_latiosWorldUnmanaged = state.GetLatiosWorldUnmanaged();
			m_query = state.Fluent()
				.WithAspect<OptimizedSkeletonAspect>()
				.With<Clips>()
				.With<FourDirectionClipStates>()
				.WithAspect<AgentMotionAspect>()
				.Without<DeadTag>()
				.Build();
		}

		[BurstCompile]
		public void OnUpdate(ref SystemState state)
		{
			state.Dependency = new AnimationJob
			{
				DeltaTime = SystemAPI.Time.DeltaTime
			}.ScheduleParallel(m_query, state.Dependency);
		}

		[BurstCompile]
		public void OnDestroy(ref SystemState state)
		{

		}
		
		[BurstCompile]
		internal partial struct AnimationJob : IJobEntity
		{
			public float DeltaTime;

			public void Execute(
				OptimizedSkeletonAspect skeleton,
				AgentMotionAspect motion,
				in Clips clips,
				ref FourDirectionClipStates clipStates)

			{
				float idleThreshold = .1f;

				ref ClipState    state = ref clipStates.Center;
				ref SkeletonClip clip  = ref clips.ClipSet.Value.clips[(int)EDirections.Center];

				if (motion.NormalizedSpeed <= idleThreshold && math.length(motion.DesiredVelocity) < idleThreshold)
				{
					state.Update(DeltaTime * state.SpeedMultiplier);
					state.Time = clip.LoopToClipTime(state.Time);
					clip.SamplePose(ref skeleton, state.Time, 1f);
				}
				else
				{
					float2 rotatedVelocity = math.normalizesafe(math.mul(motion.Rotation, motion.Velocity).xz);
					float  weight          = 0f;

					for (int i = 1; i < 5; i++)
					{

						switch (i)
						{
							case (int)EDirections.Up:
								state  = ref clipStates.Up;
								weight = rotatedVelocity.y;
								clip   = ref clips.ClipSet.Value.clips[(int)EDirections.Up];
								break;
							case (int)EDirections.Down:
								state  = ref clipStates.Down;
								weight = 1f - rotatedVelocity.y;
								clip   = ref clips.ClipSet.Value.clips[(int)EDirections.Down];
								break;
							case (int)EDirections.Left:
								state  = ref clipStates.Left;
								weight = 1f - rotatedVelocity.x;
								clip   = ref clips.ClipSet.Value.clips[(int)EDirections.Left];
								break;
							case (int)EDirections.Right:
								state  = ref clipStates.Right;
								weight = rotatedVelocity.x;
								clip   = ref clips.ClipSet.Value.clips[(int)EDirections.Right];
								break;
						}


						state.Update(DeltaTime * state.SpeedMultiplier);
						state.Time = clip.LoopToClipTime(state.Time);
						clip.SamplePose(ref skeleton, clip.LoopToClipTime(state.Time), weight);
					}
				}


				skeleton.EndSamplingAndSync();


			}
		}
	}


}