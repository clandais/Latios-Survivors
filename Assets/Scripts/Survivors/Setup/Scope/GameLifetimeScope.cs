
using Survivors.Setup.MonoBehaviours;
using Survivors.Setup.Scope.Interceptors;
using Survivors.Setup.Scope.Messages;
using Survivors.Setup.Scope.Messages.GlobalMessages;
using Survivors.Setup.ScriptableObjects;
using Survivors.Setup.Systems;
using UnityEngine;
using VContainer;
using VContainer.Unity;
using VitalRouter;
using VitalRouter.VContainer;

namespace Survivors.Setup.Scope
{
	public class GameLifetimeScope : LifetimeScope
	{
		[SerializeField] private GameScenesReferences gameScenesReferences;
		[SerializeField] private CurtainBehaviour curtainBehaviour;
		[SerializeField] private Transform cinemachineTarget;
		
		protected override void Configure(IContainerBuilder builder)
		{
			builder.RegisterInstance(gameScenesReferences);
			builder.RegisterInstance(curtainBehaviour);
			builder.RegisterInstance(cinemachineTarget);
			
			
			builder.RegisterVitalRouter(routingBuilder =>
			{
				routingBuilder.Isolated = true;
				routingBuilder.Filters
					.Add<ExceptionHandling>()
					.Add<LoggingInterceptor>();
				
				routingBuilder.Map<GlobalRouter>();
			});



			
			builder.RegisterBuildCallback( container =>
			{
				var publisher = container.Resolve<ICommandPublisher>();
				
				publisher.PublishAsync( new MainMenuStateCommand());
			});
		}
	}


}