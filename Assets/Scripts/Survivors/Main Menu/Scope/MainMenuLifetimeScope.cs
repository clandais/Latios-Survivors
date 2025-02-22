using Survivors.BootStrap;
using Survivors.Setup;
using Survivors.Setup.MonoBehaviours;
using Survivors.Setup.Scope;
using Survivors.Setup.Scope.Interceptors;
using Survivors.Setup.Systems;
using Unity.Entities;
using UnityEngine;
using VContainer;
using VContainer.Unity;
using VitalRouter;
using VitalRouter.VContainer;

namespace Survivors.Main_Menu.Scope
{
	public class MainMenuLifetimeScope : LifetimeScope
	{
		[SerializeField] private MainMenuBehaviour mainMenuBehaviour;
	
		
		protected override void Configure(IContainerBuilder builder)
		{
			builder.RegisterInstance(mainMenuBehaviour);

			
			World.DefaultGameObjectInjectionWorld?.Dispose();
			
			if (new LatiosBootstrap().Initialize("LatiosWorld"))
			{
				Debug.Log("Latios initialized");
			}
			else
			{
				Debug.LogError("Latios failed to initialize");
			}


			builder.RegisterSystemFromDefaultWorld<GlobalInputReadSystem>();
			
			builder.UseEntryPoints(cfg =>
			{
				cfg.Add<MainMenuEntryPoint>();
				cfg.OnException(Debug.LogException);
			});


			if (!Parent.Container.TryResolve(out MainMenuRouter _))
			{
				builder.RegisterVitalRouter(routing =>
				{
					routing.Map<MainMenuRouter>();
				});
			}
			
		}
		
	}
}