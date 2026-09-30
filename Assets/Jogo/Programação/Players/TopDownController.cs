using UnityEngine;
using UnityEngine.InputSystem;


public class TopDownController : MonoBehaviour
{
    public Rigidbody2D body;
    public float walk_speed;
    public Animator animator;
    public SpriteRenderer spriteRenderer;

    Vector2 direction;


    public void OnMove(InputAction.CallbackContext context)
    {
        direction = context.ReadValue<Vector2>();
    }

    private void FixedUpdate()
    {
        body.linearVelocity = direction.normalized * walk_speed;

        bool isMoving = direction.sqrMagnitude > 0.001f;

        animator.speed = isMoving ? 1f : 0f;

        if (!isMoving)
        {
            animator.Play(0, 0, 0f);
        }

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
