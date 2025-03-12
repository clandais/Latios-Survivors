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
	public partial struct AxeUpdateSystem : ISystem, ISystemNewScene
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
			var ecb = m_latiosWorldUnmanaged.syncPoint.CreateEntityCommandBuffer( );
			var icb = m_latiosWorldUnmanaged.syncPoint.CreateInstantiateCommandBuffer<WorldTransform>( );
			CollisionLayer collisionLayer = m_latiosWorldUnmanaged.sceneBlackboardEntity
				.GetCollectionComponent<EnvironmentCollisionLayer>().Layer;
			CollisionLayer enemyLayer = m_latiosWorldUnmanaged.sceneBlackboardEntity
				.GetCollectionComponent<EnemyCollisionLayer>().Layer;

			var rng = state.GetMainThreadRng();
			
			var hitEntities = new NativeList<Entity>(Allocator.TempJob);
			var destroyedAxesPositions = new NativeList<float3>(Allocator.TempJob);
			
			var sfxSpawnQueue = m_latiosWorldUnmanaged.sceneBlackboardEntity.GetCollectionComponent<SFXSpawnQueue>();

		
			state.Dependency = new AxeMovementJob
			{
				
				DeltaTime = SystemAPI.Time.DeltaTime,
				DestroyCommandBuffer = dcb.AsParallelWriter(),
				Icb = icb.AsParallelWriter(),
				WallLayer = collisionLayer,
				EnemyLayer = enemyLayer,
				DestoryedAxesPositions = destroyedAxesPositions,
				HitEntities = hitEntities,
			}.ScheduleParallel(state.Dependency);

			
			state.Dependency.Complete();


			foreach (float3 destroyedAxesPosition in destroyedAxesPositions)
			{
				var axeClashSfx = m_latiosWorldUnmanaged.sceneBlackboardEntity.GetBuffer<AxeClashSfxBufferElement>();
				sfxSpawnQueue.SFXSQueue.Enqueue( new SFXSpawnQueue.SFXSpawnData
				{
					SFXPrefab = axeClashSfx[rng.NextInt(0, axeClashSfx.Length)].ClashSfxPrefab,
					Position  = destroyedAxesPosition
				});
			}

			destroyedAxesPositions.Dispose();


			foreach (var entity in hitEntities)
			{
				
				var axeHitSfx = m_latiosWorldUnmanaged.sceneBlackboardEntity.GetBuffer<AxeHitSfxBufferElement>();
				sfxSpawnQueue.SFXSQueue.Enqueue(  new SFXSpawnQueue.SFXSpawnData
				{
					SFXPrefab = axeHitSfx[rng.NextInt(0, axeHitSfx.Length)].HitSfxPrefab,
					Position = state.EntityManager.GetComponentData<WorldTransform>(entity).position
				});
				
			
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
			public InstantiateCommandBuffer<WorldTransform>.ParallelWriter Icb;
			[NativeDisableParallelForRestriction] public NativeList<Entity> HitEntities;
			[NativeDisableParallelForRestriction] public NativeList<float3> DestoryedAxesPositions;
			[ReadOnly] public float DeltaTime;
			[ReadOnly] public CollisionLayer WallLayer;
			[ReadOnly] public CollisionLayer EnemyLayer;


			public void Execute(Entity entity, [EntityIndexInQuery] int idx,  
				ref WorldTransform transform, 
				in AxeComponent axe, 
				in Collider collider,
				in AxeDestroyVfx axeDestroyVfx)
			{
				
				TransformQvvs transformQvs = transform.worldTransform;

				CapsuleCollider capsuleCollider = collider;
				
				
				if (Physics.ColliderCast(in collider, in transformQvs, transform.position + axe.Direction, in EnemyLayer, out ColliderCastResult hitInfos, out var o))
				{
					 if ( hitInfos.distance <= capsuleCollider.radius )
						HitEntities.Add(o.entity);
				}

				if (Physics.ColliderCast(in collider, in transformQvs, transform.position + axe.Direction, in WallLayer, out ColliderCastResult result, out _))
				{
					if (result.distance <= capsuleCollider.radius)
					{
						DestoryedAxesPositions.Add(transform.position);
						Icb.Add(axeDestroyVfx.Prefab, transform, idx);
						DestroyCommandBuffer.Add(entity, idx);
					}
				}


				transform.worldTransform.position = transform.position + (axe.Direction * axe.Speed * DeltaTime);
				transform.worldTransform.rotation =  math.mul(transform.rotation, quaternion.RotateX( axe.RotationSpeed * DeltaTime));
				
				
			}
		}

		public void OnNewScene(ref SystemState state)
		{
			state.InitSystemRng("AxeUpdateSystem");
		}
	}
}