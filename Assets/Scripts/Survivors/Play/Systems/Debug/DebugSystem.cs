using Latios;
using Survivors.Play.Authoring.Enemies;
using Survivors.Play.Components;
using Survivors.Setup.Scope.Messages.GlobalMessages;
using Unity.Entities;
using Unity.Mathematics;
using VContainer;
using VitalRouter;

namespace Survivors.Play.Systems.Debug
{
	public partial class DebugSystem : SystemBase
	{
		ICommandPublisher _publisher;

		[Inject]
		public void Construct(ICommandPublisher publisher)
		{
			_publisher = publisher;
		}


		protected override void OnCreate()
		{
			RequireForUpdate<PlayerTag>();
		}

		protected override void OnUpdate()
		{

			string message = "";
			
			var aliveEnemyCount = SystemAPI.QueryBuilder().WithAll<EnemyTag>().WithNone<DeadTag>().Build().CalculateEntityCount();
			message += $"Alive Enemies: {aliveEnemyCount}\n";
			
			var deadEnemyCount = SystemAPI.QueryBuilder().WithAll<EnemyTag>().WithAll<DeadTag>().Build().CalculateEntityCount();
			message += $"Dead Enemies: {deadEnemyCount}\n";
			
			
			_publisher.PublishAsync(new DebugCommand { Message = message });
		}
	}
}