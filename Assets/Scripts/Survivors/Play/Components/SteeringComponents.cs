using Unity.Entities;
using Unity.Mathematics;

namespace Survivors.Play.Components
{

	
	public struct SeparationForce : IComponentData
	{
		public float2 Force;
		public int Count;
	}
	
	public struct AlignmentForce : IComponentData
	{
		public float2 Force;
		public int Count;
	}
	
	public struct CohesionForce : IComponentData
	{
		public float2 Force;
		public int Count;
	}
	
	
	public readonly partial struct SteeringAspect : IAspect
	{
		readonly RefRW<SeparationForce> _separation;
		readonly RefRW<AlignmentForce>  _alignment;
		readonly RefRW<CohesionForce>   _cohesion;
		
		
		public float2 SeparationForce
		{
			get => _separation.ValueRW.Force;
			set => _separation.ValueRW.Force = value;
		}
		
		public float2 AlignmentForce
		{
			get => _alignment.ValueRW.Force;
			set => _alignment.ValueRW.Force = value;
		}
		
		public float2 CohesionForce
		{
			get => _cohesion.ValueRW.Force;
			set => _cohesion.ValueRW.Force = value;
		}
		
		public float2 DesiredVelocity => _separation.ValueRW.Force + _alignment.ValueRW.Force + _cohesion.ValueRW.Force;

		public void Clear()
		{
			_separation.ValueRW.Force = float2.zero;
			_separation.ValueRW.Count = 0;
			_alignment.ValueRW.Force = float2.zero;
			_alignment.ValueRW.Count = 0;
			_cohesion.ValueRW.Force = float2.zero;
			_cohesion.ValueRW.Count = 0;
		}
	}
}