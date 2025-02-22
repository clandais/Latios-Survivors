using System.Threading.Tasks;
using Survivors.Setup.Scope.Messages.GlobalMessages;
using VitalRouter;

namespace Survivors.Setup.Scope.Interceptors
{
	public class GameStateInterceptor : ICommandInterceptor
	{

		private bool _isMainMenuState;
		private bool _isPlayState;
		
		public async ValueTask InvokeAsync<T>(T command, PublishContext context, PublishContinuation<T> next) where T : ICommand
		{

			switch (command)
			{
				case MainMenuStateCommand when _isMainMenuState:
				case PlayStateCommand when _isPlayState:
					return;
				default:
					await next(command, context);
					break;
			}


			switch (command)
			{
				case MainMenuStateCommand when !_isMainMenuState:
					_isMainMenuState = true;
					_isPlayState = false;
					break;
				case PlayStateCommand when !_isPlayState:
					_isPlayState = true;
					_isMainMenuState = false;
					break;
			}

		}
	}
}