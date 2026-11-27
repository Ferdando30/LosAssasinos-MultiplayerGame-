using Unity.Netcode;
using UnityEngine;

public class TaskReporter : NetworkBehaviour
{
    public void RequestTaskCompletion(int taskIndex)
    {
        if (!IsOwner)
        {
            return;
        }

        CompleteTaskRpc(taskIndex);
    }

    [Rpc(SendTo.Server)]
    private void CompleteTaskRpc(int taskIndex)
    {
        TaskManager manager = FindFirstObjectByType<TaskManager>();

        if (manager != null)
        {
            manager.CompleteTask(taskIndex, OwnerClientId);
        }
    }
}