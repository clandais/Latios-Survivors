using Latios;
using Latios.Systems;
using Latios.Transforms.Systems;
using Survivors.Play.Systems;
using Survivors.Play.Systems.Enemies;
using Unity.Entities;

namespace Survivors.Setup.Systems
{
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public partial class InitializationSuperSystem : RootSuperSystem
    {
        protected override void CreateSystems()
        {
            GetOrCreateAndAddUnmanagedSystem<EnemySpawnerSystem>();
            // GetOrCreateAndAddUnmanagedSystem<PlayerInitializationSystem>();
        }
    }

    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public partial class BootstrapSuperSystem : RootSuperSystem
    {
        protected override void CreateSystems()
        {
            GetOrCreateAndAddManagedSystem<GlobalInputReadSystem>();
            GetOrCreateAndAddManagedSystem<PlayerSuperSystem>();
            GetOrCreateAndAddManagedSystem<EnemySuperSystem>();
        }
    }

    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateBefore(typeof(TransformSuperSystem))]
    public partial class AnimationSuperSystem : RootSuperSystem
    {
        protected override void CreateSystems()
        {
            GetOrCreateAndAddManagedSystem<PlayerAnimationSuperSystem>();
            GetOrCreateAndAddManagedSystem<EnemyAnimationSuperSystem>();
        }
    }

    [UpdateInGroup(typeof(FixedSimulationSystemGroup))]
    public partial class PhysicsRootSystem : RootSuperSystem
    {
        protected override void CreateSystems()
        {
            GetOrCreateAndAddManagedSystem<PhysicsSuperSystem>();
            GetOrCreateAndAddManagedSystem<EnemyPhysicsSuperSystem>();
        }
    }
}