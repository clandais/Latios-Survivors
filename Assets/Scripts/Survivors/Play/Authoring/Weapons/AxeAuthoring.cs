using Survivors.Play.Components;
using Unity.Entities;
using UnityEngine;

namespace Survivors.Play.Authoring.Weapons
{
    public class AxeAuthoring : MonoBehaviour
    {
        [Header("Axe Config")]
        [SerializeField] float speed;
        [SerializeField] float rotationSpeed;
        [SerializeField] GameObject DestroyVfxPrefab;
        
        private class AxeAuthoringBaker : Baker<AxeAuthoring>
        {
            public override void Bake(AxeAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent<AxeComponent>(entity);
                AddComponent(entity, new AxeDestroyVfx
                {
                    Prefab = GetEntity(authoring.DestroyVfxPrefab, TransformUsageFlags.Dynamic)
                });
                
                AddComponent(entity, new AxeConfigComponent
                {
                    Speed         = authoring.speed,
                    RotationSpeed = authoring.rotationSpeed
                });
            }
        }
    }
}

