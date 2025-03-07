using Latios.Transforms;
using Survivors.Play.Components;
using Survivors.Play.Utilities;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace Survivors.Play.Systems.Player
{
	[RequireMatchingQueriesForUpdate]
	[BurstCompile]
	public partial struct PlayerDesiredMotionSystem : ISystem
	{

		[BurstCompile]
		public void OnCreate(ref SystemState state)
		{
			state.RequireForUpdate<PlayerTag>();
		}

		[BurstCompile]
		public void OnUpdate(ref SystemState state)
		{
			state.Dependency = new PlayerDesiredMotionJob { DeltaTime = SystemAPI.Time.DeltaTime }
				.ScheduleParallel(state.Dependency);
		}

		[BurstCompile]
		public void OnDestroy(ref SystemState state)
		{

		}
	}


	[BurstCompile]
	internal partial struct PlayerDesiredMotionJob : IJobEntity
	{
		[ReadOnly] public float DeltaTime;

		[BurstCompile]
		public void Execute(
			TransformAspect transformAspect,
			AgentMotionAspect motionAspect,
			in PlayerInputState inputState)
		{
			float2 dir = inputState.Direction;

			
			// Currently no sprinting involved
			float desiredSpeed = inputState.IsSprinting ? motionAspect.Settings.RunSpeed : motionAspect.Settings.WalkSpeed;

			motionAspect.DesiredVelocity = new float3(dir.x, 0, dir.y) * desiredSpeed;

			motionAspect.Velocity = motionAspect.Velocity.MoveTowards(motionAspect.DesiredVelocity, motionAspect.Settings.VelocityChange * DeltaTime);

			float3     position     = transformAspect.worldPosition;
			float2     aim          = inputState.MousePosition;
			float3     lookDir      = new float3(aim.x, 0, aim.y) - position;
			quaternion lookRotation = quaternion.LookRotation(lookDir, math.up());
			motionAspect.DesiredRotation = lookRotation;
			motionAspect.Rotation        = transformAspect.worldRotation.RotateTowards(motionAspect.DesiredRotation, 90 * DeltaTime);

		}
	}
}