using Unity.Entities;
using UnityEngine;
using UnityEngine.Serialization;

namespace Survivors.Play.Authoring.Weapons
{
	public class RightHandSlotAuthoring : MonoBehaviour
	{
		[FormerlySerializedAs("_rightHandSlot")] [SerializeField] private Transform rightHandSlot;
		[SerializeField] private GameObject axePrefab;
		
		private class RightHandSlotAuthoringBaker : Baker<RightHandSlotAuthoring>
		{
			public override void Bake(RightHandSlotAuthoring authoring)
			{
				var entity = GetEntity(TransformUsageFlags.Dynamic);
				AddComponent(entity, new RightHandSlot
				{
					RightHandSlotEntity = GetEntity(authoring.rightHandSlot, TransformUsageFlags.Dynamic),
					AxePrefab = GetEntity(authoring.axePrefab, TransformUsageFlags.Dynamic),
				});
				
				AddComponent<RightHandSlotThrowAxeTag>(entity);
				SetComponentEnabled<RightHandSlotThrowAxeTag>(entity, false);
			
			}
		}
		
		
	}

	public struct RightHandSlot : IComponentData
	{
		public Entity RightHandSlotEntity;
		public Entity AxePrefab;
	}
	
	public struct RightHandSlotThrowAxeTag : IComponentData, IEnableableComponent {}
}