using Unity.Cinemachine;
using UnityEngine;

namespace Survivors.Setup.MonoBehaviours
{
    public class CinemachineBehaviour : MonoBehaviour
    {
        [SerializeField] Transform target;
        [SerializeField] CinemachineSplineDolly dolly;

        public void SetPosition(Vector3 position)
        {
            target.position = position;
        }

        public void SetZoom(float commandZoomValue)
        {
            dolly.CameraPosition -= commandZoomValue;
        }
    }
}