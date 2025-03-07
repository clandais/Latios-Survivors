using Unity.Entities;
using Unity.Mathematics;

namespace Survivors.Play.Components
{

	public readonly partial struct AgentMotionAspect : IAspect
	{
		readonly RefRO<AgentSettings> _speedSettings;
		public AgentSettings Settings => _speedSettings.ValueRO;

		
		readonly RefRW<AgentVelocityComponent> _velocity;

		readonly RefRW<MotionComponent> _motion;

		public MotionComponent MotionComponent
		{
			get => _motion.ValueRW;
			set => _motion.ValueRW = value;
		}

		public float3 DesiredVelocity
		{
			get => _motion.ValueRW.DesiredVelocity;
			set => _motion.ValueRW.DesiredVelocity = value;
		}

		public float3 Velocity
		{
			get => _motion.ValueRW.Velocity;
			set => _motion.ValueRW.Velocity = value;
		}
		
		public float3 RvoVelocity
		{
			get => _velocity.ValueRW.Velocity;
			set => _velocity.ValueRW.Velocity = value;
		}

		public quaternion Rotation
		{
			get => _motion.ValueRW.Rotation;
			set => _motion.ValueRW.Rotation = value;
		}

		public quaternion DesiredRotation
		{
			get => _motion.ValueRW.DesiredRotation;
			set => _motion.ValueRW.DesiredRotation = value;
		}

		public float3 AvoidanceVelocity
		{
			get => _motion.ValueRW.AvoidanceVelocity;
			set => _motion.ValueRW.AvoidanceVelocity = value;
		}

		public float NormalizedSpeed => math.length(_motion.ValueRO.Velocity) / Settings.RunSpeed;
	}
}