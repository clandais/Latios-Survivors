using Latios;
using Latios.Psyshock;
using Latios.Transforms;
using Survivors.Play.Authoring.Enemies;
using Survivors.Play.Authoring.Weapons;
using Survivors.Play.Components;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace Survivors.Play.Systems
{
	[RequireMatchingQueriesForUpdate]
	[BurstCompile]
	public partial struct AgentMovementSystem : ISystem
	{


		[BurstCompile]
		public void OnCreate(ref SystemState state)
		{

		}

		[BurstCompile]
		public void OnUpdate(ref SystemState state)
		{
			float dt = SystemAPI.Time.DeltaTime;

			CollisionLayer collisionLayer = state.GetLatiosWorldUnmanaged()
				.sceneBlackboardEntity.GetCollectionComponent<EnvironmentCollisionLayer>().Layer;


			state.Dependency = new CollideAndSlideCharacterJob
			{
				CollisionLayer   = collisionLayer,
				DeltaTime        = dt,
			}.ScheduleParallel(state.Dependency);

			state.Dependency.Complete();
			
			var buffer =
				state.GetLatiosWorldUnmanaged().syncPoint.CreateEntityCommandBuffer();
			
			foreach (var transform in SystemAPI.Query<RefRO<WorldTransform>>().WithAll<PlayerTag>())
			{

				buffer.SetComponent(state.GetLatiosWorldUnmanaged().sceneBlackboardEntity, new PlayerPosition
				{
					Position = transform.ValueRO.position,
				});
				
			}
		}

		[BurstCompile]
		public void OnDestroy(ref SystemState state)
		{

		}
	}

	[WithNone(typeof(DeadTag))]
	[BurstCompile]
	internal partial struct CollideAndSlideCharacterJob : IJobEntity
	{
		[ReadOnly] public CollisionLayer CollisionLayer;
		[ReadOnly] public float DeltaTime;
		
		public void Execute(TransformAspect transform, AgentMotionAspect agentMotionAspect,  in Collider collider)
		{

			float3        motion            = agentMotionAspect.Velocity * DeltaTime;
			TransformQvvs currentTransform  = transform.worldTransform;
			float3        direction         = math.normalizesafe(motion);
			float         remainingDistance = math.length(agentMotionAspect.Velocity) * DeltaTime;

			float skinEpsilon = 0.01f;

			for (int iteration = 0; iteration < 16; iteration++)
			{
				if (remainingDistance < skinEpsilon)
				{
					break;
				}

				float3 end = currentTransform.position + direction * remainingDistance;

				if (Physics.ColliderCast(in collider, in currentTransform, end, in CollisionLayer, out ColliderCastResult hitInfo, out _))
				{
					currentTransform.position += direction * (hitInfo.distance - skinEpsilon);
					remainingDistance         =  math.max(0.0f, remainingDistance - hitInfo.distance);
					if (math.dot(hitInfo.normalOnTarget, direction) < -0.9f)
						break;

					direction = math.mul(quaternion.LookRotation(hitInfo.normalOnCaster, direction), math.up());
				}
				else
				{
					currentTransform.position += direction * remainingDistance;
					break;
				}
			}

			currentTransform.position.y = 0f;

			
			transform.worldPosition = currentTransform.position;
			transform.worldRotation = agentMotionAspect.Rotation;
			
		}
	}
}