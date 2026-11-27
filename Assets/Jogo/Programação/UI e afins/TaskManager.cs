using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class TaskManager : NetworkBehaviour
{
    public List<string> tasks;

    public NetworkList<bool> completedTasks = new NetworkList<bool>();

    public GameObject assassinClothes;

    private bool allTasksCompleted = false;

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            for (int i = 0; i < tasks.Count; i++)
            {
                completedTasks.Add(false);
            }
        }
    }

    public bool IsTaskCompleted(int index)
    {
        if (index < 0 || index >= completedTasks.Count)
        {
            return false;
        }

        return completedTasks[index];
    }

    public void CompleteTask(int taskIndex, ulong playedID)
    { 
        if (!IsServer || allTasksCompleted)
        {
            return;
        }
        if (taskIndex < 0 || taskIndex >= completedTasks.Count)
        {
            return;
        }
        if (completedTasks[taskIndex])
        {
            return;
        } 

        completedTasks[taskIndex] = true;

        if (NetworkManager.Singleton.ConnectedClients.TryGetValue(playedID, out var client))
        {
            PlayerScore score = client.PlayerObject.GetComponent<PlayerScore>();

            if (score != null)
            {
                score.AddPoint();
            }
        }

        CheckCompletion();
    }

    public void CheckCompletion()
    {
        foreach (bool completed in completedTasks)
        {
            if (!completed)
            {
                return;
            }

            allTasksCompleted = true;
            SelectWinner();
        }
    }

    private void SelectWinner()
    {
        int highestScore = -1;
        List<ulong> winners = new List<ulong>();

        foreach (var client in NetworkManager.Singleton.ConnectedClientsList)
        {
            PlayerScore score = client.PlayerObject.GetComponent<PlayerScore>();

            if (score == null)
            {
                continue;
            }

            int points = score.Points.Value;

            if (points > highestScore)
            {

                highestScore = points;
                winners.Clear();
                winners.Add(client.ClientId);
            }
            else if (points == highestScore)
            {
                winners.Add(client.ClientId);
            }
        }

        if (winners.Count == 0)
        {
            return;
        }

        ulong winnerID = winners[Random.Range(0, winners.Count)];

        SpawnCostume(winnerID);
    }

    private void SpawnCostume(ulong winnerID)
    {
        if (!NetworkManager.Singleton.ConnectedClients.TryGetValue(winnerID, out var client))
        {
            return;
        }
        if (client.PlayerObject == null)
        {
            return;
        }

        Vector3 spawnPosition = client.PlayerObject.transform.position + Vector3.down * 1.5f;

        GameObject costume = Instantiate(assassinClothes, spawnPosition, Quaternion.identity);
        costume.GetComponent<NetworkObject>().Spawn();
    }
}
