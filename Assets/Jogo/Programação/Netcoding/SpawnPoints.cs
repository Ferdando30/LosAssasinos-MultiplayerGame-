
using UnityEngine;
using Unity.Netcode;
using System.Collections;
using System.Collections.Generic;

public class PlayerSpawnManager : NetworkBehaviour
{
    [SerializeField] private Transform[] spawnPoints;

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            NetworkManager.Singleton.SceneManager.OnLoadEventCompleted +=
                OnSceneLoaded;
        }
    }

    public override void OnNetworkDespawn()
    {
        if (IsServer)
        {
            NetworkManager.Singleton.SceneManager.OnLoadEventCompleted -=
                OnSceneLoaded;
        }
    }

    private void OnSceneLoaded(
        string sceneName,
        UnityEngine.SceneManagement.LoadSceneMode loadSceneMode,
        List<ulong> clientsCompleted,
        List<ulong> clientsTimedOut)
    {
        if (!IsServer)
            return;

        StartCoroutine(PositionPlayers(clientsCompleted));
    }

    private IEnumerator PositionPlayers(List<ulong> clientsCompleted)
    {
        yield return new WaitForSeconds(1f);

        int index = 0;

        foreach (ulong id in clientsCompleted)
        {
            Transform spawn = spawnPoints[index % spawnPoints.Length];

            ClientRpcParams rpcParams = new ClientRpcParams
            {
                Send = new ClientRpcSendParams
                {
                    TargetClientIds = new ulong[] { id }
                }
            };

            SetPlayerPositionClientRpc(
                id,
                spawn.position,
                spawn.rotation,
                rpcParams
            );

            index++;
        }
    }

    [ClientRpc]
    private void SetPlayerPositionClientRpc(
        ulong clientId,
        Vector3 position,
        Quaternion rotation,
        ClientRpcParams rpcParams = default)
    {
        if (NetworkManager.Singleton.LocalClientId != clientId)
            return;

        NetworkObject player =
            NetworkManager.Singleton.SpawnManager.GetLocalPlayerObject();

        if (player == null)
            return;

        player.transform.SetPositionAndRotation(position, rotation);

        Debug.Log("Player " + clientId + " foi posicionado em " + position);
    }
}
