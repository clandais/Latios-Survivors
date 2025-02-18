using Latios;
using Latios.Systems;
using Unity.Entities;

namespace Survivors.Setup.Systems
{
	[UpdateInGroup(typeof(LatiosWorldSyncGroup), OrderFirst = true)]
	[UpdateAfter(typeof(MergeBlackboardsSystem))]
	public partial class BootstrapSuperSystem : RootSuperSystem
	{

		protected override void CreateSystems()
		{
			GetOrCreateAndAddUnmanagedSystem<MainMenuSystem>();
			GetOrCreateAndAddManagedSystem<MainMenuManagedSystem>();
		}
	}
}