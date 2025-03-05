using System;
using Cysharp.Threading.Tasks;
using Survivors.Play.MonoBehaviours;
using Survivors.Play.Scope.Messages;
using Survivors.Setup.Scope.Messages.GlobalMessages;
using VContainer;
using VitalRouter;

namespace Survivors.Play.Scope
{


	
	[Routes]
	public partial class PlayStateRouter : IDisposable
	{


		[Inject] private DebugPanel _debugPanel;
		
		public ICommandPublisher ParentPublisher { get; set; }

		[Route]
		private async UniTask On(BackToMainMenuClicked _)
		{
		//	await ParentPublisher.PublishAsync(new TriggerCurtainFade { FromAlpha = 0f, ToAlpha = 1f, Duration = 1f });
			await ParentPublisher.PublishAsync(new MainMenuStateCommand());
		}
		
		[Route]
		async UniTask On(ResumeButtonClicked _)
		{
			await ParentPublisher.PublishAsync(new RequestResumeStateCommand());
		}
		
		[Route]
		void On(DebugCommand command)
		{
			_debugPanel.DebugText.text = command.Message;
		}

		public void Dispose()
		{
			UnmapRoutes();
		}
	}
}