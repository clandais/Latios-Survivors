using Latios;
using Latios.Psyshock;
using Latios.Transforms;
using Survivors.Play.Authoring.Enemies;
using Survivors.Play.Authoring.Level;
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
    public partial struct EnemyRVOSystem : ISystem, ISystemNewScene
    {
        LatiosWorldUnmanaged m_world;
        EntityQuery m_query;

        Rng m_rng;

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
            var collisionLayer = m_world.sceneBlackboardEntity.GetCollectionComponent<EnvironmentCollisionLayer>();
            var grid = m_world.sceneBlackboardEntity.GetCollectionComponent<FloorGrid>();
            var enemyLayer = m_world.sceneBlackboardEntity.GetCollectionComponent<EnemyCollisionLayer>();

            state.Dependency = new FollowPlayerRvoJob
            {
                Rng                 = m_rng.Shuffle(),
                DeltaTime           = SystemAPI.Time.DeltaTime,
                Grid                = grid,
                PlayerPosition      = playerPositionComponent.Position,
                LevelCollisionLayer = collisionLayer.Layer,
                CollisionLayer      = enemyLayer.Layer,
                MotionLookup        = SystemAPI.GetComponentLookup<AgentVelocityComponent>(true),
                TransformLookup     = SystemAPI.GetComponentLookup<WorldTransform>(true),
                AgentSettingsLookup = SystemAPI.GetComponentLookup<AgentSettings>(true)
            }.ScheduleParallel(state.Dependency);
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {
        }

        public void OnNewScene(ref SystemState state)
        {
            m_rng = new Rng("EnemyRVOSystem");
        }
    }


    [WithAll(typeof(EnemyTag))]
    [WithNone(typeof(DeadTag))]
    [BurstCompile]
    internal partial struct FollowPlayerRvoJob : IJobEntity
    {
        [ReadOnly] public float DeltaTime;
        [ReadOnly] public FloorGrid Grid;
        [ReadOnly] public float3 PlayerPosition;
        [ReadOnly] public CollisionLayer LevelCollisionLayer;
        [ReadOnly] public CollisionLayer CollisionLayer;
        [ReadOnly] public ComponentLookup<AgentVelocityComponent> MotionLookup;
        [ReadOnly] public ComponentLookup<WorldTransform> TransformLookup;
        [ReadOnly] public ComponentLookup<AgentSettings> AgentSettingsLookup;
        [ReadOnly] public Rng Rng;

        [BurstCompile]
        public void Execute(
            Entity entity, [EntityIndexInQuery] int idx,
            in Collider collider,
            in AgentSettings agentSettings, ref MotionComponent motion)
        {
            var transform = TransformLookup[entity].worldTransform;

            motion.Rotation = transform.rotation;

            float2 desiredVelocity;
            
            if (Physics.ColliderCast(collider, transform,  PlayerPosition, CollisionLayer, out _, out _ ))
            {
                // if the player is not visible, follow the flow field
                
                var position = transform.position.xz;
                var cell = Grid.IndexFromWorld(position);
                
                // Follow Flow Field
                desiredVelocity = Grid.VectorField[cell];
                if (math.length(desiredVelocity) < 0.1f)
                {
                    var seq = Rng.GetSequence(idx);
                    desiredVelocity = math.normalizesafe(seq.NextFloat2());
                }

            }
            else
            {
                // Move towards the player
                desiredVelocity = math.normalizesafe(PlayerPosition.xz - transform.position.xz);
             //   UnityEngine.Debug.DrawLine( transform.position, PlayerPosition, UnityEngine.Color.blue);
            }



            
            var desiredSpeed = agentSettings.RunSpeed;
            motion.DesiredVelocity = new float3(desiredVelocity.x, 0, desiredVelocity.y);

            #region RVO

            var searchRegion = new Aabb
            {
                min = transform.position - agentSettings.ObstacleHorizon,
                max = transform.position + agentSettings.ObstacleHorizon
            };

            foreach (var candidate in Physics.FindObjects(searchRegion, CollisionLayer))
            {
                if (candidate.entity == entity) continue;

                var candidateMotion = MotionLookup[candidate.entity];
                var candidateTransform = TransformLookup[candidate.entity];

                var relativePosition = candidateTransform.position.xz - TransformLookup[entity].position.xz;
                var relativeVelocity = motion.Velocity.xz - candidateMotion.Velocity.xz;
                var combinedRadius = agentSettings.Radius + AgentSettingsLookup[candidate.entity].Radius;
                var distance = math.length(relativePosition);
                var timeToCollision = (distance - combinedRadius) / math.length(relativeVelocity);

                if (timeToCollision < agentSettings.ObstacleHorizon)
                {
                    var collisionNormal = math.normalizesafe(relativePosition);
                    var avoidanceVelocity = collisionNormal * (combinedRadius / timeToCollision);
                    motion.AvoidanceVelocity += new float3(avoidanceVelocity.x, 0, avoidanceVelocity.y);
                }
            }

            #endregion


            //   UnityEngine.Debug.DrawLine(TransformLookup[entity].position, TransformLookup[entity].position + motion.AvoidanceVelocity, UnityEngine.Color.red);


            motion.DesiredVelocity += motion.AvoidanceVelocity;
            motion.DesiredVelocity =  math.normalizesafe(motion.DesiredVelocity) * desiredSpeed;

            motion.AvoidanceVelocity = float3.zero;

            motion.Velocity = motion.Velocity.MoveTowards(motion.DesiredVelocity,
                agentSettings.VelocityChange * DeltaTime);


            if (math.lengthsq(motion.Velocity) > 0f)
            {
                var lookRotation =
                    quaternion.LookRotation(new float3(PlayerPosition.x, 0f, PlayerPosition.z) - transform.position, math.up());
                motion.DesiredRotation = lookRotation;
                motion.Rotation = transform.rotation
                    .RotateTowards(motion.DesiredRotation, 90f * DeltaTime);
            }
        }
    }
}