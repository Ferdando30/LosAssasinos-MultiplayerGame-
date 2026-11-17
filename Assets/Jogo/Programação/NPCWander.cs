using System.Collections.Generic;
using Unity.Netcode;
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
    private bool atualFlipX;

    void Start()
    {
        RandomSkinRpc();
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

        if (direction.x != 0)
        {
            bool should_flip_x = direction.x < 0;

            if (should_flip_x != atualFlipX)
            {
                atualFlipX = should_flip_x;
                SetSpriteFlipRpc(should_flip_x);
            }
        }
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

    [Rpc(SendTo.Server)]
    private void SetSpriteFlipRpc(bool flipX)
    {
        ApplySpriteFlipRpc(flipX);
    }

    [Rpc(SendTo.Everyone)]
    private void ApplySpriteFlipRpc(bool flipX)
    {
        spriteRenderer.flipX = flipX;
        atualFlipX = flipX;
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
    }

    [Rpc(SendTo.Everyone)]
    private void RandomSkinRpc()
    {
        animator.runtimeAnimatorController = sprites[Random.Range(0, sprites.Count)];
    }
}
