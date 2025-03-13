using Survivors.Play.Components;
using Unity.Entities;
using UnityEngine;

namespace Survivors.Play.Authoring.Weapons
{
    public class AxeAuthoring : MonoBehaviour
    {
        [SerializeField] GameObject DestroyVfxPrefab;
        
        private class AxeAuthoringBaker : Baker<AxeAuthoring>
        {
            public override void Bake(AxeAuthoring authoring)
            {
                var entiy = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent<AxeComponent>(entiy);
                AddComponent(entiy, new AxeDestroyVfx
                {
                    Prefab = GetEntity(authoring.DestroyVfxPrefab, TransformUsageFlags.Dynamic)
                });
            }
        }
    }
}

