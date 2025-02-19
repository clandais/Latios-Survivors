using System.Threading.Tasks;
using UnityEngine;
using VitalRouter;

namespace Survivors.Setup.Scope.Interceptors
{
	public class LoggingInterceptor : ICommandInterceptor
	{

		public async ValueTask InvokeAsync<T>(T command, PublishContext context, PublishContinuation<T> next) where T : ICommand
		{
			Debug.Log($"Start {typeof(T)}");
			
			await next(command, context);
			
			Debug.Log($"End {typeof(T)}");
		}
	}
}