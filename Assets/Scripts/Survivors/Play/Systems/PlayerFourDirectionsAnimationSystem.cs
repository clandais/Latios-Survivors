using Latios.Kinemation;
using Survivors.Play.Authoring.Animations;
using Survivors.Play.Components;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;

namespace Survivors.Play.Systems
{
	public partial struct PlayerFourDirectionsAnimationSystem : ISystem
	{
		[BurstCompile]
		public void OnCreate(ref SystemState state)
		{
			state.RequireForUpdate<PlayerTag>();
			state.RequireForUpdate<FourDirectionClipStates>();
		}

		[BurstCompile]
		public void OnUpdate(ref SystemState state)
		{
			state.Dependency = new AnimationJob
			{
				DeltaTime = SystemAPI.Time.DeltaTime
			}.ScheduleParallel(state.Dependency);
		}

		[BurstCompile]
		public void OnDestroy(ref SystemState state)
		{

		}

		[WithAll(typeof(PlayerTag))]
		[BurstCompile]
		internal partial struct AnimationJob : IJobEntity
		{
			public float DeltaTime;

			public void Execute(
				OptimizedSkeletonAspect skeleton,
				PlayerMotionAspect motion,
				in Clips clips,
				in PlayerInputState inputState,
				ref FourDirectionClipStates clipStates,
				ref AvatarMasks masks)

			{
				float idleThreshold = .1f;

				ref ClipState    state = ref clipStates.Center;
				ref SkeletonClip clip  = ref clips.ClipSet.Value.clips[(int)EDirections.Center];

				if (motion.NormalizedSpeed <= idleThreshold && math.length(inputState.Direction) < idleThreshold)
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