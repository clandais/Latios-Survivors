using Survivors.Play.Components;
using Unity.Entities;
using UnityEngine;

namespace Survivors.Play.Authoring
{
	public class PlayerAuthoring : MonoBehaviour
	{
		private class PlayerAuthoringBaker : Baker<PlayerAuthoring>
		{
			public override void Bake(PlayerAuthoring authoring)
			{
				var entity = GetEntity(TransformUsageFlags.Dynamic);
				AddComponent<PlayerTag>(entity);
				AddComponent<PlayerInputState>(entity);
			}
		}
	}
	
	public struct PlayerTag : IComponentData { }
}