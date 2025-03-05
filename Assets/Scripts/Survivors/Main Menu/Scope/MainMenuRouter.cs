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
		private ICommandPublisher _parentPublisher; 

		[Route]
		private async UniTask On(StartButtonClickedCommand _)
		{
			await UniTask.CompletedTask;
		//	await _parentPublisher.PublishAsync(new TriggerCurtainFade { FromAlpha = 0f, ToAlpha = 1f, Duration = 1f });
			_parentPublisher.PublishAsync(new PlayStateCommand()).AsUniTask().Forget();
		}

		public void Dispose()
		{
			UnmapRoutes();
		}
	}
}