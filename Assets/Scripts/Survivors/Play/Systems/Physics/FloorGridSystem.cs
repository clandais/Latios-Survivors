using Latios;
using Latios.Transforms;
using Survivors.Play.Authoring.Level;
using Survivors.Play.Authoring.Weapons;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;

namespace Survivors.Play.Systems
{
    [RequireMatchingQueriesForUpdate]
    [BurstCompile]
    public partial struct FloorGridSystem : ISystem
    {
        LatiosWorldUnmanaged m_world;
        EntityQuery m_floorQuery;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            m_world = state.GetLatiosWorldUnmanaged();
           // state.RequireForUpdate<LevelAABB>();
           m_floorQuery = state.Fluent().With<LevelAABB>()
               .Build();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            state.Enabled = false;

            var aabb = m_world.sceneBlackboardEntity.GetComponentData<LevelAABB>().AABB;
            var floorCollisionLayer = m_world.sceneBlackboardEntity.GetCollectionComponent<FloorClollisionLayer>();
            var wallsCollisionLayer = m_world.sceneBlackboardEntity.GetCollectionComponent<EnvironmentCollisionLayer>();

            var grid = m_world.sceneBlackboardEntity.GetCollectionComponent<FloorGrid>();


            grid.MinX = (int)math.floor(aabb.min.x);
            grid.MinY = (int)math.floor(aabb.min.z);
            grid.MaxX = (int)math.ceil(aabb.max.x);
            grid.MaxY = (int)math.ceil(aabb.max.z);

            grid.Width  = (grid.MaxX - grid.MinX) / grid.CellSize;
            grid.Height = (grid.MaxY - grid.MinY) / grid.CellSize;


            grid.Walkable    = new NativeArray<bool>(grid.Width * grid.Height, Allocator.Persistent);
            grid.VectorField = new NativeArray<float2>(grid.Width * grid.Height, Allocator.Persistent);


            foreach (var bounds in SystemAPI.Query<RefRO<WorldRenderBounds>>()
                         .WithAll<FloorTileTag>())
            {
                var b = bounds.ValueRO.Value;

                var min =  grid.WorldToCell(math.floor(b.Min.xz));
                var max = grid.WorldToCell(math.ceil( b.Max.xz));


                for (var y = min.y; y <= max.y; y++)
                {
                    for (var x = min.x; x <= max.x; x++)
                    {
                        var cell = grid.IndexFromCell(new int2(x, y));
                        grid.Walkable[cell] = true;
                    }
                }
            }

            foreach (var bounds in SystemAPI.Query<RefRO<WorldRenderBounds>>()
                         .WithAll<WallTileTag>())
            {
                var b = bounds.ValueRO.Value;
            
                var min =  grid.WorldToCell(math.floor(b.Min.xz));
                var max = grid.WorldToCell(math.ceil( b.Max.xz));
            
                for (var y = min.y; y <= max.y; y++)
                {
                    for (var x = min.x; x <= max.x; x++)
                    {
                        var cell = grid.IndexFromCell(new int2(x, y));
                        grid.Walkable[cell] = false;
                    }
                }
            }

            // float halfCellSize = grid.CellSize / 2f;
            //
            // var boxCollider = new BoxCollider
            // {
            // 	center = float3.zero,
            // 	halfSize = new float3(halfCellSize + .1f, halfCellSize, halfCellSize + .1f)
            // };
            //
            //
            //
            // foreach (var result in Physics.FindObjects(aabb, floorCollisionLayer.Layer))
            // {
            // 	var resAabb = result.aabb;
            //
            // 	var min = new int2((int)math.floor(resAabb.min.x), (int)math.floor(resAabb.min.z));
            // 	var max = new int2((int)math.ceil(resAabb.max.x), (int)math.ceil(resAabb.max.z));
            //
            // 	for (int x = min.x; x < max.x; x += grid.CellSize)
            // 	{
            // 		for (int y = min.y; y < max.y; y += grid.CellSize)
            // 		{
            //
            // 			var index = grid.IndexFromWorld(new float2(x, y));
            //
            // 			grid.Walkable[index] = !Physics.ColliderCast(
            // 				boxCollider,
            // 				new TransformQvvs
            // 				{
            // 					rotation = quaternion.identity,
            // 					position = new float3(x, 10f, y),
            // 					scale    = 1f
            // 				}, new float3(x, -10f, y), wallsCollisionLayer.Layer, out _, out _);
            // 		}
            // 	}
            //
            // }

            m_world.sceneBlackboardEntity.SetCollectionComponentAndDisposeOld(grid);
        }


        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {
        }
    }
}