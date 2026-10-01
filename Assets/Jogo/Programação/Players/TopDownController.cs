using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Netcode;


public class TopDownController : NetworkBehaviour
{
    public Rigidbody2D body;
    public float walk_speed;
    public Animator animator;
    public SpriteRenderer spriteRenderer;
    

    Vector2 direction;


    public void OnMove(InputAction.CallbackContext context)
    {
        direction = context.ReadValue<Vector2>();
        if (direction != Vector2.zero)
        {
            animator.SetFloat("Speed", 1f);
        }
        else
        {
            animator.SetFloat("Speed", 0f);
        }
    }

    private void FixedUpdate()
    {
        body.linearVelocity = direction.normalized * walk_speed;

        SpriteFlipServerRpc();
    }

    [Rpc(SendTo.Server)]
    private void SpriteFlipServerRpc()
    {
        SpriteFlipClientRpc();
    }

    [Rpc(SendTo.Everyone)]
    private void SpriteFlipClientRpc()
    {
        if (direction.x < 0)
        {
            spriteRenderer.flipX = true;
        }
        else if (direction.x > 0)
        {
            spriteRenderer.flipX = false;
        }
    }
}
