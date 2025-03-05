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

	public struct SteeringForces : IComponentData
	{
		public float2 Separation;
		public float2 Alignment;
		public float2 Cohesion;
		public int Count;
	}

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
				MotionLookup            = SystemAPI.GetComponentLookup<SteeringForces>(),
				MotionComponentLookup   = SystemAPI.GetComponentLookup<MotionComponent>(true),
				SteeringComponentLookup = SystemAPI.GetComponentLookup<SteeringComponent>(true)
			};

			state.Dependency = Physics.FindPairs(enemyLayer.Layer, findPairProcessor).ScheduleParallel(state.Dependency);


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
		public PhysicsComponentLookup<SteeringForces> MotionLookup;
		[ReadOnly] public ComponentLookup<MotionComponent> MotionComponentLookup;
		[ReadOnly] public ComponentLookup<SteeringComponent> SteeringComponentLookup;

		public void Execute(in FindPairsResult result)
		{
			float radius = SteeringComponentLookup[result.entityA].Radius;

			if (Physics.DistanceBetween(result.colliderA, result.transformA, result.colliderB, result.transformB, radius, out ColliderDistanceResult distanceResult))
			{
				ref float2 sep  = ref PairStream.AddPairAndGetRef<float2>(result.pairStreamKey, false, true, out _);
				ref float2 sepB = ref PairStream.AddPairAndGetRef<float2>(result.pairStreamKey, true, false, out _);
				ref float2 vel  = ref PairStream.AddPairAndGetRef<float2>(result.pairStreamKey, false, true, out _);
				ref float2 velB = ref PairStream.AddPairAndGetRef<float2>(result.pairStreamKey, true, false, out _);
				ref float2 coh  = ref PairStream.AddPairAndGetRef<float2>(result.pairStreamKey, false, true, out _);
				ref float2 cohB = ref PairStream.AddPairAndGetRef<float2>(result.pairStreamKey, true, false, out _);

				sep  += (result.transformA.position.xz - result.transformB.position.xz) / math.pow(distanceResult.distance, 2);
				sepB += (result.transformB.position.xz - result.transformA.position.xz) / math.pow(distanceResult.distance, 2);

				vel  += MotionComponentLookup[result.entityB].Velocity.xz;
				velB += MotionComponentLookup[result.entityA].Velocity.xz;

				if (distanceResult.distance < SteeringComponentLookup[result.entityA].CohesionRadius)
					coh += result.transformB.position.xz;

				if (distanceResult.distance < SteeringComponentLookup[result.entityB].CohesionRadius)
					cohB += result.transformA.position.xz;

				var motionA = MotionLookup.GetRW(result.entityA);
				motionA.ValueRW.Separation = sep;
				motionA.ValueRW.Alignment  = vel;
				motionA.ValueRW.Cohesion   = coh;
				motionA.ValueRW.Count++;

				var motionB = MotionLookup.GetRW(result.entityB);
				motionB.ValueRW.Separation = sepB;
				motionB.ValueRW.Alignment  = velB;
				motionB.ValueRW.Cohesion   = cohB;
				motionB.ValueRW.Count++;

			}

		}
	}


	internal struct MonsterVsMonsterForEachPairProcessor : IForEachPairProcessor
	{
		public PhysicsComponentLookup<SteeringForces> MotionLookup;
		public PairStream.ParallelWriter PairStream;

		public void Execute(ref PairStream.Pair pair)
		{
			if (pair.userByte == 0)
			{
				SteeringForces motionA = MotionLookup.GetRW(pair.entityA).ValueRW;
				motionA.Separation /= motionA.Count;
			}
			// else if (pair.userByte == 1)
			// {
			// 	var motionB = MotionLookup.GetRW(pair.entityB).ValueRW;
			// 	motionB.Separation /= motionB.Count;
			// }
		}
	}

	[WithAll(typeof(EnemyTag))]
	[WithNone(typeof(DeadTag))]
	[BurstCompile]
	internal partial struct FollowPlayerJob : IJobEntity
	{
		[ReadOnly] public float DeltaTime;
		[ReadOnly] public float3 PlayerPosition;

		public void Execute(TransformAspect transformAspect, AgentMotionAspect motionAspect, in SteeringComponent steeringComponent, ref SteeringForces forces)
		{
			float2 dir          = math.normalize(PlayerPosition.xz - transformAspect.worldPosition.xz);
			float  desiredSpeed = motionAspect.SpeedSettings.RunSpeed;

			float2 desiredVelocity = dir;

			if (forces.Count > 0)
			{
				desiredVelocity *= steeringComponent.VelocityWeight;
				desiredVelocity += forces.Separation * steeringComponent.SeparationWeight;
				desiredVelocity += forces.Alignment / forces.Count * steeringComponent.AlignmentWeight;
				desiredVelocity += forces.Cohesion / forces.Count * steeringComponent.CohesionWeight;
			}

			forces.Separation = float2.zero;
			forces.Alignment  = float2.zero;
			forces.Cohesion   = float2.zero;
			forces.Count      = 0;

			motionAspect.DesiredVelocity = new float3(desiredVelocity.x, 0, desiredVelocity.y);
			motionAspect.DesiredVelocity = math.normalizesafe(motionAspect.DesiredVelocity) * desiredSpeed;


			motionAspect.Velocity = motionAspect.Velocity.MoveTowards(motionAspect.DesiredVelocity, motionAspect.SpeedSettings.VelocityChange * DeltaTime);

			float3     lookDir      = motionAspect.DesiredVelocity;
			quaternion lookRotation = quaternion.LookRotation(lookDir, math.up());
			motionAspect.DesiredRotation = lookRotation;
			motionAspect.Rotation        = transformAspect.worldRotation.RotateTowards(motionAspect.DesiredRotation, 90f * DeltaTime);
		}
	}
}