using Latios;
using Latios.Mimic.Addons.Mecanim;
using Latios.Transforms;
using Survivors.Play.Authoring;
using Survivors.Play.Components;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Survivors.Play.Systems
{
	[RequireMatchingQueriesForUpdate]
	[BurstCompile]
	public partial struct PlayerDesiredMotionSystem :  ISystem
	{
		private EntityQuery _query;
		
		[BurstCompile]
		public void OnCreate(ref SystemState state)
		{
			_query = state.Fluent().With<PlayerTag>().With<PlayerInputState>().Build();
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
	partial struct PlayerDesiredMotionJob : IJobEntity
	{
		[ReadOnly] public float DeltaTime;
		
		[BurstCompile]
		public void Execute(
			TransformAspect transformAspect,
			MecanimAspect mecanimAspect,
			in PlayerInputState inputState, 
			in PlayerVelocityChange playerVelocityChange,
			in PlayerSpeedSettings speedSettings,
			
			ref PlayerVelocity playerVelocity,
			ref PlayerDesiredMotion desiredMotion)
		{
			float2 dir = inputState.Direction;
			
			float desiredSpeed = inputState.IsSprinting ? speedSettings.RunSpeed : speedSettings.WalkSpeed;
			
			desiredMotion.DesiredVelocity = new float3(dir.x, 0, dir.y) * desiredSpeed;
			
			playerVelocity.LastValue = playerVelocity.Value;
			playerVelocity.Value = MoveTowards(playerVelocity.Value, desiredMotion.DesiredVelocity, playerVelocityChange.Value * DeltaTime);
			
			transformAspect.worldPosition += playerVelocity.Value * DeltaTime;
		}
		
		
		
		[BurstCompile]
		float3 MoveTowards(float3 current, float3 target, float maxDistanceDelta)
		{
			float num1 = target.x - current.x;
			float num2 = target.y - current.y;
			float num3 = target.z - current.z;
			float d = (float) ((double) num1 * (double) num1 + (double) num2 * (double) num2 + (double) num3 * (double) num3);
			if ((double) d == 0.0 || (double) maxDistanceDelta >= 0.0 && (double) d <= (double) maxDistanceDelta * (double) maxDistanceDelta)
				return target;
			float num4 = (float) math.sqrt((double) d);
			return new Vector3(current.x + num1 / num4 * maxDistanceDelta, current.y + num2 / num4 * maxDistanceDelta, current.z + num3 / num4 * maxDistanceDelta);
		}
	}
}