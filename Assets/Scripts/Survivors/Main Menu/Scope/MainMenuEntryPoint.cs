using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using R3;
using Survivors.Setup.MonoBehaviours;
using Survivors.Setup.Scope.Messages;
using Survivors.Setup.Scope.Messages.GlobalMessages;
using UnityEngine;
using VContainer;
using VContainer.Unity;
using VitalRouter;

namespace Survivors.Main_Menu.Scope
{
	public class MainMenuEntryPoint : IStartable, IDisposable
	{
		[Inject] private MainMenuBehaviour _mainMenuBehaviour;
		[Inject] private ICommandPublisher _commandPublisher;

		private IDisposable _disposable;
		
		public void Start()
		{
			var dbb = Disposable.CreateBuilder();
			_mainMenuBehaviour.StartButton.OnClickAsObservable().Subscribe(OnStartButtonClicked)
				.AddTo(ref dbb);
			
			_disposable = dbb.Build();
		}
		
		
		void OnStartButtonClicked(Unit _)
		{
			_mainMenuBehaviour.StartButton.interactable = false;
			NotifyStartButtonClicked().Forget();
		}
		
		async UniTask NotifyStartButtonClicked()
		{
			// Wait for the command to be processed
			await _commandPublisher.PublishAsync(new StartButtonClickedCommand());
			
			// Unload the current scene
			await _commandPublisher.PublishAsync(new PlayStateCommand());
		}

		public void Dispose()
		{
			_disposable?.Dispose();
		}
	}
}