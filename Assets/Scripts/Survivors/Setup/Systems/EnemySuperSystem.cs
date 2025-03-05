using Survivors.Play.Systems;
using Survivors.Play.Systems.Enemies;

namespace Survivors.Setup.Systems
{
	public partial class EnemySuperSystem : BaseGamePlaySystem
	{

		protected override void CreateSystems()
		{
			CreateQueries();
			GetOrCreateAndAddUnmanagedSystem<EnemyFollowDesiredMotionSystem>();
			GetOrCreateAndAddUnmanagedSystem<DisableDeadCollidersSystem>();

		}
	}
	
	public partial class EnemyAnimationSuperSystem : BaseGamePlaySystem
	{

		protected override void CreateSystems()
		{
			CreateQueries();
			
			GetOrCreateAndAddUnmanagedSystem<SkeletonDeathAnimationSystem>();
		}
	}

	public partial class EnemyPhysicsSuperSystem : BaseGamePlaySystem
	{

		protected override void CreateSystems()
		{
			CreateQueries();
			GetOrCreateAndAddUnmanagedSystem<BuildEnemyCollisionLayerSystem>();
		}
	}
}