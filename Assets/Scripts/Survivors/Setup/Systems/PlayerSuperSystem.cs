using Latios;
using Survivors.Play.Components;
using Survivors.Play.Systems;
using Survivors.Play.Systems.Debug;
using Survivors.Setup.Components;
using Unity.Entities;

namespace Survivors.Setup.Systems
{

	public partial class PlayerSuperSystem : SuperSystem
	{
		private EntityQuery _pauseQuery;
		private EntityQuery _playerQuery;

		protected override void CreateSystems()
		{

			
			GetOrCreateAndAddManagedSystem<CinemachineTargetUpdater>();
			GetOrCreateAndAddManagedSystem<PlayerInputReadSystem>();
			GetOrCreateAndAddUnmanagedSystem<PlayerDesiredMotionSystem>();
			GetOrCreateAndAddUnmanagedSystem<PlayerAnimationSystem>();


			_pauseQuery = Fluent.WithAnyEnabled<PauseRequestedTag>(true).Build();
			_playerQuery = Fluent.With<PlayerTag>().Build();
		}

		public override bool ShouldUpdateSystem()
		{

			return _pauseQuery.IsEmptyIgnoreFilter && !_playerQuery.IsEmptyIgnoreFilter;
		}
	}
	

	public partial class PhysicsSuperSystem : SuperSystem
	{
		private EntityQuery _pauseQuery;
		protected override void CreateSystems()
		{
			GetOrCreateAndAddUnmanagedSystem<BuildEnvironmentCollisionLayerSystem>();
			GetOrCreateAndAddUnmanagedSystem<PlayerMovementSystem>();
			GetOrCreateAndAddUnmanagedSystem<PhysicsDebugSystem>();
			
			_pauseQuery = Fluent.WithAnyEnabled<PauseRequestedTag>(true).Build();
		}
		
		public override bool ShouldUpdateSystem()
		{

			return _pauseQuery.IsEmptyIgnoreFilter;
		}
	}
}