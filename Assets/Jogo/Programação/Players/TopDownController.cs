using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Netcode;


public class TopDownController : NetworkBehaviour
{
    public Rigidbody2D body;
    public float walk_speed;
    public Animator animator;
    public SpriteRenderer spriteRenderer;
    

    Vector2 keyboard_direction;
    private bool AtualFlipX;


    public void OnMove(InputAction.CallbackContext context)
    {
        if (!IsOwner)
        {
            return;
        }

        keyboard_direction = context.ReadValue<Vector2>();
    }

    private void FixedUpdate()
    {
        if (!IsOwner)
        {
            return;
        }

        Vector2 joystick_direction = Vector2.zero;

        if (Virtualjoystick.Instance != null )
        {
            joystick_direction = Virtualjoystick.Instance.Direction;
        }

        Vector2 movement_direction =
        joystick_direction.sqrMagnitude > 0.01f ? joystick_direction : keyboard_direction;

        body.linearVelocity = movement_direction.normalized * walk_speed;

        animator.SetFloat("Speed", movement_direction.sqrMagnitude > 0.01f ? 1f : 0f);

        if (movement_direction.x != 0)
        {
            bool should_flip_x = movement_direction.x < 0;

            if (should_flip_x != AtualFlipX)
            {
                AtualFlipX = should_flip_x;
                SetSpriteFlipRpc(should_flip_x);
            }
        }
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
