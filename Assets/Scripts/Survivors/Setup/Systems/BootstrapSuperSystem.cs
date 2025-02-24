using Latios;
using Latios.Systems;
using Latios.Transforms.Systems;
using Survivors.Play.Systems.Debug;
using Unity.Entities;
using VContainer;

namespace Survivors.Setup.Systems
{

	public partial class BootstrapSuperSystem : RootSuperSystem
	{


		protected override void CreateSystems()
		{
			GetOrCreateAndAddManagedSystem<GlobalInputReadSystem>();
			GetOrCreateAndAddManagedSystem<PlayerSuperSystem>();
		}
	}
	
	[UpdateInGroup(typeof(FixedSimulationSystemGroup))]
	public partial class PhysicsRootSystem : RootSuperSystem
	{

		protected override void CreateSystems()
		{
			GetOrCreateAndAddManagedSystem<PhysicsSuperSystem>();
		}
	}
	
}