using System;
using R3;
using UnityEngine;
using UnityEngine.UI;

namespace Survivors.Play.MonoBehaviours
{
	public class PlayStateMenu : MonoBehaviour
	{
		[SerializeField] private Button quitButton;
		[SerializeField] private Button resumeButton;
		[SerializeField] private Button mainMenuButton;


		public ReactiveCommand<Unit> QuitButtonClicked = new ReactiveCommand<Unit>();
		public ReactiveCommand<Unit> ResumeButtonClicked = new ReactiveCommand<Unit>();
		public ReactiveCommand<Unit> MainMenuButtonClicked = new ReactiveCommand<Unit>();
		
		
		
		private CanvasGroup _canvasGroup;

		private void Awake()
		{
			_canvasGroup = GetComponent<CanvasGroup>();
		}

		private void Start()
		{
			quitButton.onClick.AddListener(() => QuitButtonClicked.Execute(Unit.Default));
			resumeButton.onClick.AddListener(() => ResumeButtonClicked.Execute(Unit.Default));
			mainMenuButton.onClick.AddListener(() => MainMenuButtonClicked.Execute(Unit.Default));
		}

		private void OnDestroy()
		{
			quitButton.onClick.RemoveAllListeners();
			resumeButton.onClick.RemoveAllListeners();
			mainMenuButton.onClick.RemoveAllListeners();
		}


		public void Show()
		{
			_canvasGroup.alpha = 1;
			_canvasGroup.blocksRaycasts = true;
			_canvasGroup.interactable = true;
		}
		
		public void Hide()
		{
			_canvasGroup.alpha = 0;
			_canvasGroup.blocksRaycasts = false;
			_canvasGroup.interactable = false;
		}
		
	}
}