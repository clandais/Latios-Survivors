using Latios.Authoring;
using Latios.Kinemation;
using Latios.Kinemation.Authoring;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace Survivors.Play.Authoring.Animations
{
	public class MaskAuthoring : MonoBehaviour
	{
		[SerializeField] private AvatarMask mask;
		
		[TemporaryBakingType]
		private struct AvatarMaskSmartBakeItem : ISmartBakeItem<MaskAuthoring>
		{
			private SmartBlobberHandle<SkeletonBoneMaskSetBlob>  _blobberHandle;

			public bool Bake(MaskAuthoring authoring, IBaker baker)
			{
				baker.AddComponent<AvatarMasks>(baker.GetEntity(TransformUsageFlags.Dynamic));


				var masks = new NativeArray<UnityObjectRef<AvatarMask>>(1, Allocator.Temp);
				masks[0] = authoring.mask;
				
				_blobberHandle = baker.RequestCreateBlobAsset(baker.GetComponent<Animator>(), masks);

				masks.Dispose();
				return true;

			}

			public void PostProcessBlobRequests(EntityManager entityManager, Entity entity)
			{
				
				entityManager.SetComponentData(entity, new AvatarMasks
				{
					Blob = _blobberHandle.Resolve(entityManager)
				});
			}
		}

		class AnimatioMaskBaker : SmartBaker<MaskAuthoring, AvatarMaskSmartBakeItem>
		{
			
		}
	}


	public struct AvatarMasks : IComponentData
	{
		public BlobAssetReference<SkeletonBoneMaskSetBlob> Blob;
	}
}