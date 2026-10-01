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
    private bool AtualFlipX;


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

        if (IsOwner && direction.x != 0)
        {
            bool shouldFlipX = direction.x < 0;

            
            if (shouldFlipX != AtualFlipX)
            {
                AtualFlipX = shouldFlipX;
                SetSpriteFlipRpc(shouldFlipX);
            }
        }
    }

    private void FixedUpdate()
    {
        body.linearVelocity = direction.normalized * walk_speed;

        
    }

    [Rpc(SendTo.Server)]
    private void SetSpriteFlipRpc(bool flipX)
    {
        ApplySpriteFlipRpc(flipX);
    }

    [Rpc(SendTo.Everyone)]
    private void ApplySpriteFlipRpc(bool flipX)
    {
        spriteRenderer.flipX = flipX;
        AtualFlipX = flipX;
    }
}
