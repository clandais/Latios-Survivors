
using System.Threading.Tasks;
using Survivors.Setup.Scope.Messages.GlobalMessages;
using UnityEngine;
using VitalRouter;

namespace Survivors.Setup.Scope.Interceptors
{
	public class LoggingInterceptor : ICommandInterceptor
	{

		public async ValueTask InvokeAsync<T>(T command, PublishContext context, PublishContinuation<T> next) where T : ICommand
		{

			if (command is IDebugCommand)
			{
				await next(command, context);
				return;
			}
			
			Debug.Log($"Start {typeof(T)}");
			await next(command, context);
			Debug.Log($"End {typeof(T)}");
		}
	}
}