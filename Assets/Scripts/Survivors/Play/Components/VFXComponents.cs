using Latios;
using Survivors.Play.Components.VFXTunnels;
using Unity.Entities;
using Unity.Mathematics;

namespace Survivors.Play.Components
{

    public struct PositionEventSpawner : IComponentData
    {
        public float TimeUntilNextSpawn;
        public float TimeBetweenSpawns; 
        public UnityObjectRef<PositionGraphicsEventTunnel> EventTunnel;
    }

    public struct OneShotPositionRotationEventSpawner : IComponentData
    {
        public UnityObjectRef<PositionRotationGraphicsEventTunnel> EventTunnel;
    }
    
    
    public struct PositionInitialVelocityVFX : IComponentData
    {
	
        public float3 Position;
        public float3 InitialVelocity;
    }
    
    
    public struct AxeDestroyVfx : IComponentData
    {
        public EntityWith<Prefab> Prefab;
    }
    
    public struct SkeletonDestroyVfx : IComponentData
    {
        public EntityWith<Prefab> Prefab;
    }
}