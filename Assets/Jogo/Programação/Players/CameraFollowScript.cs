using UnityEngine;
using Cinemachine;
using Unity.Netcode;
using System.Collections;


public class CameraFollowScript : MonoBehaviour
{
    private IEnumerator Start()
    {
        var cameraVirtual = GetComponent<CinemachineVirtualCamera>();

        while (NetworkManager.Singleton == null ||
               !NetworkManager.Singleton.IsListening ||
               NetworkManager.Singleton.LocalClient == null ||
               NetworkManager.Singleton.LocalClient.PlayerObject == null)
        {
            yield return null;
        }

        cameraVirtual.Follow =
            NetworkManager.Singleton.LocalClient.PlayerObject.transform;
    }

}
