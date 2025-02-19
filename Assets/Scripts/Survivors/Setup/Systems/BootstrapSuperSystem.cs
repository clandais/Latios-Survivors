using Latios;
using Latios.Systems;
using Latios.Transforms.Systems;
using Survivors.Play.Systems;
using Unity.Entities;

namespace Survivors.Setup.Systems
{

	[UpdateAfter(typeof(TransformSuperSystem))]
	public partial class BootstrapSuperSystem : RootSuperSystem
	{

		protected override void CreateSystems()
		{
			GetOrCreateAndAddManagedSystem<PlayerSuperSystem>();
		}
	}
}