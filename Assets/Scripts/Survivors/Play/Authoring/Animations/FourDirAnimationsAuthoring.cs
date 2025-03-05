using System;
using Latios.Authoring;
using Latios.Kinemation;
using Latios.Kinemation.Authoring;
using Survivors.Play.Components;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;
using UnityEngine.Serialization;
using SkeletonClipConfig = Latios.Kinemation.Authoring.SkeletonClipConfig;

namespace Survivors.Play.Authoring.Animations
{
	public class FourDirAnimationsAuthoring : MonoBehaviour
	{
		public FourDirAnimations animations;

		[TemporaryBakingType] struct AnimationClipSmartBakeItem : ISmartBakeItem<FourDirAnimationsAuthoring>
		{

			SmartBlobberHandle<SkeletonClipSetBlob> m_clipSetHandle;

			public bool Bake(FourDirAnimationsAuthoring authoring, IBaker baker)
			{
				Entity entity = baker.GetEntity(TransformUsageFlags.Dynamic);
				baker.AddComponent<Clips>(entity);

				var clips = new NativeArray<SkeletonClipConfig>(6, Allocator.Temp);

				for (int i = 0; i < 6; i++)
				{
					AnimationClip clip = null;
					switch (i)
					{
						case (int)EDirections.Center:
							clip = authoring.animations.center.clip;
							break;
						case (int)EDirections.Down:
							clip = authoring.animations.down.clip;
							break;
						case (int)EDirections.Up:
							clip = authoring.animations.up.clip;
							break;
						case (int)EDirections.Left:
							clip = authoring.animations.left.clip;
							break;
						case (int)EDirections.Right:
							clip = authoring.animations.right.clip;
							break;
						case (int)EDirections.Attack:
							clip = authoring.animations.attack.clip;
							break;
					}

					if (clip.events != null)
					{
						clips[i] = new SkeletonClipConfig
						{
							clip     = clip,
							settings = SkeletonClipCompressionSettings.kDefaultSettings,
							events   = clip.ExtractKinemationClipEvents(Allocator.Temp)
						};
					}
					else
					{
						clips[i] = new SkeletonClipConfig
						{
							clip     = clip,
							settings = SkeletonClipCompressionSettings.kDefaultSettings,
						};
					}
					

				}

				m_clipSetHandle = baker.RequestCreateBlobAsset(baker.GetComponent<Animator>(), clips);
				return true;
			}

			public void PostProcessBlobRequests(EntityManager entityManager, Entity entity)
			{
				var clipSet = m_clipSetHandle.Resolve(entityManager);
				entityManager.SetComponentData(entity, new Clips { ClipSet = clipSet });
			}
		}

		class ClpBaker : SmartBaker<FourDirAnimationsAuthoring, AnimationClipSmartBakeItem>
		{

		}


		class FourDirAnimationsBaker : Baker<FourDirAnimationsAuthoring>
		{
			public override void Bake(FourDirAnimationsAuthoring authoring)
			{

				Entity entity = GetEntity(TransformUsageFlags.Dynamic);

				var clipStates = new FourDirectionClipStates
				{
					Center   = new ClipState { SpeedMultiplier = authoring.animations.center.speedMultiplier },
					Down     = new ClipState { SpeedMultiplier = authoring.animations.down.speedMultiplier },
					Up       = new ClipState { SpeedMultiplier = authoring.animations.up.speedMultiplier },
					Left     = new ClipState { SpeedMultiplier = authoring.animations.left.speedMultiplier },
					Right    = new ClipState { SpeedMultiplier = authoring.animations.right.speedMultiplier },
					Attack = new ClipState
					{
						SpeedMultiplier = authoring.animations.attack.speedMultiplier,
						EventHash       =  (authoring.animations.attack.clip.events.Length > 0) ? authoring.animations.attack.clip.ExtractKinemationClipEvents(Allocator.Temp)[0].name.GetHashCode() : -1,
					}
				};

				AddComponent(entity, clipStates);
			}
		}

	}


	#region Animation Authoring Structs

	[Serializable]
	public struct FourDirAnimations
	{
		public AnimationClipProperty center;
		public AnimationClipProperty down;
		public AnimationClipProperty up;
		public AnimationClipProperty left;
		public AnimationClipProperty right;
		public AnimationClipProperty attack;

	}

	[Serializable]
	public struct AnimationClipProperty
	{
		public AnimationClip clip;
		public float speedMultiplier;
	}

	#endregion
}