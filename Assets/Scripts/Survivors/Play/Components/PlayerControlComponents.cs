using Unity.Entities;
using Unity.Mathematics;

namespace Survivors.Play.Components
{
	
	public struct PlayerTag : IComponentData { }
	
	public struct PlayerInputState : IComponentData
	{
		public float2 Direction;
		public bool IsSprinting;
		public float2 MousePosition;
	}
	
	public struct PlayerMotion : IComponentData
	{
		public float3 DesiredVelocity;
		public quaternion DesiredRotation;
		public float3 Velocity;
		public quaternion Rotation;
	}
	
	public struct PlayerSpeedSettings : IComponentData
	{
		public float WalkSpeed;
		public float RunSpeed;
		public float VelocityChange;
	}
	
}