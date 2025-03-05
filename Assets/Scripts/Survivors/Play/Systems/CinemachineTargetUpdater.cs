using Latios;
using Latios.Transforms;
using Survivors.Play.Authoring;
using Survivors.Play.Authoring.Weapons;
using Survivors.Play.Components;
using Unity.Entities;
using UnityEngine;
using VContainer;

namespace Survivors.Play.Systems
{
	[RequireMatchingQueriesForUpdate]
	public partial class CinemachineTargetUpdater : SubSystem
	{

		private Transform _cinemachineTarget;
		
		[Inject]
		public void Construct(Transform cinemachineTarget)
		{
			_cinemachineTarget = cinemachineTarget;
		}

		protected override void OnCreate()
		{
			RequireForUpdate<PlayerTag>();
		}


		protected override void OnUpdate()
		{


			var playerPosition = sceneBlackboardEntity.GetComponentData<PlayerPosition>();
			_cinemachineTarget.position = playerPosition.Position;
			//
			// foreach (var worldTransform in SystemAPI.Query<RefRO<WorldTransform>>()
			// 	         .WithPresent<PlayerTag>())
			// {
			// 	_cinemachineTarget.position = worldTransform.ValueRO.worldTransform.position;
			// }
		}
	}
}