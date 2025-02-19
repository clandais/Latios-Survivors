using Survivors.Setup.MonoBehaviours;
using Survivors.Setup.Scope;
using Survivors.Setup.Systems;
using UnityEngine;
using VContainer;
using VContainer.Unity;
using VitalRouter.VContainer;

namespace Survivors.Main_Menu.Scope
{
	public class MainMenuLifetimeScope : LifetimeScope
	{
		[SerializeField] private MainMenuBehaviour mainMenuBehaviour;
	
		
		protected override void Configure(IContainerBuilder builder)
		{
			builder.RegisterInstance(mainMenuBehaviour);
			
			
			builder.UseEntryPoints(cfg =>
			{
				cfg.Add<MainMenuEntryPoint>();
				cfg.OnException(Debug.LogException);
			});
			
			builder.RegisterVitalRouter(routing =>
			{
				routing.Map<MainMenuRouter>();
			});
		}
	}
}