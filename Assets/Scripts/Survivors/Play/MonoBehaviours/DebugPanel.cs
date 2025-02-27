using TMPro;
using UnityEngine;

namespace Survivors.Play.MonoBehaviours
{
	public class DebugPanel : MonoBehaviour
	{
		[SerializeField] private TMP_Text _text;
		
		public TMP_Text DebugText => _text;
	}
}