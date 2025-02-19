using Latios;
using Latios.Transforms;
using Survivors.Play.Authoring;
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


		protected override void OnUpdate()
		{
			foreach (var worldTransform in SystemAPI.Query<RefRO<WorldTransform>>().WithAll<PlayerTag>())
			{
				_cinemachineTarget.position = worldTransform.ValueRO.worldTransform.position;
			}
		}
	}
}