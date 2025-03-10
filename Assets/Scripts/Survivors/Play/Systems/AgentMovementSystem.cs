using Latios;
using Latios.Psyshock;
using Latios.Transforms;
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
        LatiosWorldUnmanaged m_world;


        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            m_world = state.GetLatiosWorldUnmanaged();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var dt = SystemAPI.Time.DeltaTime;

            var sceneBlackboardEntity = m_world.sceneBlackboardEntity;

            var collisionLayer = sceneBlackboardEntity.GetCollectionComponent<EnvironmentCollisionLayer>().Layer;


            state.Dependency = new CollideAndSlideCharacterJob
            {
                CollisionLayer = collisionLayer,
                DeltaTime      = dt
            }.ScheduleParallel(state.Dependency);

            state.Dependency.Complete();


            foreach (var transform in SystemAPI.Query<RefRO<WorldTransform>>().WithAll<PlayerTag>())
                sceneBlackboardEntity.SetComponentData(new PlayerPosition
                {
                    LastPosition = sceneBlackboardEntity.GetComponentData<PlayerPosition>().Position,
                    Position     = transform.ValueRO.position
                });
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

        public void Execute(TransformAspect transform, AgentMotionAspect agentMotionAspect, in Collider collider)
        {
            agentMotionAspect.RvoVelocity = agentMotionAspect.Velocity;

            var motion = agentMotionAspect.Velocity * DeltaTime;
            var currentTransform = transform.worldTransform;
            var direction = math.normalizesafe(motion);
            var remainingDistance = math.length(agentMotionAspect.Velocity) * DeltaTime;

            var skinEpsilon = 0.01f;

            for (var iteration = 0; iteration < 16; iteration++)
            {
                if (remainingDistance < skinEpsilon) break;

                var end = currentTransform.position + direction * remainingDistance;

                if (Physics.ColliderCast(in collider, in currentTransform, end, in CollisionLayer, out var hitInfo,
                        out _))
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