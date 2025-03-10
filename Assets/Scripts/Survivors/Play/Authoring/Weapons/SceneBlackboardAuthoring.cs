using System.Collections.Generic;
using Latios;
using Latios.Psyshock;
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
		[SerializeField] List<GameObject> sfxPrefabs;
		
		private class SceneBlackboardAuthoringBaker : Baker<SceneBlackboardAuthoring>
		{
			public override void Bake(SceneBlackboardAuthoring authoring)
			{
				var entity = GetEntity(TransformUsageFlags.Dynamic);
				AddComponent(entity, new AxeConfigComponent
				{
					Speed         = authoring.speed,
					RotationSpeed = authoring.rotationSpeed
				});
				
				AddComponent<SceneMouse>(entity);
				AddComponent<PlayerPosition>(entity);
				AddComponent<LevelAABB>(entity);

				var sfxBuffer = AddBuffer<AxeSfxBufferElement>(entity);
				foreach (var authoringSfxPrefab in authoring.sfxPrefabs)
				{
					sfxBuffer.Add(new AxeSfxBufferElement
					{
						SfxPrefab = GetEntity(authoringSfxPrefab, TransformUsageFlags.Dynamic),
					});
				}

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
		//public EntityWith<Prefab> SfxPrefab;
	}
	
	public struct AxeSfxBufferElement : IBufferElementData
	{
		public EntityWith<Prefab> SfxPrefab;
	}
	
	public struct SceneMouse : IComponentData
	{
		public float2 Position;
		public float2 WorldPosition;
	}
	
	public struct PlayerPosition : IComponentData
	{
		public float3 LastPosition;
		public float3 Position;
	}

	public struct LevelAABB : IComponentData
	{
		public Aabb AABB;
	}
}