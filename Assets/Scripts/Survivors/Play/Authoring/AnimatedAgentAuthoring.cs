using Survivors.Play.Components;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Survivors.Play.Authoring
{
	public class AnimatedAgentAuthoring : MonoBehaviour
	{
		[SerializeField] private float walkSpeed = 2f;
		[SerializeField] private float runSpeed = 5f;
		[SerializeField] private float velocityChange = 10f;
		[SerializeField] private float radius = 1f;
		[SerializeField] private float obstacleHorizon = 10f;
		
		
		void OnDrawGizmos()
		{
			Gizmos.color = Color.cyan;
			
			Gizmos.DrawWireSphere( transform.position, radius);
		}
		
		private class AnimatedAgentAuthoringBaker : Baker<AnimatedAgentAuthoring>
		{
			public override void Bake(AnimatedAgentAuthoring authoring)
			{
				Entity entity = GetEntity(TransformUsageFlags.Dynamic);
				
				AddComponent(entity, new AgentSettings
				{
					WalkSpeed = authoring.walkSpeed,
					RunSpeed = authoring.runSpeed,
					VelocityChange = authoring.velocityChange,
					Radius = authoring.radius,
					ObstacleHorizon = authoring.obstacleHorizon,
				});
				
				AddComponent(entity, new MotionComponent
				{
					Rotation = quaternion.identity,
					DesiredRotation = quaternion.LookRotation(math.forward(), math.up())
				});
				
				AddComponent<AgentVelocityComponent>(entity);
			}
		}
	}
}