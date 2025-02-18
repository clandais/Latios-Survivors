using Survivors.Setup.MonoBehaviours;
using VContainer;
using VContainer.Unity;

namespace Survivors.Main_Menu.Scope
{
	public class MainMenuEntryPoint : IStartable
	{
		[Inject] private MainMenuBehaviour _mainMenuBehaviour;

		public void Start()
		{

		}
	}
}