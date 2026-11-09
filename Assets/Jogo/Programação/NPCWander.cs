using System.Collections.Generic;
using UnityEngine;

public class NPCWander : MonoBehaviour
{

    public Rigidbody2D body;
    public float walkSpeed;
    public Animator animator;
    public SpriteRenderer spriteRenderer;
    public List<RuntimeAnimatorController> sprites;

    public float minWalkTime = 1f;
    public float maxWalkTime = 2.5f;

    public float minIdleTime = 5f;
    public float maxIdleTime = 20f;

    private Vector2 direction;
    private float timer;
    private bool walking = false;

    void Start()
    {
        RandomSkin();
        StartIdle();
    }

    void Update()
    {
        timer -= Time.deltaTime;

        if (timer < 0f)
        {
            if (walking)
            {
                StartIdle();
            }
            else
            {
                ChooseDirection();
            }
        }

        animator.SetFloat("Speed", walking ? 1f : 0f);
    }

    private void FixedUpdate()
    {
        if (walking)
        {
            body.linearVelocity = direction * walkSpeed;
        }
        else
        {
            body.linearVelocity = Vector2.zero;
        }
    }

    private void StartIdle()
    {
        walking = false;
        direction = Vector2.zero;

        timer = Random.Range(minIdleTime, maxIdleTime);
    }

    private void ChooseDirection()
    {
        direction = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
        walking = true;

        timer = Random.Range(minWalkTime, maxWalkTime);

        if(spriteRenderer != null && direction.x != 0)
        {
            spriteRenderer.flipX = direction.x < 0;
        }
    }

    private void RandomSkin()
    {
        animator.runtimeAnimatorController = sprites[Random.Range(0, sprites.Count)];
    }
}
