using Latios;
using Latios.Authoring;
using Latios.Calligraphics;
using Latios.Kinemation;
using Latios.Transforms;
using Unity.Entities;
using UnityEngine.Scripting;

namespace Survivors.BootStrap
{
	[Preserve]
	public class LatiosBakingBootstrap : ICustomBakingBootstrap
	{
		public void InitializeBakingForAllWorlds(ref CustomBakingBootstrapContext context)
		{
			Latios.Authoring.CoreBakingBootstrap.ForceRemoveLinkedEntityGroupsOfLength1(ref context);
			Latios.Transforms.Authoring.TransformsBakingBootstrap.InstallLatiosTransformsBakers(ref context);
			Latios.Psyshock.Authoring.PsyshockBakingBootstrap.InstallUnityColliderBakers(ref context);
			Latios.Kinemation.Authoring.KinemationBakingBootstrap.InstallKinemation(ref context);

			Latios.Mimic.Authoring.MimicBakingBootstrap.InstallMecanimAddon(ref context);
		}
	}

#if UNITY_EDITOR
	[Preserve]
	public class LatiosEditorBootstrap : ICustomEditorBootstrap
	{
		public World InitializeOrModify(World defaultEditorWorld)
		{
			var world                        = new LatiosWorld("Latios Editor World", defaultEditorWorld.Flags);
			world.zeroToleranceForExceptions = true;

			var systems = DefaultWorldInitialization.GetAllSystemTypeIndices(WorldSystemFilterFlags.Default, true);
			BootstrapTools.InjectSystems(systems, world, world.simulationSystemGroup);

			TransformsBootstrap.InstallTransforms(world, world.simulationSystemGroup);
			KinemationBootstrap.InstallKinemation(world);
			
			CalligraphicsBootstrap.InstallCalligraphics(world);

			return world;
		}
	}
#endif

	[Preserve]
	public class LatiosBootstrap : ICustomBootstrap
	{
		public bool Initialize(string defaultWorldName)
		{
			var world                             = new LatiosWorld(defaultWorldName);
			World.DefaultGameObjectInjectionWorld = world;
			world.useExplicitSystemOrdering       = true;
			world.zeroToleranceForExceptions      = true;

			var systems = DefaultWorldInitialization.GetAllSystemTypeIndices(WorldSystemFilterFlags.Default);

			BootstrapTools.InjectUnitySystems(systems, world, world.simulationSystemGroup);

			CoreBootstrap.InstallSceneManager(world);
			Latios.Transforms.TransformsBootstrap.InstallTransforms(world, world.simulationSystemGroup);
			Latios.Myri.MyriBootstrap.InstallMyri(world);
			Latios.Kinemation.KinemationBootstrap.InstallKinemation(world);
			Latios.Mimic.MimicBootstrap.InstallMecanimAddon(world);
			Latios.Calligraphics.CalligraphicsBootstrap.InstallCalligraphics(world);
			Latios.Calligraphics.CalligraphicsBootstrap.InstallCalligraphicsAnimations(world);

			BootstrapTools.InjectRootSuperSystems(systems, world, world.simulationSystemGroup);

			world.initializationSystemGroup.SortSystems();
			world.simulationSystemGroup.SortSystems();
			world.presentationSystemGroup.SortSystems();

			ScriptBehaviourUpdateOrder.AppendWorldToCurrentPlayerLoop(world);
			return true;
		}
	}
}