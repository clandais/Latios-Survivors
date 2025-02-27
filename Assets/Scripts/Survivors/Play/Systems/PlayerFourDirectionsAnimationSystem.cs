using Latios;
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

			if (motion.NormalizedSpeed <= idleThreshold && math.length(inputState.Direction) < idleThreshold)
			{
				ref ClipState state = ref clipStates.Center;
				state.Time += DeltaTime * state.SpeedMultiplier;

				ref SkeletonClip clip = ref clips.ClipSet.Value.clips[(int)EDirections.Center];
				clip.SamplePose(ref skeleton, clip.LoopToClipTime(state.Time), 1f);
			}
			else
			{
				float2 rotatedVelocity = math.normalizesafe(math.mul(motion.Rotation, motion.Velocity).xz);
				
				{
					ref ClipState state = ref clipStates.Left;
					state.Time += DeltaTime * state.SpeedMultiplier;

					ref SkeletonClip clip = ref clips.ClipSet.Value.clips[(int)EDirections.Left];
					clip.SamplePose(ref skeleton, clip.LoopToClipTime(state.Time), 1f - rotatedVelocity.x);
				}

				{
					ref ClipState state = ref clipStates.Right;
					state.Time += DeltaTime * state.SpeedMultiplier;

					ref SkeletonClip clip = ref clips.ClipSet.Value.clips[(int)EDirections.Right];
					clip.SamplePose(ref skeleton, clip.LoopToClipTime(state.Time), rotatedVelocity.x);
				}


				{
					ref ClipState state = ref clipStates.Down;
					state.Time += DeltaTime * state.SpeedMultiplier;

					ref SkeletonClip clip = ref clips.ClipSet.Value.clips[(int)EDirections.Down];
					clip.SamplePose(ref skeleton, clip.LoopToClipTime(state.Time), 1f - rotatedVelocity.y);
				}

				{
					ref ClipState state = ref clipStates.Up;
					state.Time += DeltaTime * state.SpeedMultiplier;

					ref SkeletonClip clip = ref clips.ClipSet.Value.clips[(int)EDirections.Up];
					clip.SamplePose(ref skeleton, clip.LoopToClipTime(state.Time), rotatedVelocity.y);
				}
			}

			skeleton.EndSamplingAndSync();

			ref SkeletonClip axeThrowClip  = ref clips.ClipSet.Value.clips[(int)EDirections.AxeThrow];
			ref ClipState    axeThrowState = ref clipStates.AxeThrow;

			if (inputState.MainAttackTriggered || axeThrowState.Time < axeThrowClip.duration && axeThrowState.Time > 0.033f)
			{

				axeThrowState.Time += DeltaTime * axeThrowState.SpeedMultiplier;
				axeThrowClip.SamplePose(ref skeleton, masks.Blob.Value.masks[0].AsSpan(), axeThrowClip.LoopToClipTime(axeThrowState.Time), 1f);
			}
			else
			{
				axeThrowState.Time = 0;
			}


			if (skeleton.needsSync) skeleton.EndSamplingAndSync();
		}
	}
}