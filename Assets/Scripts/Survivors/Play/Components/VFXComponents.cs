using Survivors.Play.Components.VFXTunnels;
using Unity.Entities;

namespace Survivors.Play.Components
{

    public struct PositionEventSpawner : IComponentData
    {
        public float TimeUntilNextSpawn;
        public float TimeBetweenSpawns; 
        public UnityObjectRef<PositionGraphicsEventTunnel> EventTunnel;
    }
}