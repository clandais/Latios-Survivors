using System;
using System.Threading.Tasks;
using UnityEngine;
using VitalRouter;

namespace Survivors.Setup.Scope.Interceptors
{
	public class ExceptionHandling : ICommandInterceptor
	{

		public async ValueTask InvokeAsync<T>(T command, PublishContext context, PublishContinuation<T> next) where T : ICommand
		{
			try
			{
				await next(command, context);
			}
			catch (Exception e)
			{
				Debug.LogException(e);
			}
		}
	}
}