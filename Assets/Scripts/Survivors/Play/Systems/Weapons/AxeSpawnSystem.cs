using Latios;
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
    // [RequireMatchingQueriesForUpdate]
    public partial struct AxeSpawnSystem : ISystem
    {
        LatiosWorldUnmanaged _world;
        Rng _rng;

        public void OnCreate(ref SystemState state)
        {
            _world = state.GetLatiosWorldUnmanaged();
            _rng = new Rng(new FixedString128Bytes("AxeSpawnSystem"));
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {

            var spawnQueue = _world.worldBlackboardEntity.GetCollectionComponent<AxeSpawnQueue>();
            var axeConfig = _world.sceneBlackboardEntity.GetComponentData<AxeConfigComponent>();
            var axeSfxBuffer = _world.sceneBlackboardEntity.GetBuffer<AxeSfxBufferElement>();
            InstantiateCommandBuffer<AxeComponent, WorldTransform> icb = _world.syncPoint.CreateInstantiateCommandBuffer<AxeComponent, WorldTransform>();
            InstantiateCommandBuffer<WorldTransform> sfxIcb = _world.syncPoint.CreateInstantiateCommandBuffer<WorldTransform>();


            var positionList = new NativeList<float3>(Allocator.TempJob);


            while (!spawnQueue.AxesQueue.IsEmpty())
            {
                if (spawnQueue.AxesQueue.TryDequeue(out var axe))
                {

                    var transform = new WorldTransform
                    {
                        worldTransform = TransformQvvs.identity
                    };


                    transform.worldTransform.position = axe.Position;
                    positionList.Add(axe.Position);
                    transform.worldTransform.rotation = quaternion.LookRotation(axe.Direction, math.up());

                    icb.Add(axe.AxePrefab, new AxeComponent
                    {
                        Direction     = axe.Direction,
                        Speed         = axeConfig.Speed,
                        RotationSpeed = axeConfig.RotationSpeed
                    }, transform);

                }

            }


            state.Dependency = new AxeSfxSpanJob
            {
                Rng = _rng.Shuffle(),
                Positions = positionList,
                SfxPrefab = axeSfxBuffer,
                Icb       = sfxIcb.AsParallelWriter(),
            }.Schedule(positionList.Length, state.Dependency);
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {

        }
    }

    [BurstCompile]
    internal struct AxeSfxSpanJob : IJobFor
    {
        [ReadOnly] public NativeList<float3> Positions;
        [ReadOnly] public DynamicBuffer<AxeSfxBufferElement> SfxPrefab;
        public InstantiateCommandBuffer<WorldTransform>.ParallelWriter Icb;
        [ReadOnly] public Rng Rng;


        public void Execute(int index)
        {
            var transform = new WorldTransform
            {
                worldTransform = new TransformQvvs
                {
                    position = Positions[index],
                    rotation = quaternion.identity,
                    scale    = 1f
                }
            };

            var random = Rng.GetSequence(index);
            
            
            
            transform.worldTransform.position = Positions[index];
            Icb.Add(SfxPrefab[random.NextInt(0, SfxPrefab.Length)].SfxPrefab, transform, index);
        }
    }
}
