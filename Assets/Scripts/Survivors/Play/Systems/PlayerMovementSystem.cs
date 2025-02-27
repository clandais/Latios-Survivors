using Latios;
using Latios.Psyshock;
using Latios.Transforms;
using Survivors.Play.Components;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;


namespace Survivors.Play.Systems
{
	[RequireMatchingQueriesForUpdate]
	[BurstCompile]
	public partial struct PlayerMovementSystem : ISystem
	{
		
		
		[BurstCompile]
		public void OnCreate(ref SystemState state)
		{
			state.RequireForUpdate<PlayerTag>();
		}

		[BurstCompile]
		public void OnUpdate(ref SystemState state)
		{
			float dt = SystemAPI.Time.DeltaTime;

			CollisionLayer collisionLayer = state.GetLatiosWorldUnmanaged().sceneBlackboardEntity.GetCollectionComponent<EnvironmentCollisionLayer>().Layer;
			
			
			state.Dependency =  new CollideAndSlideCharacterJob
			{
				CollisionLayer = collisionLayer,
				DeltaTime = dt,
			}.ScheduleParallel(state.Dependency);
		}

		[BurstCompile]
		public void OnDestroy(ref SystemState state)
		{

		}
	}
	
	[WithAll(typeof(PlayerTag))]
	[BurstCompile]
	partial struct CollideAndSlideCharacterJob : IJobEntity
	{
		[ReadOnly] public CollisionLayer CollisionLayer;
		[ReadOnly] public float DeltaTime;

		public void Execute(TransformAspect transform, in Collider collider, in PlayerMotion playerMotion)
		{
			
			var motion = playerMotion.Velocity * DeltaTime;
			var currentTransform = transform.worldTransform;
			var direction = math.normalizesafe(motion);
			float remainingDistance = math.length(playerMotion.Velocity) * DeltaTime;
			
			float skinEpsilon = 0.01f;
			
			for (int iteration = 0; iteration < 16; iteration++)
			{
				if (remainingDistance < skinEpsilon)
				{
					break;
				}
			
				var end = currentTransform.position + direction * remainingDistance;
				
				if (Physics.ColliderCast(in collider, in currentTransform, end, in CollisionLayer, out var hitInfo, out _))
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
			
			
			
			transform.worldPosition = currentTransform.position;
			transform.worldRotation = playerMotion.Rotation;
		}
	}
}