using Latios;
using Latios.Transforms;
using Survivors.Play.Authoring.Level;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Survivors.Play.Systems
{

	[RequireMatchingQueriesForUpdate]
	public partial struct FloorGridSystem : ISystem, ISystemNewScene
	{
		

		LatiosWorldUnmanaged m_world;
		EntityQuery m_floorQuery;

		[BurstCompile]
		public void OnCreate(ref SystemState state)
		{
			m_world      = state.GetLatiosWorldUnmanaged();
			m_floorQuery = state.Fluent().With<FloorTag>(true).Build();
		}

		[BurstCompile]
		public void OnUpdate(ref SystemState state)
		{

			//	state.Enabled = false;

			var grid = m_world.sceneBlackboardEntity.GetCollectionComponent<FloorGrid>();


			int minX = int.MaxValue;
			int minY = int.MaxValue;
			int maxX = int.MinValue;
			int maxY = int.MinValue;


			foreach (var transform in SystemAPI.Query<RefRO<WorldTransform>>().WithAll<FloorTag>())
			{
				float2 worldPos = transform.ValueRO.position.xz;

				if (worldPos.x < minX) minX = (int)worldPos.x;
				if (worldPos.y < minY) minY = (int)worldPos.y;
				if (worldPos.x > maxX) maxX = (int)worldPos.x;
				if (worldPos.y > maxY) maxY = (int)worldPos.y;
			}


			grid.MinX = minX;
			grid.MinY = minY;
			grid.MaxX = maxX;
			grid.MaxY = maxY;

			grid.Height   = maxY - minY + 1;
			grid.Width    = maxX - minX + 1;
			grid.Walkable = new NativeArray<bool>(grid.CellCount, Allocator.Persistent);


			foreach (var transform in SystemAPI.Query<RefRO<WorldTransform>>().WithAll<FloorTag>())
			{
				float2 worldPos = transform.ValueRO.position.xz;
				var    gridPos  = new int2((int)((worldPos.x - minX) / grid.CellSize), (int)((worldPos.y - minY) / grid.CellSize));

				grid.Walkable[gridPos.x + gridPos.y * grid.Width] = true;
			}

			m_world.sceneBlackboardEntity.SetCollectionComponentAndDisposeOld(grid);


			for (int i = 0; i < grid.Walkable.Length; i++)
			{
				Color color = grid.Walkable[i] ? Color.green : Color.red;


				float2 cell = grid.CellToWorld(new int2(i % grid.Width , i / grid.Width));

				UnityEngine.Debug.DrawLine(
					new float3(cell.x - grid.CellSize/2f, 0f, cell.y - grid.CellSize/2f),
					new float3(cell.x + grid.CellSize / 2f, 0f, cell.y +  grid.CellSize / 2f),
					color);

				//UnityEngine.Debug.DrawLine(new float3(i % grid.Width, 0, i / grid.Width), new float3(i % grid.Width, 10, i / grid.Width), color);
			}

		}
	

		[BurstCompile]
		public void OnDestroy(ref SystemState state)
		{

		}

		public void OnNewScene(ref SystemState state)
		{
			m_world.sceneBlackboardEntity.AddOrSetCollectionComponentAndDisposeOld<FloorGrid>(default);
		}
	}


}