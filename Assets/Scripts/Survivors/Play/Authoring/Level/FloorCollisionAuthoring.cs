using Unity.Entities;
using UnityEngine;

namespace Survivors.Play.Authoring.Level
{
	
	[DisallowMultipleComponent]
	[AddComponentMenu("Survivors/Level/Floor")]
	public class FloorCollisionAuthoring : MonoBehaviour
	{
		private class FloorCollisionAuthoringBaker : Baker<FloorCollisionAuthoring>
		{
			public override void Bake(FloorCollisionAuthoring authoring)
			{
				var entity = GetEntity(TransformUsageFlags.Renderable);
				AddComponent<FloorTag>(entity);
			}
		}
	}

	public struct FloorTag : IComponentData { }
}