using Unity.Entities;
using UnityEngine;

namespace Survivors.Play.Authoring
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
}