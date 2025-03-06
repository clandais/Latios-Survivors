using Survivors.Play.Components;
using Survivors.Play.Systems.Enemies;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Survivors.Play.Authoring.Enemies
{
	
	public class EnemyAuthoring : MonoBehaviour
	{

		[SerializeField] float velocityWeight;
		[SerializeField] float separationWeight;
		[SerializeField] float alignmentWeight;
		[SerializeField] float cohesionWeight;
		[SerializeField] float viewRadius = 50f;
		[SerializeField] float separationRadius = 20f;
		[SerializeField] float cohesionRadius = 10f;
		private class EnemyAuthoringBaker : Baker<EnemyAuthoring>
		{
			public override void Bake(EnemyAuthoring authoring)
			{
				var entity = GetEntity(TransformUsageFlags.Dynamic);
				AddComponent<EnemyTag>(entity);
				
				AddComponent<CohesionForce>(entity);
				AddComponent<AlignmentForce>(entity);
				AddComponent<SeparationForce>(entity);
				
				AddComponent(entity, new SteeringComponent
				{
					ViewRadius = authoring.viewRadius,
					SeparationRadius = authoring.separationRadius,
					CohesionRadius = authoring.cohesionRadius,
					VelocityWeight = authoring.velocityWeight,
					SeparationWeight = authoring.separationWeight,
					AlignmentWeight  = authoring.alignmentWeight,
					CohesionWeight   = authoring.cohesionWeight,
				});
			}
		}
	}
	
	public struct EnemyTag : IComponentData
	{
	}
	
	public struct SteeringComponent : IComponentData
	{

		public float ViewRadius;
		public float SeparationRadius;
		public float CohesionRadius;
		public float VelocityWeight;
		public float SeparationWeight;
		public float AlignmentWeight;
		public float CohesionWeight;
	}
}