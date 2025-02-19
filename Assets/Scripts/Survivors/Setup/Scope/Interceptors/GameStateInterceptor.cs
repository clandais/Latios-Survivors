using System.Threading.Tasks;
using VitalRouter;

namespace Survivors.Setup.Scope.Interceptors
{
	public class GameStateInterceptor : ICommandInterceptor
	{

		public async ValueTask InvokeAsync<T>(T command, PublishContext context, PublishContinuation<T> next) where T : ICommand
		{
			await next(command, context);
		}
	}
}