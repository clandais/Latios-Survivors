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
		EntityQuery m_pauseQuery;
		EntityQuery m_playerQuery;

		protected override void CreateSystems()
		{

			GetOrCreateAndAddManagedSystem<CinemachineTargetUpdater>();
			GetOrCreateAndAddManagedSystem<PlayerInputReadSystem>();
			GetOrCreateAndAddManagedSystem<MotionDebugSystem>();
			GetOrCreateAndAddUnmanagedSystem<PlayerDesiredMotionSystem>();
			GetOrCreateAndAddUnmanagedSystem<PlayerFourDirectionsAnimationSystem>();

			m_pauseQuery  = Fluent.WithAnyEnabled<PauseRequestedTag>(true).Build();
			m_playerQuery = Fluent.With<PlayerTag>().Build();
		}

		public override bool ShouldUpdateSystem()
		{

			return m_pauseQuery.IsEmptyIgnoreFilter && !m_playerQuery.IsEmptyIgnoreFilter;
		}
	}

	public partial class PhysicsSuperSystem : SuperSystem
	{
		EntityQuery m_pauseQuery;

		protected override void CreateSystems()
		{
			GetOrCreateAndAddUnmanagedSystem<BuildEnvironmentCollisionLayerSystem>();
			GetOrCreateAndAddUnmanagedSystem<PlayerMovementSystem>();
			GetOrCreateAndAddUnmanagedSystem<PhysicsDebugSystem>();

			m_pauseQuery = Fluent.WithAnyEnabled<PauseRequestedTag>(true).Build();
		}

		public override bool ShouldUpdateSystem()
		{

			return m_pauseQuery.IsEmptyIgnoreFilter;
		}
	}
}