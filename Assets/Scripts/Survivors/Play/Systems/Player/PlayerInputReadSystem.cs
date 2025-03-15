using Latios;
using Survivors.Play.Authoring.Weapons;
using Survivors.Play.Components;
using Survivors.Play.Input;
using Survivors.Play.Scope.Messages;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using VContainer;
using VitalRouter;

namespace Survivors.Play.Systems.Player
{

	public partial class PlayerInputReadSystem : SubSystem
	{

		private Image _crosshair;
		private ICommandPublisher _commandPublisher;

		[Inject]
		public void Construct(Image crosshair, ICommandPublisher commandPublisher)
		{
			_crosshair = crosshair;
			_commandPublisher = commandPublisher;
		}
		

		PlayStateInput _playerStateInput;

		bool _attackTriggered;

		protected override void OnCreate()
		{
			_playerStateInput = new PlayStateInput();
			_playerStateInput.Enable();

			_playerStateInput.Player.Attack.performed += AttackPerformed;

		}

		void AttackPerformed(InputAction.CallbackContext _)
		{
			_attackTriggered = true;
		}

		protected override void OnDestroy()
		{
			_playerStateInput.Player.Attack.performed -= AttackPerformed;
			_playerStateInput.Disable();
		}

		protected override void OnUpdate()
		{


			PlayStateInput.PlayerActions actions = _playerStateInput.Player;

			float2 movement      = actions.Move.ReadValue<Vector2>();
			bool   isSprinting   = actions.Sprint.ReadValue<float>() > 0.1f;
			float2 mousePosition = actions.MouseMoved.ReadValue<Vector2>();
			float scrollValue   = actions.Zoom.ReadValue<float>();

			if (math.abs(scrollValue) > math.EPSILON)
				_commandPublisher.PublishAsync(new PlayerScrollCommand
				{
					ScrollValue = scrollValue,
				});
			
			
			_crosshair.rectTransform.position = new float3(mousePosition.x, mousePosition.y, 0);

			var mousePositionComponent = sceneBlackboardEntity.GetComponentData<SceneMouse>();
			mousePositionComponent.Position = mousePosition;

			Ray ray = Camera.main.ScreenPointToRay(new Vector3(mousePosition.x, mousePosition.y, 0));

			var plane = new Plane(Vector3.up, Vector3.zero);

			if (plane.Raycast(ray, out float enter))
			{
				Vector3 hitPoint = ray.GetPoint(enter);
				mousePosition                        = new float2(hitPoint.x, hitPoint.z);
				mousePositionComponent.WorldPosition = mousePosition;
			}
			else
			{
				mousePosition                        = new float2(0, 0);
				mousePositionComponent.WorldPosition = mousePosition;
			}


			var inputState = new PlayerInputState
			{
				Direction           = movement,
				IsSprinting         = isSprinting,
				MousePosition       = mousePosition,
				MainAttackTriggered = _attackTriggered
			};


			sceneBlackboardEntity.SetComponentData(mousePositionComponent);

			_attackTriggered = false;

			foreach (var playerInputState in SystemAPI.Query<RefRW<PlayerInputState>>().WithAll<PlayerTag>())
			{
				playerInputState.ValueRW = inputState;
			}
		}
	}
}