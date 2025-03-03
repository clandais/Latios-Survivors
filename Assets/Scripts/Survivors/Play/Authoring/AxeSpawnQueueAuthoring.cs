using Latios;
using Survivors.Play.Components;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace Survivors.Play.Authoring
{
	public class AxeSpawnQueueAuthoring : MonoBehaviour
	{
		private class AxeSpawnQueueAuthoringBaker : Baker<AxeSpawnQueueAuthoring>
		{
			public override void Bake(AxeSpawnQueueAuthoring authoring)
			{
				var entity = GetEntity(TransformUsageFlags.None);
			}
			
		}
	}
}