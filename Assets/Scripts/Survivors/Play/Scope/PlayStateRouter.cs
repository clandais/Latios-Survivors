using System;
using Cysharp.Threading.Tasks;
using Survivors.Play.Scope.Messages;
using Survivors.Setup.Scope.Messages.GlobalMessages;
using VitalRouter;

namespace Survivors.Play.Scope
{

	[Routes]
	public partial class PlayStateRouter : IDisposable
	{


		public ICommandPublisher ParentPublisher { get; set; }

		[Route]
		private async UniTask On(BackToMainMenuClicked _)
		{
			await ParentPublisher.PublishAsync(new TriggerCurtainFade { FromAlpha = 0f, ToAlpha = 1f, Duration = 1f });
			await ParentPublisher.PublishAsync(new MainMenuStateCommand());
		}
		
		[Route]
		async UniTask On(ResumeButtonClicked _)
		{
			await ParentPublisher.PublishAsync(new RequestResumeStateCommand());
		}

		public void Dispose()
		{
			UnmapRoutes();
		}
	}
}