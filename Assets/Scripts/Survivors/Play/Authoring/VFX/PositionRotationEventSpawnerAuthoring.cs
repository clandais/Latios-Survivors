using Survivors.Play.Components;
using Survivors.Play.Components.VFXTunnels;
using Unity.Entities;
using UnityEngine;

namespace Survivors.Play.Authoring.VFX
{
    public class PositionRotationEventSpawnerAuthoring : MonoBehaviour
    {
        [SerializeField] float period = 1f;
        [SerializeField] PositionRotationGraphicsEventTunnel tunnel;
        
        private class PositionRotationEventSpawnerAuthoringBaker : Baker<PositionRotationEventSpawnerAuthoring>
        {
            public override void Bake(PositionRotationEventSpawnerAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Renderable);
                AddComponent(entity, new PositionRotationEventSpawner
                {
                    EventTunnel = new UnityObjectRef<PositionRotationGraphicsEventTunnel> { Value = authoring.tunnel },
                    TimeBetweenSpawns = authoring.period,
                    TimeUntilNextSpawn = authoring.period,
                });
            }
        }
    }
}

