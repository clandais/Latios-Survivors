using Latios;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;

namespace Survivors.Play.Components
{
	
	public struct PlayerTag : IComponentData { }
	
	public struct PlayerInputState : IComponentData
	{
		public float2 Direction;
		public bool IsSprinting;
		public float2 MousePosition;
		public bool MainAttackTriggered;
	}
	
	
	public struct AgentVelocityComponent : IComponentData
	{
		public float3 Velocity;
	}
	
	public struct MotionComponent : IComponentData
	{
		public float3 DesiredVelocity;
		public quaternion DesiredRotation;
		public float3 Velocity;
		public quaternion Rotation;
		public float3 AvoidanceVelocity;
	}
	
	public struct AgentSettings : IComponentData
	{
		public float WalkSpeed;
		public float RunSpeed;
		public float VelocityChange;
		public float Radius;
		public float ObstacleHorizon;
	}
	
	public partial struct AxeSpawnQueue : ICollectionComponent
	{

		public struct AxeSpawnData
		{
			public Entity AxePrefab;
			public float3 Direction;
			public float3 Position;
		}
		
		public NativeQueue<AxeSpawnData> AxesQueue;
		
		public JobHandle TryDispose(JobHandle inputDeps)
		{
			if (!AxesQueue.IsCreated)
				return inputDeps;
			
			return AxesQueue.Dispose(inputDeps);
		}
	}
	
}