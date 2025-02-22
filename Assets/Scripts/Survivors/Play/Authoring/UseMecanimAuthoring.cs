using Unity.Entities;
using UnityEngine;

namespace Survivors.Play.Authoring
{
	public class UseMecanimAuthoring : MonoBehaviour
	{
		private class UseMecanimAuthoringBaker : Baker<UseMecanimAuthoring>
		{
			public override void Bake(UseMecanimAuthoring authoring)
			{
				var entity = GetEntity(TransformUsageFlags.Dynamic);
				AddComponent<UseMecanim>(entity);
			}
		}
	}
	
	
	public struct UseMecanim : IComponentData { }
}