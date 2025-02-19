using Latios;
using Latios.Transforms.Systems;
using Survivors.Play.Systems;
using Unity.Entities;
using UnityEngine;

namespace Survivors.Setup.Systems
{
	
	public partial class PlayerSuperSystem : SuperSystem
	{

		protected override void CreateSystems()
		{
			GetOrCreateAndAddManagedSystem<CinemachineTargetUpdater>();
			GetOrCreateAndAddManagedSystem<PlayerInputReadSystem>();
			GetOrCreateAndAddUnmanagedSystem<PlayerControllerSystem>();
		}
	}
}