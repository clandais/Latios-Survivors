using System;
using System.Collections.Generic;
using Latios.Authoring;
using Latios.Kinemation;
using Latios.Kinemation.Authoring;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace Survivors.Play.Authoring.Mecanim
{



	public class MecanimLikeAuthoring : MonoBehaviour
	{
		[SerializeField] private List<AnimationParameter> animationParameters;


		[TemporaryBakingType]
		private struct AnimationClipSmartBakeItem : ISmartBakeItem<MecanimLikeAuthoring>
		{

			private SmartBlobberHandle<SkeletonClipSetBlob> m_clipSetHandle;
			private SmartBlobberHandle<BlendParametersSetBlob> m_blendParameterHandle;

			public bool Bake(MecanimLikeAuthoring authoring, IBaker baker)
			{
				Entity entity = baker.GetEntity(TransformUsageFlags.Dynamic);
				baker.AddComponent<Clips>(entity);
				baker.AddComponent<BlendParameters>(entity);

				var clips      = new NativeArray<SkeletonClipConfig>(authoring.animationParameters.Count, Allocator.Temp);
				var parameters = new NativeArray<BlendParameter>(authoring.animationParameters.Count, Allocator.Temp);
				for (int i = 0; i < authoring.animationParameters.Count; i++)
				{
					clips[i] = new SkeletonClipConfig
					{
						clip     = authoring.animationParameters[i].Clip,
						settings = SkeletonClipCompressionSettings.kDefaultSettings
					};

					parameters[i] = new BlendParameter
					{
						Value = authoring.animationParameters[i].BlendParameter
					};


				}

				m_clipSetHandle        = baker.RequestCreateBlobAsset(baker.GetComponent<Animator>(), clips);
				m_blendParameterHandle = baker.RequestCreateBlobAsset(parameters.Reinterpret<float>().ToArray());
				return true;
			}

			public void PostProcessBlobRequests(EntityManager entityManager, Entity entity)
			{


				var clipSet = m_clipSetHandle.Resolve(entityManager);

				

				entityManager.SetComponentData(entity, new Clips
				{
					ClipSet = clipSet
				});

				var parameters = m_blendParameterHandle.Resolve(entityManager);
				
				entityManager.SetComponentData(entity, new BlendParameters
				{
					Parameters = parameters,
				});

			}
		}

		private class ClipBaker : SmartBaker<MecanimLikeAuthoring, AnimationClipSmartBakeItem>
		{
		}

	}
	
	[Serializable]
	public struct AnimationParameter
	{
		public AnimationClip Clip;
		public float BlendParameter;
	}


	public struct BlendParameter
	{
		public float Value;
	}

	public struct BlendParametersSetBlob
	{
		public BlobArray<BlendParameter> Parameters;
	}

	public struct BlendParameters : IComponentData
	{
		public BlobAssetReference<BlendParametersSetBlob> Parameters;
	}

	public struct Clips : IComponentData
	{
		public BlobAssetReference<SkeletonClipSetBlob> ClipSet;
	}


	[TemporaryBakingType]
	internal struct BlendParameterElementInput : IBufferElementData
	{
		public float Value;
	}

	public struct ParametersSmartBlobberRequestFilter : ISmartBlobberRequestFilter<BlendParametersSetBlob>
	{
		public float[] Value;

		public bool Filter(IBaker baker, Entity blobBakingEntity)
		{
			var buffer = baker.AddBuffer<BlendParameterElementInput>(blobBakingEntity).Reinterpret<float>();

			foreach (float f in Value)
			{
				buffer.Add(f);
			}

			return true;
		}
	}


}