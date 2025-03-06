using Latios;
using Latios.Psyshock;
using Latios.Transforms;
using Survivors.Play.Authoring.Enemies;
using Survivors.Play.Authoring.Weapons;
using Survivors.Play.Components;
using Survivors.Play.Utilities;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace Survivors.Play.Systems.Enemies
{
	
	public partial struct EnemyFollowDesiredMotionSystem : ISystem
	{

		LatiosWorldUnmanaged m_world;

		[BurstCompile]
		public void OnCreate(ref SystemState state)
		{
			state.RequireForUpdate<EnemyTag>();
			m_world = state.GetLatiosWorldUnmanaged();
		}

		[BurstCompile]
		public void OnUpdate(ref SystemState state)
		{
			var playerPositionComponent = m_world.sceneBlackboardEntity.GetComponentData<PlayerPosition>();

			var enemyLayer = m_world.sceneBlackboardEntity.GetCollectionComponent<EnemyCollisionLayer>();

			var pairStream = new PairStream(
				enemyLayer.Layer,
				state.WorldUpdateAllocator);


			var findPairProcessor = new MonsterVsMonsterFindPairProcessor
			{
				PairStream              = pairStream.AsParallelWriter(),
				AlignmentLookup         = SystemAPI.GetComponentLookup<AlignmentForce>(),
				CohesionLookup          = SystemAPI.GetComponentLookup<CohesionForce>(),
				SeparationLookup        = SystemAPI.GetComponentLookup<SeparationForce>(),
				MotionComponentLookup   = SystemAPI.GetComponentLookup<MotionComponent>(true),
				SteeringComponentLookup = SystemAPI.GetComponentLookup<SteeringComponent>(true)
			};

			state.Dependency = Physics.FindPairs(enemyLayer.Layer, findPairProcessor).ScheduleParallel(state.Dependency);

			var monsterForEachProcessor = new MonsterForEachProcessor
			{
				AlignmentLookup         = SystemAPI.GetComponentLookup<AlignmentForce>(),
				CohesionLookup          = SystemAPI.GetComponentLookup<CohesionForce>(),
				SeparationLookup        = SystemAPI.GetComponentLookup<SeparationForce>(),
				MotionComponentLookup   = SystemAPI.GetComponentLookup<MotionComponent>(true),
				SteeringComponentLookup = SystemAPI.GetComponentLookup<SteeringComponent>(true),
				TransformLookup         = SystemAPI.GetComponentLookup<WorldTransform>(true)
			};

			state.Dependency = Physics.ForEachPair(pairStream, monsterForEachProcessor).ScheduleParallel(state.Dependency);

			state.Dependency = new FollowPlayerJob
			{
				DeltaTime      = SystemAPI.Time.DeltaTime,
				PlayerPosition = playerPositionComponent.Position
			}.ScheduleParallel(state.Dependency);
		}

		[BurstCompile]
		public void OnDestroy(ref SystemState state)
		{

		}
	}

	internal struct MonsterVsMonsterFindPairProcessor : IFindPairsProcessor
	{

		public PairStream.ParallelWriter PairStream;
		public PhysicsComponentLookup<SeparationForce> SeparationLookup;
		public PhysicsComponentLookup<AlignmentForce> AlignmentLookup;
		public PhysicsComponentLookup<CohesionForce> CohesionLookup;

		[ReadOnly] public ComponentLookup<MotionComponent> MotionComponentLookup;
		[ReadOnly] public ComponentLookup<SteeringComponent> SteeringComponentLookup;

		public void Execute(in FindPairsResult result)
		{
			float viewRadius = SteeringComponentLookup[result.entityA].ViewRadius;

			if (Physics.DistanceBetween(result.colliderA, result.transformA, result.colliderB, result.transformB, viewRadius, out ColliderDistanceResult distanceResult))
			{

				PairStream.AddPairAndGetRef<int>(result.pairStreamKey, true, false, out _);

				var alignment  = AlignmentLookup.GetRW(result.entityA);
				var cohesion   = CohesionLookup.GetRW(result.entityA);
				var separation = SeparationLookup.GetRW(result.entityA);

				alignment.ValueRW.Force += MotionComponentLookup[result.entityB].Velocity.xz;
				alignment.ValueRW.Count++;

				cohesion.ValueRW.Force += result.transformB.position.xz;
				cohesion.ValueRW.Count++;

				if (distanceResult.distance < SteeringComponentLookup[result.entityA].SeparationRadius)
					separation.ValueRW.Force += (result.transformA.position.xz - result.transformB.position.xz) / math.pow(distanceResult.distance, 2);
			}

		}
	}


	internal struct MonsterForEachProcessor : IForEachPairProcessor
	{
		public PhysicsComponentLookup<SeparationForce> SeparationLookup;
		public PhysicsComponentLookup<AlignmentForce> AlignmentLookup;
		public PhysicsComponentLookup<CohesionForce> CohesionLookup;

		[ReadOnly] public ComponentLookup<SteeringComponent> SteeringComponentLookup;
		[ReadOnly] public ComponentLookup<WorldTransform> TransformLookup;
		[ReadOnly] public ComponentLookup<MotionComponent> MotionComponentLookup;

		public void Execute(ref PairStream.Pair pair)
		{

			var cohesionA = CohesionLookup.GetRW(pair.entityA);
			if (cohesionA.ValueRW.Count > 0)
			{
				cohesionA.ValueRW.Force /= cohesionA.ValueRW.Count;
				cohesionA.ValueRW.Force -= TransformLookup[pair.entityA].position.xz;
				cohesionA.ValueRW.Force *= SteeringComponentLookup[pair.entityA].CohesionWeight;
			}


			var separationA = SeparationLookup.GetRW(pair.entityA);
			if (separationA.ValueRW.Count > 0)
			{
				separationA.ValueRW.Force /= separationA.ValueRW.Count;
				separationA.ValueRW.Force *= SteeringComponentLookup[pair.entityA].SeparationWeight;
			}

			var alignmentA = AlignmentLookup.GetRW(pair.entityA);
			if (alignmentA.ValueRW.Count > 0)
			{
				alignmentA.ValueRW.Force /= alignmentA.ValueRW.Count;
				alignmentA.ValueRW.Force -= MotionComponentLookup[pair.entityA].Velocity.xz;
				alignmentA.ValueRW.Force *= SteeringComponentLookup[pair.entityA].AlignmentWeight;
			}

		}
	}


	[WithAll(typeof(EnemyTag))]
	[WithNone(typeof(DeadTag))]
	[BurstCompile]
	internal partial struct FollowPlayerJob : IJobEntity
	{
		[ReadOnly] public float DeltaTime;
		[ReadOnly] public float3 PlayerPosition;

		public void Execute(TransformAspect transformAspect, SteeringAspect steeringAspect, AgentMotionAspect motionAspect, in SteeringComponent steeringComponent)
		{


			motionAspect.Rotation = transformAspect.worldRotation;

			float2 desiredVelocity = math.normalizesafe((PlayerPosition.xz - transformAspect.worldPosition.xz) * steeringComponent.VelocityWeight + steeringAspect.DesiredVelocity);
			float  desiredSpeed    = motionAspect.SpeedSettings.RunSpeed;


			steeringAspect.Clear();


			motionAspect.DesiredVelocity = new float3(desiredVelocity.x, 0, desiredVelocity.y);
			motionAspect.DesiredVelocity = math.normalizesafe(motionAspect.DesiredVelocity) * desiredSpeed;


			motionAspect.Velocity = motionAspect.Velocity.MoveTowards(motionAspect.DesiredVelocity, motionAspect.SpeedSettings.VelocityChange * DeltaTime);


			if (math.lengthsq(motionAspect.Velocity) > 0f)
			{

				quaternion lookRotation = quaternion.LookRotation(motionAspect.DesiredVelocity, math.up());
				motionAspect.DesiredRotation = lookRotation;
				motionAspect.Rotation        = transformAspect.worldRotation.RotateTowards(motionAspect.DesiredRotation, 45f * DeltaTime);
			}
		}
	}
}