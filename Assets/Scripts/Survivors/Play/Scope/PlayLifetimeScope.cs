using Survivors.Play.MonoBehaviours;
using Survivors.Play.Systems;
using Survivors.Play.Systems.Debug;
using UnityEngine;
using VContainer;
using VContainer.Unity;
using VitalRouter;
using VitalRouter.VContainer;

namespace Survivors.Play.Scope
{
	public class PlayLifetimeScope : LifetimeScope
	{
		[SerializeField] private PlayStateMenu _playStateMenu;
		[SerializeField] private DebugPanel _debugPanel;
		
		protected override void Configure(IContainerBuilder builder)
		{

			builder.RegisterInstance(_playStateMenu);
			builder.RegisterInstance(_debugPanel);

			builder.UseEntryPoints(cfg =>
			{
				cfg.Add<PlayLifetimeContoller>();
				cfg.OnException(Debug.LogException);
			});

			builder.RegisterVitalRouter(routingBuilder =>
			{

				routingBuilder.Map<PlayStateRouter>();
			});

			builder.RegisterSystemFromDefaultWorld<CinemachineTargetUpdater>();
			builder.RegisterSystemFromDefaultWorld<MotionDebugSystem>();
			
			builder.RegisterBuildCallback(container =>
			{
				var publisher       = Parent.Container.Resolve<ICommandPublisher>();
				var playStateRouter = container.Resolve<PlayStateRouter>();
				playStateRouter.ParentPublisher = publisher;

			});


		}
	}
}