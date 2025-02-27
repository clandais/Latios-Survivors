using VitalRouter;

namespace Survivors.Setup.Scope.Messages.GlobalMessages
{

	public struct MainMenuStateCommand : ICommand { }
	
	public struct PlayStateCommand : ICommand { }
	
	public struct RequestPauseStateCommand : ICommand { }
	public struct RequestResumeStateCommand : ICommand { }
	
	
	public interface IDebugCommand : ICommand {}
	
	public struct DebugCommand : IDebugCommand
	{
		public string Message;
	}
}