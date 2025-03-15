using VitalRouter;

namespace Survivors.Setup.Scope.Messages.GlobalMessages
{
    public struct CameraZoomCommand : ICommand
    {
        public float ZoomValue;
    }
}