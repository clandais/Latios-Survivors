using Latios;
using Latios.Transforms;
using Survivors.Play.Authoring.Level;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;

namespace Survivors.Play.Systems
{
	[RequireMatchingQueriesForUpdate]
	public partial struct FloorGridSystem : ISystem, ISystemNewScene
	{
		
		const int k_cellSize = 4;
		
		LatiosWorldUnmanaged m_world;
		EntityQuery m_floorQuery;
		
		[BurstCompile]
		public void OnCreate(ref SystemState state)
		{
			m_world = state.GetLatiosWorldUnmanaged();
			m_floorQuery = state.Fluent().With<FloorTag>(true).Build();
		}

		[BurstCompile]
		public void OnUpdate(ref SystemState state)
		{

			state.Enabled = false;
			
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
			
			grid.Height = maxY - minY + 1;
			grid.Width = maxX - minX + 1;
			grid.Walkable = new NativeArray<bool>(grid.Width * grid.Height, Allocator.Persistent);
			

			foreach (var transform in SystemAPI.Query<RefRO<WorldTransform>>().WithAll<FloorTag>())
			{
				float2 worldPos = transform.ValueRO.position.xz;
				int2 gridPos = new int2((int)math.floor((worldPos.x - minX ) / k_cellSize), (int)math.floor((worldPos.y - minY ) / k_cellSize));

				grid.Walkable[gridPos.x + gridPos.y * grid.Width] = true;
			}
			
			m_world.sceneBlackboardEntity.SetCollectionComponentAndDisposeOld(grid);
			
			
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