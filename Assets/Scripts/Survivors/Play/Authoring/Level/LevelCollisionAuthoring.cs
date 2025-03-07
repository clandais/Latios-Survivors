using Latios;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

namespace Survivors.Play.Authoring.Level
{
	public class LevelCollisionAuthoring : MonoBehaviour
	{
		private class LevelCollisionAuthoringBaker : Baker<LevelCollisionAuthoring>
		{
			public override void Bake(LevelCollisionAuthoring authoring)
			{
				var entity = GetEntity(TransformUsageFlags.Renderable);
				AddComponent<LevelTag>(entity);
			}
		}
	}
	
	public struct LevelTag : IComponentData { }
	
	public partial struct FloorGrid : ICollectionComponent
	{
		public int Width;
		public int Height;

		public int CellSize => 4;
		public int CellCount => Width * Height;
		public int2 CellSize2D => new int2(CellSize, CellSize);
		
		public NativeArray<bool> Walkable;

		public int MinX;
		public int MinY;
		public int MaxX;
		public int MaxY;
		
		public int2 WorldToCell(float3 worldPos)
		{
			return new int2( (int)(worldPos.x - MinX) / CellSize, (int)(worldPos.z- MinY) / CellSize);
		}
		
		public float2 CellToWorld(int2 cellPos)
		{
			return new float2(cellPos.x * CellSize + MinX, cellPos.y * CellSize + MinY);
		}
		
		
		public JobHandle TryDispose(JobHandle inputDeps)
		{
			if (Walkable.IsCreated)
			{
				return Walkable.Dispose(inputDeps);
			}
			
			return inputDeps;
			
		}
	}
}