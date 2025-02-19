using Latios;
using Survivors.Play.Authoring;
using Survivors.Play.Components;
using Survivors.Play.Input;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Survivors.Play.Systems
{
	public partial class PlayerInputReadSystem : SubSystem
	{ 
		PlayStateInput _playerStateInput;

		protected override void OnCreate()
		{
			_playerStateInput = new PlayStateInput();
			_playerStateInput.Enable();
			
		}

		protected override void OnDestroy()
		{
			_playerStateInput.Disable();
		}

		protected override void OnUpdate()
		{
			var actions = _playerStateInput.Player;

			float2 movement = actions.Move.ReadValue<Vector2>();
			
			var inputState = new PlayerInputState
			{
				Direction = movement
			};

			foreach (var playerInputState in SystemAPI.Query<RefRW<PlayerInputState>>().WithAll<PlayerTag>())
			{
				playerInputState.ValueRW = inputState;
			}
		}
	}
}