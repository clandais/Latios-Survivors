using Latios;
using Survivors.Play.Authoring.Level;
using Survivors.Play.Authoring.Weapons;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;

namespace Survivors.Play.Systems
{
    [RequireMatchingQueriesForUpdate]
    [BurstCompile]
    public partial struct VectorFieldSystem : ISystem
    {
        LatiosWorldUnmanaged m_world;
        EntityQuery m_floorQuery;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            m_world = state.GetLatiosWorldUnmanaged();
            state.RequireForUpdate<LevelAABB>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var grid = m_world.sceneBlackboardEntity.GetCollectionComponent<FloorGrid>();
            FloorGrid.Draw(grid);
            
            var playerPos = m_world.sceneBlackboardEntity.GetComponentData<PlayerPosition>();

            var lastPosition = grid.WorldToCell(playerPos.LastPosition.xz);
            var targetPos = grid.WorldToCell(playerPos.Position.xz);

            if (math.distance(lastPosition, targetPos) < 1f) return;

            var vectorFieldJob = new BuildVectorFieldJob
            {
                TargetPos = targetPos,
                Grid      = grid
            };

            state.Dependency = vectorFieldJob.Schedule(state.Dependency);


            // m_world.sceneBlackboardEntity.SetCollectionComponentAndDisposeOld(grid);
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {
        }
    }


    [BurstCompile]
    internal struct BuildVectorFieldJob : IJob
    {
        [ReadOnly] public int2 TargetPos;
        public FloorGrid Grid;


        public void Execute()
        {
            var directions = new NativeArray<int2>(8, Allocator.Temp);

            directions[0] = new int2(-1, 0);
            directions[1] = new int2(1, 0);
            directions[2] = new int2(0, -1);
            directions[3] = new int2(0, 1);
            directions[4] = new int2(-1, -1);
            directions[5] = new int2(1, -1);
            directions[6] = new int2(-1, 1);
            directions[7] = new int2(1, 1);
            


            var q = new NativeQueue<int>(Allocator.Temp);
            var reached = new NativeList<int>(Grid.CellCount, Allocator.Temp);

            var idx = Grid.CellToIndex(TargetPos);
            q.Enqueue(idx);
            reached.Add(idx);

            while (q.Count > 0)
            {
                var currentIdx = q.Dequeue();
                var currentCell = Grid.IndexToCell(currentIdx);

                foreach (var direction in directions)
                {
                    var neighborCell = currentCell + direction;
                    var neighborIndex = Grid.CellToIndex(neighborCell);

                    if (neighborIndex < 0 || neighborIndex >= Grid.CellCount || reached.Contains(neighborIndex) || !Grid.Walkable[neighborIndex])
                        continue;
                        

                    Grid.VectorField[neighborIndex] = math.normalize( currentCell - neighborCell);
                    q.Enqueue(neighborIndex);
                    reached.Add(neighborIndex);
                }
            }

            q.Dispose();
            reached.Dispose();
        }
    }
}