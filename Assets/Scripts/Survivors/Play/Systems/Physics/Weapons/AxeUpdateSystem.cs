using Latios;
using Latios.Psyshock;
using Latios.Transforms;
using Survivors.Play.Authoring.Weapons;
using Survivors.Play.Components;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;

namespace Survivors.Play.Systems.Weapons
{
	public partial struct AxeUpdateSystem : ISystem
	{
		
		LatiosWorldUnmanaged m_latiosWorldUnmanaged;
		
		[BurstCompile]
		public void OnCreate(ref SystemState state)
		{
			m_latiosWorldUnmanaged = state.GetLatiosWorldUnmanaged();
		}

		[BurstCompile]
		public void OnUpdate(ref SystemState state)
		{

			var dcb = m_latiosWorldUnmanaged.syncPoint.CreateDestroyCommandBuffer( );

			CollisionLayer collisionLayer = m_latiosWorldUnmanaged.sceneBlackboardEntity
				.GetCollectionComponent<EnvironmentCollisionLayer>().Layer;
			CollisionLayer enemyLayer = m_latiosWorldUnmanaged.sceneBlackboardEntity
				.GetCollectionComponent<EnemyCollisionLayer>().Layer;
			
			var hitEntities = new NativeList<Entity>(Allocator.TempJob);
			
			state.Dependency = new AxeMovementJob
			{
				DeltaTime = SystemAPI.Time.DeltaTime,
				DestroyCommandBuffer = dcb.AsParallelWriter(),
				WallLayer = collisionLayer,
				EnemyLayer = enemyLayer,
				HitEntities = hitEntities,
			}.ScheduleParallel(state.Dependency);
		
			state.Dependency.Complete();
			
			foreach (var entity in hitEntities)
			{
				state.EntityManager.AddComponent<DeadTag>(entity);
			}
			
			
			hitEntities.Dispose();
			
		}

		[BurstCompile]
		public void OnDestroy(ref SystemState state)
		{

		}
		
		[BurstCompile]
		partial struct AxeMovementJob : IJobEntity
		{
			
			public DestroyCommandBuffer.ParallelWriter DestroyCommandBuffer;
			[NativeDisableParallelForRestriction] public NativeList<Entity> HitEntities;
			[ReadOnly] public float DeltaTime;
			[ReadOnly] public CollisionLayer WallLayer;
			[ReadOnly] public CollisionLayer EnemyLayer;


			public void Execute(Entity entity, [EntityIndexInQuery] int idx,  TransformAspect transform, in AxeComponent axe, in Collider collider)
			{
				
				TransformQvvs transformQvs = transform.worldTransform;

				CapsuleCollider capsuleCollider = collider;
				
				
				if (Physics.ColliderCast(in collider, in transformQvs, transform.worldPosition + axe.Direction, in EnemyLayer, out ColliderCastResult hitInfos, out var o))
				{
					 if ( hitInfos.distance <= capsuleCollider.radius )
						HitEntities.Add(o.entity);
				}

				if (Physics.ColliderCast(in collider, in transformQvs, transform.worldPosition + axe.Direction, in WallLayer, out ColliderCastResult result, out _))
				{
					if ( result.distance <= capsuleCollider.radius )
						DestroyCommandBuffer.Add(entity, idx);
				}


				transform.TranslateWorld(axe.Direction * axe.Speed * DeltaTime);
				transform.worldRotation =  math.mul(transform.worldTransform.rotation, quaternion.RotateX( axe.RotationSpeed * DeltaTime));
				
				
			}
		}
	}
}