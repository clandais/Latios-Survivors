
using Latios.Transforms;
using Survivors.Play.Components;
using Survivors.Play.Utilities;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace Survivors.Play.Systems
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
			in PlayerInputState inputState,
			in PlayerSpeedSettings speedSettings,
			ref PlayerMotion motion)
		{
			float2 dir = inputState.Direction;

			float desiredSpeed = inputState.IsSprinting ? speedSettings.RunSpeed : speedSettings.WalkSpeed;

			motion.DesiredVelocity = new float3(dir.x, 0, dir.y) * desiredSpeed;

			motion.Velocity = motion.Velocity.MoveTowards(motion.DesiredVelocity, speedSettings.VelocityChange * DeltaTime);

			float3     position     = transformAspect.worldPosition;
			float2     aim          = inputState.MousePosition;
			float3     lookDir      = new float3(aim.x, 0, aim.y) - position;
			quaternion lookRotation = quaternion.LookRotation(lookDir, math.up());
			motion.DesiredRotation = lookRotation;
			motion.Rotation = transformAspect.worldRotation.RotateTowards(motion.DesiredRotation, 90 * DeltaTime);

		}
	}
}