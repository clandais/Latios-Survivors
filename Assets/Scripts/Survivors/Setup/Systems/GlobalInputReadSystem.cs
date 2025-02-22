using Latios;
using R3;
using Survivors.Play.Components;
using Survivors.Setup.Components;
using Survivors.Setup.Scope.Messages.GlobalMessages;
using Unity.Entities;
using VContainer;
using VitalRouter;

namespace Survivors.Setup.Systems
{
	public partial class GlobalInputReadSystem : SubSystem
	{
		private ICommandPublisher _commandPublisher;
		protected ICommandSubscribable _commandSubscribable;
		private readonly ReactiveProperty<bool> _isEscapePressed = new();
		private DisposableBag _disposableBag;

		[Inject]
		public void Construct(ICommandPublisher commandPublisher, ICommandSubscribable commandSubscribable)
		{

			_commandPublisher = commandPublisher;
			_commandSubscribable = commandSubscribable;

			_commandSubscribable.Subscribe<RequestResumeStateCommand>((command, context) =>
			{
				worldBlackboardEntity.RemoveComponent<PauseRequestedTag>();
			});
			
			_isEscapePressed.Subscribe(OnEscapePressed)
				.AddTo(ref _disposableBag);
		}

		private void OnEscapePressed(bool pressed)
		{

			if (!pressed)
			{
				return;
			}

			if (worldBlackboardEntity.HasComponent<PauseRequestedTag>())
			{
				worldBlackboardEntity.RemoveComponent<PauseRequestedTag>();
				_commandPublisher.PublishAsync(new RequestResumeStateCommand());
			}
			else
			{
				worldBlackboardEntity.AddComponent<PauseRequestedTag>();
				_commandPublisher.PublishAsync(new RequestPauseStateCommand());
			}
		}

		private InputSystem_Actions _inputSystemActions;

		private EntityQuery _shouldUpdateQuery;

		protected override void OnCreate()
		{
			_inputSystemActions = new InputSystem_Actions();
			_inputSystemActions.Enable();

			_shouldUpdateQuery = Fluent.WithAnyEnabled<PlayerTag>(true).Build();
		}

		protected override void OnDestroy()
		{
			_disposableBag.Dispose();
			_inputSystemActions.Disable();
		}

		protected override void OnUpdate()
		{
			InputSystem_Actions.GlobalsActions actions = _inputSystemActions.Globals;
			_isEscapePressed.Value = actions.Escape.ReadValue<float>() > 0.1f;
		}


		protected override void OnStopRunning()
		{


		}

		public override bool ShouldUpdateSystem()
		{
			return !_shouldUpdateQuery.IsEmptyIgnoreFilter;
		}
	}
}