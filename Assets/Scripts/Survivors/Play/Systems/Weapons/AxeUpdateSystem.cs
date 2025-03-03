using Latios.Transforms;
using Survivors.Play.Authoring.Weapons;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;

namespace Survivors.Play.Systems.Weapons
{
	public partial struct AxeUpdateSystem : ISystem
	{
		[BurstCompile]
		public void OnCreate(ref SystemState state)
		{

		}

		[BurstCompile]
		public void OnUpdate(ref SystemState state)
		{

			state.Dependency = new AxeMovementJob
			{
				DeltaTime = SystemAPI.Time.DeltaTime,
			}.ScheduleParallel(state.Dependency);
			
		}

		[BurstCompile]
		public void OnDestroy(ref SystemState state)
		{

		}
		
		[BurstCompile]
		partial struct AxeMovementJob : IJobEntity
		{
			
			public float DeltaTime;
			
			public void Execute(TransformAspect transform, in AxeComponent axe)
			{

				
				transform.TranslateWorld(axe.Direction * axe.Speed * DeltaTime);
				transform.worldRotation =  math.mul(transform.worldTransform.rotation, quaternion.RotateX( axe.RotationSpeed * DeltaTime));
			}
		}
	}
}