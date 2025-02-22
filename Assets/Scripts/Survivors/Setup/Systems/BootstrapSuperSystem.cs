using Latios;
using Latios.Systems;
using Latios.Transforms.Systems;
using Unity.Entities;
using VContainer;

namespace Survivors.Setup.Systems
{

	[UpdateAfter(typeof(TransformSuperSystem))]
	public partial class BootstrapSuperSystem : RootSuperSystem
	{


		protected override void CreateSystems()
		{
			GetOrCreateAndAddManagedSystem<GlobalInputReadSystem>();
			GetOrCreateAndAddManagedSystem<PlayerSuperSystem>();
		}
	}
}