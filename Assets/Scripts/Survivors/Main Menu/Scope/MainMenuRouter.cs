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
	public partial class MainMenuRouter
	{
		[Inject] private ICommandPublisher _commandPublisher;

		[Route]
		private async UniTask On(StartButtonClickedCommand _)
		{
			await _commandPublisher.PublishAsync(new TriggerCurtainFade { FromAlpha = 0f, ToAlpha = 1f, Duration = 1f });
		}
	}
}