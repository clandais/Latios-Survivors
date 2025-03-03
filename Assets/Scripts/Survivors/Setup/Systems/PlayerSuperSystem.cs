using Latios;
using Survivors.Play.Components;
using Survivors.Play.Systems;
using Survivors.Play.Systems.Debug;
using Survivors.Play.Systems.Weapons;
using Survivors.Setup.Components;
using Unity.Entities;

namespace Survivors.Setup.Systems
{


	public abstract partial class BaseGamePlaySystem : SuperSystem
	{
		EntityQuery m_pauseQuery;
		EntityQuery m_playerQuery;

		
		protected void CreateQueries()
		{
			m_pauseQuery  = Fluent.WithAnyEnabled<PauseRequestedTag>(true).Build();
			m_playerQuery = Fluent.With<PlayerTag>().Build();
		}
		
		

		public override bool ShouldUpdateSystem()
		{
			return m_pauseQuery.IsEmptyIgnoreFilter && !m_playerQuery.IsEmptyIgnoreFilter;
		}
	}
	
	public partial class PlayerSuperSystem : BaseGamePlaySystem
	{

		protected override void CreateSystems()
		{

			CreateQueries();
			
			GetOrCreateAndAddManagedSystem<CinemachineTargetUpdater>();
			GetOrCreateAndAddManagedSystem<PlayerInputReadSystem>();
			GetOrCreateAndAddManagedSystem<MotionDebugSystem>();
			GetOrCreateAndAddUnmanagedSystem<PlayerDesiredMotionSystem>();
			GetOrCreateAndAddUnmanagedSystem<PlayerActionsSystem>();

			GetOrCreateAndAddUnmanagedSystem<AxeSpawnSystem>();

		}
		
	}

	public partial class PlayerAnimationSuperSystem : BaseGamePlaySystem
	{

		protected override void CreateSystems()
		{
			CreateQueries();
			// Animation Systems
			GetOrCreateAndAddUnmanagedSystem<PlayerFourDirectionsAnimationSystem>();
			GetOrCreateAndAddUnmanagedSystem<PlayerActionsAnimationSystem>();
		}
	}

	public partial class PhysicsSuperSystem : BaseGamePlaySystem
	{


		protected override void CreateSystems()
		{
			
			CreateQueries();
			
			GetOrCreateAndAddUnmanagedSystem<BuildEnvironmentCollisionLayerSystem>();
			GetOrCreateAndAddUnmanagedSystem<PlayerMovementSystem>();
			GetOrCreateAndAddUnmanagedSystem<PhysicsDebugSystem>();
			GetOrCreateAndAddUnmanagedSystem<AxeUpdateSystem>();

		}


	}
}