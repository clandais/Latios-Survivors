using System;
using System.Threading;
using System.Threading.Tasks;
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
	public class MainMenuEntryPoint : IStartable, IAsyncStartable, IDisposable
	{
		[Inject] private MainMenuBehaviour _mainMenuBehaviour;
		[Inject] private ICommandPublisher _commandPublisher;

		private IDisposable _disposable;
		
		public void Start()
		{

			DisposableBuilder d = Disposable.CreateBuilder();
			
			_mainMenuBehaviour.StartButtonClicked.AsObservable()
				.SubscribeAwait(OnStartButtonClicked)
				.AddTo(ref d);
			

			_disposable = d.Build();
			
		}
		
		public async UniTask StartAsync(CancellationToken cancellation = new CancellationToken())
		{
			await _commandPublisher.PublishAsync(new TriggerCurtainFade { FromAlpha = 1f, ToAlpha = 0f, Duration = 1f}, cancellation);
		}
		
		
		async ValueTask OnStartButtonClicked(Unit _, CancellationToken cancellation)
		{
			await _commandPublisher.PublishAsync(new StartButtonClickedCommand(), cancellation);
		}
		

		public void Dispose()
		{
			_disposable.Dispose();
		}


	}
}