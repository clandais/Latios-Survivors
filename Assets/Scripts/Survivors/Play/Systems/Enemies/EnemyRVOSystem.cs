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
    [RequireMatchingQueriesForUpdate]
    public partial struct EnemyRVOSystem : ISystem
    {
        LatiosWorldUnmanaged m_world;
        EntityQuery m_query;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            m_world = state.GetLatiosWorldUnmanaged();
            m_query = state.Fluent().With<EnemyTag>().Build();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var playerPositionComponent = m_world.sceneBlackboardEntity.GetComponentData<PlayerPosition>();
            var enemyLayer = m_world.sceneBlackboardEntity.GetCollectionComponent<EnemyCollisionLayer>();
            
        //    var pairStream = new PairStream(enemyLayer.Layer, state.WorldUpdateAllocator);
            // var findPairProcessor = new MonsterVsMonsterRvoFindPairProcessor
            // {
            //     MotionLookup = SystemAPI.GetComponentLookup<MotionComponent>(false),
            //     AgentSettingsLookup = SystemAPI.GetComponentLookup<AgentSettings>(true),
            //     DeltaTime = SystemAPI.Time.DeltaTime,
            // };
            //
            // state.Dependency = Physics.FindPairs(enemyLayer.Layer, findPairProcessor).ScheduleParallel(state.Dependency);
            
            
            
            state.Dependency = new FollowPlayerRvoJob
            {
                DeltaTime           = SystemAPI.Time.DeltaTime,
                PlayerPosition      = playerPositionComponent.Position,
                CollisionLayer      = enemyLayer.Layer,
                MotionLookup        = SystemAPI.GetComponentLookup<AgentVelocityComponent>(true),
                TransformLookup     = SystemAPI.GetComponentLookup<WorldTransform>(true),
                AgentSettingsLookup = SystemAPI.GetComponentLookup<AgentSettings>(true),
            }.ScheduleParallel(state.Dependency);
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {

        }
    }
    

    [WithAll(typeof(EnemyTag))]
    [WithNone(typeof(DeadTag))]
    [BurstCompile]
    internal partial struct FollowPlayerRvoJob : IJobEntity
    {
        [ReadOnly] public float DeltaTime;
        [ReadOnly] public float3 PlayerPosition;
        [ReadOnly] public CollisionLayer CollisionLayer;
        [ReadOnly] public ComponentLookup<AgentVelocityComponent> MotionLookup;
        [ReadOnly] public ComponentLookup<WorldTransform> TransformLookup;
        [ReadOnly] public ComponentLookup<AgentSettings> AgentSettingsLookup;

        public void Execute(Entity entity, in AgentSettings agentSettings, ref MotionComponent motion)
        {

            
            
            motion.Rotation = TransformLookup[entity].worldTransform.rotation;
            

            float2 desiredVelocity = math.normalizesafe(
                PlayerPosition.xz -  TransformLookup[entity].position.xz); 
            float  desiredSpeed    = agentSettings.RunSpeed;
            

            motion.DesiredVelocity = new float3(desiredVelocity.x, 0, desiredVelocity.y);
            

            var searchRegion = new Aabb
            {
                min = TransformLookup[entity].position - agentSettings.ObstacleHorizon, max = TransformLookup[entity].position + agentSettings.ObstacleHorizon,
            };

            foreach (var candidate in Physics.FindObjects(searchRegion, CollisionLayer))
            {
                if (candidate.entity == entity) continue;
                
                var candidateMotion = MotionLookup[candidate.entity];
                var candidateTransform = TransformLookup[candidate.entity];
                
                var relativePosition = candidateTransform.position.xz - TransformLookup[entity].position.xz;
                var relativeVelocity = motion.Velocity.xz - candidateMotion.Velocity.xz;
                float combinedRadius = agentSettings.Radius + AgentSettingsLookup[candidate.entity].Radius;
                float distance = math.length(relativePosition);
                float timeToCollision = (distance - combinedRadius) / math.length(relativeVelocity);

                if (timeToCollision < agentSettings.ObstacleHorizon)
                {
                    float2 collisionNormal = math.normalizesafe(relativePosition);
                    float2 avoidanceVelocity = collisionNormal * (combinedRadius / timeToCollision);
                    motion.AvoidanceVelocity += new float3(avoidanceVelocity.x, 0, avoidanceVelocity.y);
                }
            }
			
            
            UnityEngine.Debug.DrawLine(TransformLookup[entity].position, TransformLookup[entity].position + motion.AvoidanceVelocity, UnityEngine.Color.red);
			


            motion.DesiredVelocity += motion.AvoidanceVelocity;
            motion.DesiredVelocity =  math.normalizesafe(motion.DesiredVelocity) * desiredSpeed;

            motion.AvoidanceVelocity = float3.zero;
            
            motion.Velocity = motion.Velocity.MoveTowards(motion.DesiredVelocity, 
                agentSettings.VelocityChange * DeltaTime);


            if (math.lengthsq(motion.Velocity) > 0f)
            {

                quaternion lookRotation = quaternion.LookRotation(new float3(desiredVelocity.x, 0f, desiredVelocity.y), math.up());
                motion.DesiredRotation = lookRotation;
                motion.Rotation        = TransformLookup[entity].rotation.RotateTowards(motion.DesiredRotation, 90f * DeltaTime);
            }
        }
    }
}
