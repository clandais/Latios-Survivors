
using Survivors.Setup.MonoBehaviours;
using Survivors.Setup.Scope.Interceptors;
using Survivors.Setup.Scope.Messages.GlobalMessages;
using Survivors.Setup.ScriptableObjects;
using Unity.Entities;
using UnityEditor;
using UnityEngine;
using VContainer;
using VContainer.Unity;
using VitalRouter;
using VitalRouter.VContainer;

namespace Survivors.Setup.Scope
{
	public class GameLifetimeScope : LifetimeScope
	{
		[SerializeField] GameScenesReferences gameScenesReferences;
		[SerializeField] CurtainBehaviour curtainBehaviour;
		[SerializeField] CinemachineBehaviour cinemachineBehaviour;

#if UNITY_EDITOR

		protected override void Awake()
		{
			base.Awake();
			
			// Dispose MANUALLY the world when exiting play mode
			EditorApplication.playModeStateChanged += state =>
			{
				if (state == PlayModeStateChange.ExitingPlayMode)
				{
					World.DefaultGameObjectInjectionWorld?.Dispose();
				}
			};
		}
#endif

		protected override void Configure(IContainerBuilder builder)
		{
			builder.RegisterInstance(gameScenesReferences);
			builder.RegisterInstance(curtainBehaviour);
			builder.RegisterInstance(cinemachineBehaviour);


			builder.RegisterVitalRouter(routingBuilder =>
			{
				routingBuilder.Isolated = true;
				
				routingBuilder.Filters
					.Add<ExceptionHandling>()
					.Add<LoggingInterceptor>();

				routingBuilder.Map<GlobalRouter>();
			});


			builder.RegisterBuildCallback(container =>
			{
				// Upon build, we want to start the game in the main menu
				var publisher = container.Resolve<ICommandPublisher>();
				publisher.PublishAsync(new MainMenuStateCommand());
			});
		}
	}
}