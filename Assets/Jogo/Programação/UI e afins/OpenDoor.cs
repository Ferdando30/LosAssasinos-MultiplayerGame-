using Unity.Netcode;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class OpenDoor : MonoBehaviour
{
    private Transform playerLocal;
    public Collider2D DoorCollider;

    public Sprite OpenDoorSprite;
    public Sprite ClosedDoorSprite;

    

    private void EncontrarPlayerLocal()
    {
        if (playerLocal != null)
            return;

        foreach (NetworkObject player in FindObjectsByType<NetworkObject>(FindObjectsSortMode.None))
        {
            if (player.IsOwner && player.CompareTag("Player"))
            {
                playerLocal = player.transform; 
                break;
            }
        }
    }

    private void Zonas()
    {
        EncontrarPlayerLocal();
        if (playerLocal == null)
            return;

        bool InDoor = DoorCollider.bounds.Contains(playerLocal.position);

        SpriteRenderer myRenderer = GetComponent<SpriteRenderer>();
        
        if (InDoor)
        {
            
            myRenderer.sprite = OpenDoorSprite;
        }

        else if (!InDoor)
        {
            myRenderer.sprite = ClosedDoorSprite;
        }
    }

    // Update is called once per frame
    void Update()
    {
        Zonas();
    }
}
