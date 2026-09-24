using UnityEngine;
using UnityEngine.InputSystem;


public class TopDownController : MonoBehaviour
{
    public Rigidbody2D body;
    public float walk_speed;

    Vector2 direction;


    public void OnMove(InputAction.CallbackContext context)
    {
        direction = context.ReadValue<Vector2>();
    }

    private void FixedUpdate()
    {
        body.linearVelocity = direction.normalized * walk_speed;
    }
}
