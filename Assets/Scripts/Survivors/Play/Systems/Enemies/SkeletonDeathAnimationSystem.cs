using Latios;
using Latios.Kinemation;
using Survivors.Play.Authoring.Enemies;
using Survivors.Play.Components;
using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Entities;

namespace Survivors.Play.Systems.Enemies
{
	[RequireMatchingQueriesForUpdate]
	public partial struct SkeletonDeathAnimationSystem : ISystem, ISystemNewScene
	{
		LatiosWorldUnmanaged m_latiosWorldUnmanaged;
		EntityQuery m_query;
		
		[BurstCompile]
		public void OnCreate(ref SystemState state)
		{
			m_latiosWorldUnmanaged = state.GetLatiosWorldUnmanaged();
			m_query = state.Fluent()
				.With<EnemyTag>()
				.With<DeadTag>()
				.WithAspect<OptimizedSkeletonAspect>()
				.With<DeathClips>()
				.With<DeathClipsStates>()
				.Build();
		}

		
		public void OnNewScene(ref SystemState state)
		{
			state.InitSystemRng("SkeletonDeathAnimationSystem");
		}
		
		[BurstCompile]
		public void OnUpdate(ref SystemState state)
		{
			
			
			state.Dependency = new DeathAnimationJob
			{
				Rng       = state.GetJobRng(),
				DeltaTime = SystemAPI.Time.DeltaTime
			}.ScheduleParallel(m_query, state.Dependency);
		}

		[BurstCompile]
		public void OnDestroy(ref SystemState state)
		{

		}

		[BurstCompile]
		partial struct DeathAnimationJob : IJobEntity, IJobEntityChunkBeginEnd
		{
			public SystemRng Rng;
			public float DeltaTime;
			
			public void Execute(
				OptimizedSkeletonAspect skeleton,
				in DeathClips clips,
				ref DeathClipsStates clipsStates)
			{

				
				if (clipsStates.ChosenState == -1)
				{
					clipsStates.ChosenState = Rng.NextInt(0, 3);
				}

				ref ClipState state = ref clipsStates.StateA;
				
				switch (clipsStates.ChosenState)
				{
					case 0:
						state = ref clipsStates.StateA;
						break;
					case 1:
						state = ref clipsStates.StateB;
						break;
					case 2:
						state = ref clipsStates.StateC;
						break;
				}
				
				
				
				state.Update(DeltaTime * state.SpeedMultiplier);
				
				if (state.Time >= clips.ClipSet.Value.clips[clipsStates.ChosenState].duration)
				{
					state.Time =clips.ClipSet.Value.clips[clipsStates.ChosenState].duration;
					return;
				}
				
				
				clips.ClipSet.Value.clips[clipsStates.ChosenState].SamplePose(ref skeleton, state.Time, 1f);
				
				skeleton.EndSamplingAndSync();
			}

			public bool OnChunkBegin(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
			{
				Rng.BeginChunk(unfilteredChunkIndex);
				return true;
			}

			public void OnChunkEnd(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask, bool chunkWasExecuted)
			{
				
			}
		}

	}
}