using Survivors.Play.Systems;
using Survivors.Setup.Scope.Messages.GlobalMessages;
using VContainer;
using VContainer.Unity;
using VitalRouter;

namespace Survivors.Play.Scope
{
	public class PlayLifetimeScope : LifetimeScope
	{
		
		
		protected override void Configure(IContainerBuilder builder)
		{


			builder.RegisterSystemFromDefaultWorld<CinemachineTargetUpdater>();
			
			builder.RegisterBuildCallback( container =>
			{
				var publisher = container.Resolve<ICommandPublisher>();
				publisher.PublishAsync(new TriggerCurtainFade() { FromAlpha = 1f, ToAlpha = 0f, Duration = 1f});
			});
		}
	}
}