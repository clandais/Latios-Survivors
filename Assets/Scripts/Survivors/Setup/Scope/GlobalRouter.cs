using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Survivors.Setup.MonoBehaviours;
using Survivors.Setup.Scope.Interceptors;
using Survivors.Setup.Scope.Messages;
using Survivors.Setup.Scope.Messages.GlobalMessages;
using Survivors.Setup.ScriptableObjects;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;
using VContainer;
using VitalRouter;

namespace Survivors.Setup.Scope
{

	[Routes]
	[Filter(typeof(LoggingInterceptor))]
	[Filter(typeof(GameStateInterceptor))]
	public partial class GlobalRouter : IDisposable
	{
		[Inject] private CurtainBehaviour _curtainBehaviour;
		[Inject] private GameScenesReferences _gameScenesReferences;
		
		private AsyncOperationHandle<SceneInstance> _mainMenuScene;
		private AsyncOperationHandle<SceneInstance> _playScene;
		
		[Route]
		async UniTask On(MainMenuStateCommand _)
		{
			var handle = Addressables.LoadSceneAsync(_gameScenesReferences.mainMenuScene, LoadSceneMode.Additive);
			await handle.Task;
			
			_mainMenuScene = handle;
		}
		
		
		[Route]
		private async UniTask On(PlayStateCommand ç)
		{
			await Addressables.UnloadSceneAsync(_mainMenuScene);
			var handle = Addressables.LoadSceneAsync(_gameScenesReferences.playScene, LoadSceneMode.Additive);
			await handle.Task;
			
			_playScene = handle;
		}
		

		[Route]
		async UniTask On(TriggerCurtainFade cmd)
		{
			await _curtainBehaviour.FadeAlpha( cmd.FromAlpha, cmd.ToAlpha, cmd.Duration);
		}

		public void Dispose()
		{
			if (_mainMenuScene.IsValid())
				_mainMenuScene.Release();
			
			if (_playScene.IsValid())
				_playScene.Release();
		}
	}
}