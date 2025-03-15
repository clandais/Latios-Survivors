using System;
using Cysharp.Threading.Tasks;
using Survivors.Play.MonoBehaviours;
using Survivors.Play.Scope.Messages;
using Survivors.Setup.Scope.Messages.GlobalMessages;
using UnityEngine;
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

		[Route]
		void On(PlayerScrollCommand command)
		{
			ParentPublisher.PublishAsync( new CameraZoomCommand { ZoomValue = command.ScrollValue });
		}

		public void Dispose()
		{
			UnmapRoutes();
		}
	}
}