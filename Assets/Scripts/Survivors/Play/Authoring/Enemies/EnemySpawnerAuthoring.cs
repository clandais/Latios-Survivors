using Unity.Entities;
using UnityEngine;

namespace Survivors.Play.Authoring.Enemies
{
	public class EnemySpawnerAuthoring : MonoBehaviour
	{
		
		[SerializeField] private GameObject enemyPrefab;
		[SerializeField] private float spawnInterval;
		[SerializeField] private int maxEnemies;
		
		private class EnemySpawnerAuthoringBaker : Baker<EnemySpawnerAuthoring>
		{
			
			public override void Bake(EnemySpawnerAuthoring authoring)
			{
				var entity = GetEntity(TransformUsageFlags.Dynamic);
				AddComponent(entity, new EnemySpawnerData
				{
					enemyPrefab = GetEntity(authoring.enemyPrefab, TransformUsageFlags.Dynamic),
					spawnInterval = authoring.spawnInterval,
					maxEnemies = authoring.maxEnemies
				});
			}
		}
		
	}
	
	public struct EnemySpawnerData : IComponentData
	{
		public Entity enemyPrefab;
		public float spawnInterval;
		public int maxEnemies;
		
		public float currentSpawnInterval;
		public int currentEnemies;
	}
}