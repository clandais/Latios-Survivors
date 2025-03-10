using Unity.Entities;
using UnityEngine;

namespace Survivors.Play.Authoring.Level
{
    public class WallTileAuthoring : MonoBehaviour
    {
        private class WallTileAuthoringBaker : Baker<WallTileAuthoring>
        {
            public override void Bake(WallTileAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Renderable);
                AddComponent<WallTileTag>(entity);
            }
        }
    }
    
    public struct WallTileTag : IComponentData { }
}