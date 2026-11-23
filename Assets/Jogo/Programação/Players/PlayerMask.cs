using UnityEngine;
using Unity.Netcode;
using System.Collections;

public class PlayerMask : NetworkBehaviour
{
    public NetworkVariable<bool> HasMask = new NetworkVariable<bool>(
        false,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server
    );

    

    public void GetMask()
    {
        if (!IsServer) return;

        HasMask.Value = true;
        StartCoroutine(MaskTimer());
    }

    IEnumerator MaskTimer()
    {
        yield return new WaitForSeconds(10f);

        HasMask.Value = false;
    }
}
