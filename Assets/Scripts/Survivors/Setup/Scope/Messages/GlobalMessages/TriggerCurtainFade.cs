using VitalRouter;

namespace Survivors.Setup.Scope.Messages.GlobalMessages
{
	public struct TriggerCurtainFade : ICommand
	{
		public float FromAlpha;
		public float ToAlpha;
		public float Duration;
	}
}