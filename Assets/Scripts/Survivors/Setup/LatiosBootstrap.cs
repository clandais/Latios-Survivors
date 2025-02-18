using Latios;
using Latios.Authoring;
using Latios.Calligraphics;
using Latios.Kinemation;
using Latios.Kinemation.Authoring;
using Latios.Myri;
using Latios.Psyshock.Authoring;
using Latios.Transforms;
using Latios.Transforms.Authoring;
using Unity.Entities;
using UnityEngine.Scripting;

namespace Survivors.Setup
{
	[Preserve]
	public class LatiosBakingBootstrap : ICustomBakingBootstrap
	{
		public void InitializeBakingForAllWorlds(ref CustomBakingBootstrapContext context)
		{
			//Latios.Authoring.CoreBakingBootstrap.ForceRemoveLinkedEntityGroupsOfLength1(ref context);
			TransformsBakingBootstrap.InstallLatiosTransformsBakers(ref context);
			PsyshockBakingBootstrap.InstallUnityColliderBakers(ref context);
			KinemationBakingBootstrap.InstallKinemation(ref context);
			//Latios.Mimic.Authoring.MimicBakingBootstrap.InstallMecanimAddon(ref context);
		}
	}

	[Preserve]
	public class LatiosEditorBootstrap : ICustomEditorBootstrap
	{
		public World InitializeOrModify(World defaultEditorWorld)
		{
			var world = new LatiosWorld(defaultEditorWorld.Name, defaultEditorWorld.Flags);

			var systems = DefaultWorldInitialization.GetAllSystemTypeIndices(WorldSystemFilterFlags.Default, true);
			BootstrapTools.InjectUnitySystems(systems, world, world.simulationSystemGroup);

			TransformsBootstrap.InstallTransforms(world, world.simulationSystemGroup);
			KinemationBootstrap.InstallKinemation(world);
			CalligraphicsBootstrap.InstallCalligraphics(world);

			BootstrapTools.InjectUserSystems(systems, world, world.simulationSystemGroup);

			return world;
		}
	}

	[Preserve]
	public class LatiosBootstrap : ICustomBootstrap
	{
		public bool Initialize(string defaultWorldName)
		{
			var world = new LatiosWorld(defaultWorldName);
			World.DefaultGameObjectInjectionWorld = world;
			world.useExplicitSystemOrdering = true;

			var systems = DefaultWorldInitialization.GetAllSystemTypeIndices(WorldSystemFilterFlags.Default);

			BootstrapTools.InjectUnitySystems(systems, world, world.simulationSystemGroup);


			CoreBootstrap.InstallSceneManager(world);

			TransformsBootstrap.InstallTransforms(world, world.simulationSystemGroup);
			MyriBootstrap.InstallMyri(world);
			KinemationBootstrap.InstallKinemation(world);
			//Latios.Mimic.MimicBootstrap.InstallMecanimAddon(world);
			CalligraphicsBootstrap.InstallCalligraphics(world);
			CalligraphicsBootstrap.InstallCalligraphicsAnimations(world);

			BootstrapTools.InjectRootSuperSystems(systems, world, world.simulationSystemGroup);

			world.initializationSystemGroup.SortSystems();
			world.simulationSystemGroup.SortSystems();
			world.presentationSystemGroup.SortSystems();

			ScriptBehaviourUpdateOrder.AppendWorldToCurrentPlayerLoop(world);
			return true;
		}
	}
}