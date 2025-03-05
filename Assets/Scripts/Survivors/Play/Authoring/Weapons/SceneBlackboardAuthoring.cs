using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Survivors.Play.Authoring.Weapons
{
	public class SceneBlackboardAuthoring : MonoBehaviour
	{
		[Header("Axe Config")]
		[SerializeField] float speed;
		[SerializeField] float rotationSpeed;
		
		
		private class AxeAuthoringBaker : Baker<SceneBlackboardAuthoring>
		{
			public override void Bake(SceneBlackboardAuthoring authoring)
			{
				var entity = GetEntity(TransformUsageFlags.Dynamic);
				AddComponent(entity, new AxeConfigComponent
				{
					Speed         = authoring.speed,
					RotationSpeed = authoring.rotationSpeed,
				});
				
				AddComponent<SceneMouse>(entity);
				AddComponent<PlayerPosition>(entity);
			}
		}
	}

	public struct AxeComponent : IComponentData
	{
		public float Speed;
		public float RotationSpeed;
		public float3 Direction;
	}
	
	public struct AxeConfigComponent : IComponentData
	{
		public float Speed;
		public float RotationSpeed;
	}
	
	public struct SceneMouse : IComponentData
	{
		public float2 Position;
		public float2 WorldPosition;
	}
	
	public struct PlayerPosition : IComponentData
	{
		public float3 Position;
	}
}