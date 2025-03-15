using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Survivors.BootStrap;
using Survivors.Setup.MonoBehaviours;
using Survivors.Setup.Scope.Messages.GlobalMessages;
using Survivors.Setup.ScriptableObjects;
using Unity.Entities;
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
	public partial class GlobalRouter : IDisposable
	{


		[Inject] ICommandPublisher _publisher;
		[Inject] CurtainBehaviour _curtainBehaviour;
		[Inject] GameScenesReferences _gameScenesReferences;
		[Inject] CinemachineBehaviour _cinemachine;
		

		readonly Dictionary<string, AsyncOperationHandle<SceneInstance>> _handles = new();


		[Route]
		void On(CameraZoomCommand command)
		{
			_cinemachine.SetZoom(command.ZoomValue);
		}

		async UniTask DisposeScene(string assetGuid)
		{
			if (_handles.ContainsKey(assetGuid))
			{
				var h = _handles[assetGuid];

				if (h.IsValid())
					Addressables.Release(h);

				var handle = Addressables.UnloadSceneAsync(_handles[assetGuid]);
				await handle.Task;
				_handles.Remove(assetGuid);
			}
		}


		[Route]
		async UniTask On(MainMenuStateCommand _)
		{

			await _curtainBehaviour.FadeAlpha(0f, 1f, 1f);
			

			
			await DisposeScene(_gameScenesReferences.playScene.AssetGUID);

			if (_handles.ContainsKey(_gameScenesReferences.mainMenuScene.AssetGUID))
			{
				Debug.LogWarning("MainMenuScene is already loaded");
				return;
			}

			{

				World.DefaultGameObjectInjectionWorld?.Dispose();
				
				var handle = Addressables.LoadSceneAsync(_gameScenesReferences.mainMenuScene, LoadSceneMode.Additive);
				_handles.Add(_gameScenesReferences.mainMenuScene.AssetGUID, handle);

				await handle.ToUniTask();
				await _curtainBehaviour.FadeAlpha(1f, 0f, 1f);
			}
		}

		[Route]
		async UniTask On(PlayStateCommand _)
		{

			await _curtainBehaviour.FadeAlpha(0f, 1f, 1f);

			await DisposeScene(_gameScenesReferences.mainMenuScene.AssetGUID);


			
			if (_handles.ContainsKey(_gameScenesReferences.playScene.AssetGUID))
			{
				Debug.LogWarning("PlayScene is already loaded");
				return;
			}

			{

				if (new LatiosBootstrap().Initialize("LatiosWorld"))
				{
					Debug.Log("Latios initialized");
				}
				else
				{
					Debug.LogException( new System.Exception("Latios failed to initialize :'("));
					return;
				}
				
				var handle = Addressables.LoadSceneAsync(_gameScenesReferences.playScene, LoadSceneMode.Additive);
				_handles.Add(_gameScenesReferences.playScene.AssetGUID, handle);

				await handle.ToUniTask();
				await _curtainBehaviour.FadeAlpha(1f, 0f, 1f);

			}
		}


		[Route]
		async UniTask On(TriggerCurtainFade cmd)
		{
			await _curtainBehaviour.FadeAlpha(cmd.FromAlpha, cmd.ToAlpha, cmd.Duration);
		}


		public void Dispose()
		{

		}
	}
}