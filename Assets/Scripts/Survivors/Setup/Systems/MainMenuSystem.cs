using Latios;
using Survivors.Setup.Scope;
using Unity.Burst;
using Unity.Entities;
using VContainer;
using VitalRouter;

namespace Survivors.Setup.Systems
{
	
	public partial class MainMenuManagedSystem : SystemBase
	{

		private ICommandPublisher _commandPublisher;

		[Inject]
		public void Construct(ICommandPublisher commandPublisher)
		{
			_commandPublisher = commandPublisher;
		}


		protected override void OnCreate()
		{
			
		}

		protected override void OnUpdate()
		{
			_commandPublisher.PublishAsync(new PingMessage { Message = "MainMenuManagedSystem OnCreate" });
		}
	}

	public partial struct MainMenuSystem : ISystem
	{

		private LatiosWorldUnmanaged m_world;


		[BurstCompile]
		public void OnCreate(ref SystemState state)
		{
			m_world = state.GetLatiosWorldUnmanaged();
		}

		[BurstCompile]
		public void OnUpdate(ref SystemState state)
		{

		}

		[BurstCompile]
		public void OnDestroy(ref SystemState state)
		{

		}
	}
}