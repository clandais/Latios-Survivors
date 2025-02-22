using Latios;
using Survivors.Play.Systems;
using Survivors.Setup.Components;
using Unity.Entities;

namespace Survivors.Setup.Systems
{

	public partial class PlayerSuperSystem : SuperSystem
	{
		private EntityQuery _pauseQuery;

		protected override void CreateSystems()
		{
			GetOrCreateAndAddManagedSystem<CinemachineTargetUpdater>();
			GetOrCreateAndAddManagedSystem<PlayerInputReadSystem>();
			GetOrCreateAndAddUnmanagedSystem<PlayerDesiredMotionSystem>();
			GetOrCreateAndAddUnmanagedSystem<PlayerAnimationSystem>();

			_pauseQuery = Fluent.WithAnyEnabled<PauseRequestedTag>(true).Build();
		}

		public override bool ShouldUpdateSystem()
		{

			return _pauseQuery.IsEmptyIgnoreFilter;
		}
	}
}