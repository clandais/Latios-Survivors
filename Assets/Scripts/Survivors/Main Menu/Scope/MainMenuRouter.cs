using System;
using Cysharp.Threading.Tasks;
using Survivors.Setup.Scope.Messages.GlobalMessages;
using VContainer;
using VitalRouter;

namespace Survivors.Main_Menu.Scope
{

	public struct StartButtonClickedCommand : ICommand
	{
	}

	[Routes]
	public partial class MainMenuRouter : IDisposable
	{
		[Inject]
		ICommandPublisher _parentPublisher; 

		[Route]
		async UniTask On(StartButtonClickedCommand _)
		{
			await UniTask.CompletedTask;
			_parentPublisher.PublishAsync(new PlayStateCommand()).AsUniTask().Forget();
		}

		public void Dispose()
		{
			UnmapRoutes();
		}
	}
}