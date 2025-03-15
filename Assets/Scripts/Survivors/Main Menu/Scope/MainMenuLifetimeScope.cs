
using Survivors.Setup.MonoBehaviours;
using UnityEngine;
using VContainer;
using VContainer.Unity;
using VitalRouter.VContainer;

namespace Survivors.Main_Menu.Scope
{
	public class MainMenuLifetimeScope : LifetimeScope
	{
		[SerializeField] MainMenuBehaviour mainMenuBehaviour;


		protected override void Configure(IContainerBuilder builder)
		{
			builder.RegisterInstance(mainMenuBehaviour);



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