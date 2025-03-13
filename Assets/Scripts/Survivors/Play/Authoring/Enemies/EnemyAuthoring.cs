using System;
using Survivors.Play.Components;
using Survivors.Play.Systems.Enemies;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Survivors.Play.Authoring.Enemies
{
	
	public class EnemyAuthoring : MonoBehaviour
	{
		

		[SerializeField] GameObject DestroyVfxPrefab;


		private class EnemyAuthoringBaker : Baker<EnemyAuthoring>
		{
			
			
			public override void Bake(EnemyAuthoring authoring)
			{
				var entity = GetEntity(TransformUsageFlags.Dynamic);
				AddComponent<EnemyTag>(entity);
				AddComponent(entity, new SkeletonDestroyVfx
				{
					Prefab = GetEntity(authoring.DestroyVfxPrefab, TransformUsageFlags.Dynamic)
				});
				
			}
		}
	}
	
	public struct EnemyTag : IComponentData
	{
	}
}