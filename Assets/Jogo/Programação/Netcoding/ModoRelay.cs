using UnityEngine;
using Unity.Services.Authentication;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using Unity.Services.Core;

public class ModoRelay : MonoBehaviour
{
   async void Start()
    {
        await UnityServices.InitializeAsync();
        await AuthenticationService.Instance.SignInAnonymouslyAsync();
    }

    async void CreatRelay()
    {
        Allocation allocation = await RelayService.Instance.CreateAllocationAsync(3);
        string joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
    }

    async void JoinRelay(string joinCode)
    {
        await RelayService.Instance.JoinAllocationAsync(joinCode);
    }
}
