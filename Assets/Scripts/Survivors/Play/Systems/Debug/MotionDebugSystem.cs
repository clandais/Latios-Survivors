using Survivors.Play.Components;
using Survivors.Setup.Scope.Messages.GlobalMessages;
using Unity.Entities;
using Unity.Mathematics;
using VContainer;
using VitalRouter;

namespace Survivors.Play.Systems.Debug
{
	public partial class MotionDebugSystem : SystemBase
	{
		ICommandPublisher _publisher;

		[Inject]
		public void Construct(ICommandPublisher publisher)
		{
			_publisher = publisher;
		}


		protected override void OnCreate()
		{
			RequireForUpdate<PlayerTag>();
		}

		protected override void OnUpdate()
		{

			string message = "";

			Entities.ForEach((Entity entity,  AgentMotionAspect motion, in PlayerTag _) =>
			{
				message += $"[{entity.Index}]\n" +
				           $"Velocity: {motion.Velocity.xz}\n" +
				           $"Normalized Speed {motion.NormalizedSpeed}\n" +
				           $"Desired Velocity {motion.DesiredVelocity.xz}\n" +
				           $"Rotated Velocity {math.normalizesafe( math.mul(motion.Rotation, motion.Velocity).xz)}\n";

				
			}).WithoutBurst().Run();

			_publisher.PublishAsync(new DebugCommand { Message = message });
		}
	}
}