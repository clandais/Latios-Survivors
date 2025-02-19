using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Survivors.Setup.MonoBehaviours
{

	[RequireComponent(typeof(Image))]
	public class CurtainBehaviour : MonoBehaviour
	{
		Image _image;

		private void Awake()
		{
			_image = GetComponent<Image>();
		}


		public IEnumerator FadeAlpha(float from, float to, float time)
		{
			float elapsedTime = 0;
			Color color = _image.color;
			color.a = from;
			_image.color = color;

			while (elapsedTime < time)
			{
				elapsedTime += Time.deltaTime;
				color.a = Mathf.Lerp(from, to, elapsedTime / time);
				_image.color = color;
				yield return null;
			}

			color.a = to;
			_image.color = color;
		}
	}
}