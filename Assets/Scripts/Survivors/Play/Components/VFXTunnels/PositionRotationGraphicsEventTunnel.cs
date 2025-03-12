using Latios.LifeFX;
using UnityEngine;
using UnityEngine.VFX;

namespace Survivors.Play.Components.VFXTunnels
{
    
    [CreateAssetMenu(fileName = "PositionRotationGraphicsEventTunnel", menuName = "Survivors/VFXTunnels/PositionRotationGraphicsEventTunnel")]
    public class PositionRotationGraphicsEventTunnel : GraphicsEventTunnel<PositionRotationEventInput>
    {
    }

    [VFXType(VFXTypeAttribute.Usage.GraphicsBuffer)]
    public struct PositionRotationEventInput
    {
        public Vector3 Position;
        public Vector3 Rotation;
    }
}
