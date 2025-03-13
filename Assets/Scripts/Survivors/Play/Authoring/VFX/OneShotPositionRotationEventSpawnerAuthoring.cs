using Survivors.Play.Components;
using Survivors.Play.Components.VFXTunnels;
using Unity.Entities;
using UnityEngine;

namespace Survivors.Play.Authoring.VFX
{
    public class OneShotPositionRotationEventSpawnerAuthoring : MonoBehaviour
    {
        [SerializeField] PositionRotationGraphicsEventTunnel tunnel;
        
        private class OneShotPositionRotationEventSpawnerAuthoringBaker : Baker<OneShotPositionRotationEventSpawnerAuthoring>
        {
            public override void Bake(OneShotPositionRotationEventSpawnerAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Renderable);
                AddComponent(entity, new OneShotPositionRotationEventSpawner
                {
                    EventTunnel = new UnityObjectRef<PositionRotationGraphicsEventTunnel> { Value = authoring.tunnel },
                });
            }
        }
    }
}

