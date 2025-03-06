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
		public NativeArray<bool> Walkable;
		
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