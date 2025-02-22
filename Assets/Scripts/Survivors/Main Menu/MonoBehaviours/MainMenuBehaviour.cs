using System;
using Latios;
using R3;
using Unity.Entities;
using UnityEngine;
using UnityEngine.UI;

namespace Survivors.Setup.MonoBehaviours
{
	public class MainMenuBehaviour : MonoBehaviour
	{

		[SerializeField] private Button startButton;
		
		public ReactiveCommand<Unit> StartButtonClicked { get; } = new ReactiveCommand<Unit>();


		private void Awake()
		{
			startButton.onClick.AddListener(() => StartButtonClicked.Execute(Unit.Default));
		}

		private void OnDestroy()
		{
			startButton.onClick.RemoveAllListeners();
		}

	}
}