using System;
using R3;
using Survivors.Play.MonoBehaviours;
using Survivors.Play.Scope.Messages;
using Survivors.Setup.Scope.Messages.GlobalMessages;
using UnityEditor;
using VContainer;
using VContainer.Unity;
using VitalRouter;

namespace Survivors.Play.Scope
{
	public class PlayLifetimeContoller : IStartable, IDisposable
	{
		[Inject] private PlayStateMenu _playStateMenu;
		[Inject] private ICommandSubscribable _commandSubscribable;
		[Inject] private ICommandPublisher _commandPublisher;

		private DisposableBag _disposable;


		public void Start()
		{
			_disposable = new DisposableBag();

			_playStateMenu.QuitButtonClicked.Subscribe(OnQuitButtonClicked)
				.AddTo(ref _disposable);

			_playStateMenu.ResumeButtonClicked.Subscribe(OnResumeButtonClicked)
				.AddTo(ref _disposable);

			_playStateMenu.MainMenuButtonClicked.Subscribe(OnMainMenuButtonClicked)
				.AddTo(ref _disposable);

			_commandSubscribable.Subscribe<RequestPauseStateCommand>(OnPauseStateRequested)
				.AddTo(ref _disposable);

			_commandSubscribable.Subscribe<RequestResumeStateCommand>(OnResumeStateRequested)
				.AddTo(ref _disposable);

			_playStateMenu.Hide();

			_commandPublisher.PublishAsync(new TriggerCurtainFade { FromAlpha = 1f, ToAlpha = 0f, Duration = 1f });
		}


		private void OnPauseStateRequested(RequestPauseStateCommand _, PublishContext ctx)
		{
			_playStateMenu.Show();
		}

		private void OnResumeStateRequested(RequestResumeStateCommand _, PublishContext ctx)
		{
			_playStateMenu.Hide();
		}

		private void OnMainMenuButtonClicked(Unit _)
		{
			_commandPublisher.PublishAsync(new BackToMainMenuClicked());
		}

		private void OnResumeButtonClicked(Unit _)
		{
			_commandPublisher.PublishAsync(new ResumeButtonClicked());
		}

		private void OnQuitButtonClicked(Unit _)
		{


#if UNITY_EDITOR
			EditorApplication.ExitPlaymode();
#else
			Application.Quit();
#endif
		}

		public void Dispose()
		{
			_disposable.Dispose();
		}


	}
}