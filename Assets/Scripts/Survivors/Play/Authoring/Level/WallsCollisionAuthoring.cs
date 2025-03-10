using Latios;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

namespace Survivors.Play.Authoring.Level
{
	public class WallsCollisionAuthoring : MonoBehaviour
	{
		private class WallsCollisionAuthoringBaker : Baker<WallsCollisionAuthoring>
		{
			public override void Bake(WallsCollisionAuthoring authoring)
			{
				var entity = GetEntity(TransformUsageFlags.Renderable);
				AddComponent<WallsTag>(entity);
			}
		}
	}
	
	public struct WallsTag : IComponentData { }
	
	public partial struct FloorGrid : ICollectionComponent
	{
		
		public NativeArray<bool> Walkable;
		public NativeArray<float2> VectorField;
		
		public int Width;
		public int Height;

		public int CellSize => 2;
		public int CellCount => Width * Height;
		public int2 CellSize2D => new int2(CellSize, CellSize);
		


		public int MinX;
		public int MinY;
		public int MaxX;
		public int MaxY;
		
		public int2 WorldToCell(float2 worldPos)
		{
			return new int2( (int)(worldPos.x - MinX) / CellSize, (int)(worldPos.y- MinY) / CellSize);
		}
		
		public float2 CellToWorld(int2 cellPos)
		{
			return new float2(cellPos.x * CellSize + MinX, cellPos.y * CellSize + MinY);
		}
		
		public int2 IndexToCell(int index)
		{
			return new int2(index % Width, index / Width);
		}

		public int IndexFromCell(int2 cellPos)
		{
			return cellPos.y * Width + cellPos.x;
		}
		
		public float2 IndexToWorld(int index)
		{
			return CellToWorld(IndexToCell(index));
		}

		public int IndexFromWorld(float2 worldPos)
		{
			return IndexFromCell(WorldToCell(worldPos));
		}

		public int CellToIndex(int2 cellPos)
		{
			return cellPos.x + cellPos.y * Width;
		}

		
		/// <summary>
		///  Draw the grid in the editor
		/// </summary>
		/// <param name="grid">
		/// The grid to draw
		/// </param>
		public static void Draw(FloorGrid grid)
		{
			for (int i = 0; i < grid.Walkable.Length; i++)
			{
				Color color = grid.Walkable[i] ? Color.green : Color.red;

			//	if (!grid.Walkable[i]) continue;
					

				float2 cell = grid.IndexToWorld(i);

				UnityEngine.Debug.DrawLine(
					new float3(cell.x - grid.CellSize/2f, .1f, cell.y - grid.CellSize/2f ),
					new float3(cell.x - grid.CellSize/2f, .1f, cell.y +  grid.CellSize / 2f),
					color);
				
				UnityEngine.Debug.DrawLine(
					new float3(cell.x + grid.CellSize/2f, .1f, cell.y - grid.CellSize/2f ),
					new float3(cell.x + grid.CellSize/2f, .1f, cell.y +  grid.CellSize / 2f),
					color);
				
				
				UnityEngine.Debug.DrawLine(
					new float3(cell.x - grid.CellSize/2f, .1f, cell.y - grid.CellSize/2f ),
					new float3(cell.x + grid.CellSize/2f, .1f, cell.y -  grid.CellSize / 2f),
					color);
				
				UnityEngine.Debug.DrawLine(
					new float3(cell.x - grid.CellSize/2f, .1f, cell.y + grid.CellSize/2f ),
					new float3(cell.x + grid.CellSize/2f, .1f, cell.y +  grid.CellSize / 2f),
					color);
				
				UnityEngine.Debug.DrawLine(
					new float3(cell.x, .1f, cell.y) ,
					new float3(cell.x + grid.VectorField[i].x, .1f, cell.y +  grid.VectorField[i].y),
					Color.blue);
			}
		}
		
		
		public JobHandle TryDispose(JobHandle inputDeps)
		{
			
			if (!VectorField.IsCreated) return inputDeps;
			
			return JobHandle.CombineDependencies(Walkable.Dispose(inputDeps), VectorField.Dispose(inputDeps));
		}
	}
}