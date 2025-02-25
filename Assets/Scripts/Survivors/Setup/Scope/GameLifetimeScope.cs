using Latios;
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
		[SerializeField] private GameScenesReferences gameScenesReferences;
		[SerializeField] private CurtainBehaviour curtainBehaviour;
		[SerializeField] private Transform cinemachineTarget;

#if UNITY_EDITOR

		protected override void Awake()
		{
			base.Awake();

			EditorApplication.playModeStateChanged += state =>
			{
				if (state == PlayModeStateChange.ExitingPlayMode)
				{
					World.DisposeAllWorlds();
					UnityEditorTool.RestartEditorWorld();
				}
			};
		}
#endif

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


			builder.RegisterBuildCallback(container =>
			{
				var publisher = container.Resolve<ICommandPublisher>();

				publisher.PublishAsync(new MainMenuStateCommand());
			});
		}
	}


}