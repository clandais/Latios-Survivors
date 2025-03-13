using System.Collections.Generic;
using Latios;
using Latios.Psyshock;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Serialization;

namespace Survivors.Play.Authoring.Weapons
{
	public class SceneBlackboardAuthoring : MonoBehaviour
	{
		[Header("Axe Config")]
		[SerializeField] float speed;
		[SerializeField] float rotationSpeed;
		[FormerlySerializedAs("sfxPrefabs")] [SerializeField] List<GameObject> wooshSfxPrefabs;
		[SerializeField] List<GameObject> hitSfxPrefabs;
		[SerializeField] List<GameObject> clashSfxPrefabs;
		
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
				foreach (var authoringSfxPrefab in authoring.wooshSfxPrefabs)
				{
					sfxBuffer.Add(new AxeSfxBufferElement
					{
						WooshSfxPrefab = GetEntity(authoringSfxPrefab, TransformUsageFlags.Dynamic),
					});
				}
				
				var hitSfxBuffer = AddBuffer<AxeHitSfxBufferElement>(entity);
				foreach (var authoringSfxPrefab in authoring.hitSfxPrefabs)
				{
					hitSfxBuffer.Add(new AxeHitSfxBufferElement
					{
						HitSfxPrefab = GetEntity(authoringSfxPrefab, TransformUsageFlags.Dynamic),
					});
				}

				var clashSfxBuffer = AddBuffer<AxeClashSfxBufferElement>(entity);
				foreach (var authoringSfxPrefab in authoring.clashSfxPrefabs)
				{
					clashSfxBuffer.Add(new AxeClashSfxBufferElement
					{
						ClashSfxPrefab = GetEntity(authoringSfxPrefab, TransformUsageFlags.Dynamic),
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
	}
	
	public struct AxeSfxBufferElement : IBufferElementData
	{
		public EntityWith<Prefab> WooshSfxPrefab;
	}
	
	public struct AxeHitSfxBufferElement : IBufferElementData
	{
		public EntityWith<Prefab> HitSfxPrefab;
	}
	
	public struct AxeClashSfxBufferElement : IBufferElementData
	{
		public EntityWith<Prefab> ClashSfxPrefab;
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