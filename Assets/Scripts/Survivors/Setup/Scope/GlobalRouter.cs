using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;
using VitalRouter;

namespace Survivors.Setup.Scope
{

	[Routes]
	public partial class GlobalRouter
	{
		[Route]
		private void On(PingMessage _)
		{
			// Do nothing
		}
		
		[Route]
		private async UniTask On(LoadSceneCommand command)
		{
			await Addressables.LoadSceneAsync(command.Scene, LoadSceneMode.Additive);
		}
	}
}