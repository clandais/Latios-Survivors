using Unity.Entities;
using UnityEngine;

namespace Survivors.Play.Authoring.Level
{
	
	[DisallowMultipleComponent]
	[AddComponentMenu("Survivors/Level/Floor")]
	public class FloorLevelAuthoring : MonoBehaviour
	{
		private class FloorLevelAuthoringBaker : Baker<FloorLevelAuthoring>
		{
			public override void Bake(FloorLevelAuthoring authoring)
			{
				var entity = GetEntity(TransformUsageFlags.Renderable);
				AddComponent<FloorTag>(entity);
			}
		}
	}

	public struct FloorTag : IComponentData { }
}