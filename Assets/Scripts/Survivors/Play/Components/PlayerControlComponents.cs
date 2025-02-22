using Unity.Entities;
using Unity.Mathematics;

namespace Survivors.Play.Components
{
	
	public struct PlayerTag : IComponentData { }
	
	public struct PlayerInputState : IComponentData
	{
		public float2 Direction;
		public bool IsSprinting;
	}
	
	public struct PlayerDesiredMotion : IComponentData
	{
		public float3 DesiredVelocity;
	}
	
	public struct PlayerSpeedSettings : IComponentData
	{
		public float WalkSpeed;
		public float RunSpeed;
	}
	
	public struct PlayerVelocityChange : IComponentData
	{
		public float Value;
	}
	
	public struct PlayerVelocity : IComponentData
	{
		public float3 LastValue;
		public float3 Value;
	}
}