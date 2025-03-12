using Survivors.Play.Components;
using Survivors.Play.Components.VFXTunnels;
using Unity.Entities;
using UnityEngine;

namespace Survivors.Play.Authoring.VFX
{
    public class PositionEventSpawnerAuthoring : MonoBehaviour
    {
        [SerializeField] float period = 1f;
        [SerializeField] PositionGraphicsEventTunnel tunnel;
         
        private class PositionEventSpawnerAuthoringBaker : Baker<PositionEventSpawnerAuthoring>
        {
            public override void Bake(PositionEventSpawnerAuthoring authoring)
            {

                var entity = GetEntity(TransformUsageFlags.Renderable);
                AddComponent(entity, new PositionEventSpawner
                {
                    EventTunnel = authoring.tunnel,
                    TimeBetweenSpawns = authoring.period,
                });
            }
        }
    }
}