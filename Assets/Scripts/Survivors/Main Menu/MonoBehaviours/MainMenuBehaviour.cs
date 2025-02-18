using Latios;
using Unity.Entities;
using UnityEngine;
using UnityEngine.UI;

namespace Survivors.Setup.MonoBehaviours
{
	public class MainMenuBehaviour : MonoBehaviour
	{

		[SerializeField] private Button startButton;
		
		public Button StartButton => startButton;
		
	}
}