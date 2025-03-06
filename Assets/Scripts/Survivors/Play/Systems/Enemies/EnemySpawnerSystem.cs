using Latios;
using Latios.Transforms;
using Survivors.Play.Authoring.Enemies;
using Unity.Burst;
using Unity.Entities;

namespace Survivors.Play.Systems.Enemies
{
	[RequireMatchingQueriesForUpdate]
	public partial struct EnemySpawnerSystem : ISystem
	{
		
		LatiosWorldUnmanaged m_latiosWorldUnmanaged;
		EntityQuery m_enemySpawnerQuery;
		
		[BurstCompile]
		public void OnCreate(ref SystemState state)
		{
			m_latiosWorldUnmanaged = state.GetLatiosWorldUnmanaged();
			m_enemySpawnerQuery = state.Fluent()
				.With<EnemySpawnerData>()
				.Build();
		}

		[BurstCompile]
		public void OnUpdate(ref SystemState state)
		{

			foreach (var (data, transform) in SystemAPI.Query<RefRW<EnemySpawnerData>, RefRO<WorldTransform>>())
			{
				
				data.ValueRW.currentSpawnInterval += SystemAPI.Time.DeltaTime;
				
				if (data.ValueRW.currentSpawnInterval >= data.ValueRO.spawnInterval)
				{
					if (data.ValueRO.currentEnemies >= data.ValueRO.maxEnemies) continue;
					
					
					var newEnemy = state.EntityManager.Instantiate(data.ValueRO.enemyPrefab);
					state.EntityManager.SetComponentData(newEnemy,transform.ValueRO);
					data.ValueRW.currentEnemies++;
					data.ValueRW.currentSpawnInterval = 0;
				}
				
			}
			
		}

		[BurstCompile]
		public void OnDestroy(ref SystemState state)
		{

		}
	}
}