using Unity.Entities;
using UnityEngine;

namespace Survivors.Play.Authoring.Level
{
    public class FloorTileAuthoring : MonoBehaviour
    {
        private class FloorTileAuthoringBaker : Baker<FloorTileAuthoring>
        {
            public override void Bake(FloorTileAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Renderable);
                AddComponent<FloorTileTag>(entity);
            }
        }
    }
    
    public struct FloorTileTag : IComponentData { }
}