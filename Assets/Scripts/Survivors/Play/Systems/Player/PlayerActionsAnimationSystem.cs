using Latios;
using Latios.Kinemation;
using Survivors.Play.Authoring.Animations;
using Survivors.Play.Authoring.Weapons;
using Survivors.Play.Components;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

namespace Survivors.Play.Systems.Player
{
	public partial struct PlayerActionsAnimationSystem : ISystem
	{


		[BurstCompile]
		public void OnCreate(ref SystemState state)
		{
			state.RequireForUpdate<PlayerTag>();
		}

		[BurstCompile]
		public void OnUpdate(ref SystemState state)
		{


			var                 singleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
			EntityCommandBuffer ecb       = singleton.CreateCommandBuffer(state.WorldUnmanaged);

			state.Dependency = new AnimationJob
			{
				DeltaTime = SystemAPI.Time.DeltaTime,
				Ecb       = ecb.AsParallelWriter()
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

			[ReadOnly] public float DeltaTime;
			public EntityCommandBuffer.ParallelWriter Ecb;

			public void Execute(
				Entity entity,
				[ChunkIndexInQuery] int chunkIndexInQuery,
				OptimizedSkeletonAspect skeleton,
				in Clips clips,
				in PlayerInputState inputState,
				ref FourDirectionClipStates clipStates,
				ref AvatarMasks masks)
			{
				ref SkeletonClip axeThrowClip  = ref clips.ClipSet.Value.clips[(int)EDirections.Attack];
				ref ClipState    axeThrowState = ref clipStates.Attack;

				if (inputState.MainAttackTriggered || axeThrowState.PreviousTime < axeThrowState.Time)
				{

					axeThrowState.Update(DeltaTime * axeThrowState.SpeedMultiplier);
					axeThrowState.Time = axeThrowClip.LoopToClipTime(axeThrowState.Time);
					axeThrowClip.SamplePose(ref skeleton, masks.Blob.Value.masks[0].AsSpan(), axeThrowState.Time, 1f);


					if (axeThrowClip.events.TryGetEventsRange(axeThrowState.PreviousTime, axeThrowState.Time, out int index, out int evenyCount))
					{
						for (int i = index; i < evenyCount; i++)
						{
							int evt = axeThrowClip.events.nameHashes[i];
							if (evt == axeThrowState.EventHash)
							{
								Ecb.SetComponentEnabled<RightHandSlotThrowAxeTag>(chunkIndexInQuery, entity, true);
							}
						}
					}

				}
				else
				{
					axeThrowState.Time = 0;
				}


				if (skeleton.needsSync) skeleton.EndSamplingAndSync();
			}
		}


	}


}