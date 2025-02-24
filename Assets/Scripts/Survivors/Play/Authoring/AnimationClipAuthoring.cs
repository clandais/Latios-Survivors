using Latios.Authoring;
using Latios.Kinemation;
using Latios.Kinemation.Authoring;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace Survivors.Play.Authoring
{
	public class AnimationClipAuthoring : MonoBehaviour
	{
		[SerializeField] private AnimationClip idleClip;
		[SerializeField] private AnimationClip walkClip;
		[SerializeField] private AnimationClip walkBackwardClip;
		[SerializeField] private AnimationClip strafeLeftClip;
		[SerializeField] private AnimationClip strafeRightClip;

		[TemporaryBakingType]
		private struct AnimationClipSmartBakeItem : ISmartBakeItem<AnimationClipAuthoring>
		{

			private SmartBlobberHandle<SkeletonClipSetBlob> _blobberHandle;

			public bool Bake(AnimationClipAuthoring authoring, IBaker baker)
			{

				baker.AddComponent<LocomotionClips>(baker.GetEntity(TransformUsageFlags.Dynamic));
				var clips = new NativeArray<SkeletonClipConfig>(5, Allocator.Temp);

				clips[0] = new SkeletonClipConfig { clip = authoring.idleClip, settings         = SkeletonClipCompressionSettings.kDefaultSettings };
				clips[1] = new SkeletonClipConfig { clip = authoring.walkClip, settings         = SkeletonClipCompressionSettings.kDefaultSettings };
				clips[2] = new SkeletonClipConfig { clip = authoring.walkBackwardClip, settings = SkeletonClipCompressionSettings.kDefaultSettings };
				clips[3] = new SkeletonClipConfig { clip = authoring.strafeLeftClip, settings   = SkeletonClipCompressionSettings.kDefaultSettings };
				clips[4] = new SkeletonClipConfig { clip = authoring.strafeRightClip, settings  = SkeletonClipCompressionSettings.kDefaultSettings };

				_blobberHandle = baker.RequestCreateBlobAsset(baker.GetComponent<Animator>(), clips);
				return true;
			}

			public void PostProcessBlobRequests(EntityManager entityManager, Entity entity)
			{
				entityManager.SetComponentData(entity, new LocomotionClips
				{
					Blob = _blobberHandle.Resolve(entityManager)
				});

				entityManager.AddBuffer<ClipsTimes>(entity);

				var clipsTimes = entityManager.GetBuffer<ClipsTimes>(entity);
				clipsTimes.Add(new ClipsTimes { Time = 0 });
				clipsTimes.Add(new ClipsTimes { Time = 0 });
				clipsTimes.Add(new ClipsTimes { Time = 0 });
				clipsTimes.Add(new ClipsTimes { Time = 0 });
				clipsTimes.Add(new ClipsTimes { Time = 0 });

			}
		}

		private class AnimationClipBaker : SmartBaker<AnimationClipAuthoring, AnimationClipSmartBakeItem>
		{
		}
	}

	public struct LocomotionClips : IComponentData
	{
		public BlobAssetReference<SkeletonClipSetBlob> Blob;
	}

	public struct ClipsTimes : IBufferElementData
	{
		public float Time;
	}
}