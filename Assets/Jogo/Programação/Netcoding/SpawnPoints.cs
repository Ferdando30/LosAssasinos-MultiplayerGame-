using UnityEngine;
using Unity.Netcode;
using System.Collections;

public class PlayerSpawnManager : NetworkBehaviour
{
    [SerializeField] private Transform[] spawnPoints;

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            StartCoroutine(SpawnPlayers());
        }
    }

    private IEnumerator SpawnPlayers()
    {
        // Espera a cena terminar de inicializar
        yield return null;

        int index = 0;

        foreach (NetworkClient client in
                 NetworkManager.Singleton.ConnectedClientsList)
        {
            if (client.PlayerObject == null)
                continue;

            Transform spawn = spawnPoints[index % spawnPoints.Length];

            client.PlayerObject.transform.SetPositionAndRotation(
                spawn.position,
                spawn.rotation
            );

            index++;
        }
    }
}
