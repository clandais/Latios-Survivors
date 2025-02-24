using Survivors.Play.Components;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Survivors.Play.Authoring
{
	public class PlayerAuthoring : MonoBehaviour
	{

		[SerializeField] private float walkSpeed = 2f;
		[SerializeField] private float runSpeed = 5f;
		[SerializeField] private float velocityChange = 10f;

		private class PlayerAuthoringBaker : Baker<PlayerAuthoring>
		{
			public override void Bake(PlayerAuthoring authoring)
			{
				Entity entity = GetEntity(TransformUsageFlags.Dynamic);
				AddComponent<PlayerTag>(entity);
				AddComponent<PlayerInputState>(entity);
				AddComponent(entity, new PlayerSpeedSettings { WalkSpeed = authoring.walkSpeed, RunSpeed = authoring.runSpeed, VelocityChange = authoring.velocityChange});

				AddComponent(entity, new PlayerMotion
				{
					Rotation = quaternion.identity,
					DesiredRotation = quaternion.LookRotation(math.forward(), math.up())
				});

			}
		}
	}


}