using Latios;
using Latios.Transforms;
using Survivors.Play.Authoring;
using Survivors.Play.Authoring.Weapons;
using Survivors.Play.Components;
using Survivors.Setup.MonoBehaviours;
using Unity.Entities;
using UnityEngine;
using VContainer;

namespace Survivors.Play.Systems
{
	[RequireMatchingQueriesForUpdate]
	public partial class CinemachineTargetUpdater : SubSystem
	{

		private CinemachineBehaviour _cinemachine;
		
		[Inject]
		public void Construct(CinemachineBehaviour cinemachine)
		{
			_cinemachine = cinemachine;
		}

		protected override void OnCreate()
		{
			RequireForUpdate<PlayerTag>();
		}


		protected override void OnUpdate()
		{


			var playerPosition = sceneBlackboardEntity.GetComponentData<PlayerPosition>();
			_cinemachine.SetPosition( playerPosition.Position );
		}
	}
}