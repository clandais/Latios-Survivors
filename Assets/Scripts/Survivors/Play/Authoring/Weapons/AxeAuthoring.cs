using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Survivors.Play.Authoring.Weapons
{
	public class AxeAuthoring : MonoBehaviour
	{
		[SerializeField] float speed;
		[SerializeField] float rotationSpeed;
		
		
		private class AxeAuthoringBaker : Baker<AxeAuthoring>
		{
			public override void Bake(AxeAuthoring authoring)
			{
				var entity = GetEntity(TransformUsageFlags.Dynamic);
				AddComponent(entity, new AxeComponent
				{
					Speed         = authoring.speed,
					RotationSpeed = authoring.rotationSpeed,
				});
			}
		}
	}

	public struct AxeComponent : IComponentData
	{
		public float Speed;
		public float RotationSpeed;
		public float3 Direction;
	}
}