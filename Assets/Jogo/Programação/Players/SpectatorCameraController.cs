using UnityEngine;
using Cinemachine;

public class SpectatorCameraController : MonoBehaviour
{
    [Header("Câmera")]
    [SerializeField] private CinemachineVirtualCamera virtualCamera;

    public void SetTarget(Transform target)
    {
        if (target == null)
            return;

        virtualCamera.Follow = target;

        Debug.Log("Câmera acompanhando: " + target.name);
    }
}