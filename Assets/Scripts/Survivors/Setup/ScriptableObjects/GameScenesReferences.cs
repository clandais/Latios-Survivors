using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Survivors.Setup.ScriptableObjects
{
	[CreateAssetMenu(fileName = "GameScenesReferences", menuName = "Survivors/Setup/GameScenesReferences")]
	public class GameScenesReferences : ScriptableObject
	{
		[SerializeField] public AssetReference mainMenuScene;
		[SerializeField] public AssetReference playScene;
		
	}
}