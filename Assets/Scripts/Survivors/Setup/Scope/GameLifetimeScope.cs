
using Survivors.Setup.ScriptableObjects;
using UnityEngine;
using UnityEngine.AddressableAssets;
using VContainer;
using VContainer.Unity;
using VitalRouter;
using VitalRouter.VContainer;

namespace Survivors.Setup.Scope
{
	public class GameLifetimeScope : LifetimeScope
	{
		[SerializeField] private GameScenesReferences gameScenesReferences;
		
		protected override void Configure(IContainerBuilder builder)
		{
			builder.RegisterInstance(gameScenesReferences);
			
			builder.RegisterVitalRouter(routingBuilder =>
			{
				routingBuilder.Map<GlobalRouter>();
			});
			
			builder.RegisterBuildCallback( container =>
			{
				var publisher = container.Resolve<ICommandPublisher>();
				publisher.PublishAsync( new LoadSceneCommand { Scene = gameScenesReferences.mainMenuScene });
			});
		}
	}

	public struct LoadSceneCommand : ICommand
	{
		public AssetReference Scene;
	}
}