using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Animator))]
public class PlayerMovement : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    public float moveSpeed = 0.02f;

    private bool facingRight = true;
    private Vector3 targetPosition;
    private Animator animator;

    public LayerMask obstacleLayer;
    public LayerMask enemyLayer;

    public AudioClip attack01Sound;
    public AudioClip attack02Sound;
    public AudioClip walkSound;
    public AudioClip deathSound;

    private AudioSource audioSource;

    private void Start()
    {
        targetPosition = transform.position;

        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            if (spriteRenderer == null) { Debug.LogError("SpriteRenderer is missing from the player object."); }
        }

        animator = GetComponent<Animator>();
        if (animator == null) { Debug.LogError("Animator component is missing from the player object."); }

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            Debug.LogWarning("AudioSource is missing on the player object. Adding one dynamically.");
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        audioSource.playOnAwake = false;
    }

    private void Update()
    {

        if (animator == null || animator.GetBool("isAttacking01") || animator.GetBool("isAttacking02") || animator.GetBool("isDead")) { return; }

        HandleMovement();
        SmoothMove();
    }

    void HandleMovement()
    {
        if (Vector3.Distance(transform.position, targetPosition) > 0.001f) { return; }

        Vector3 moveDelta = Vector3.zero;

        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            moveDelta = Vector3.right;
            FaceDirection(Vector3.right);
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            moveDelta = Vector3.left;
            FaceDirection(Vector3.left);
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            moveDelta = Vector3.up;
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            moveDelta = Vector3.down;
        }

        if (moveDelta != Vector3.zero)
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, moveDelta, 1f, enemyLayer);
            if (hit.collider != null)
            {
                OrcBehavior orc = hit.collider.GetComponent<OrcBehavior>();
                if (orc != null)
                {
                    Debug.Log("Attacking an orc.");
                    StartCoroutine(AttackOrc(orc));

                    PlayerActionTracker tracker = FindObjectOfType<PlayerActionTracker>();
                    if (tracker != null) { tracker.RegisterAction(); }
                    else { Debug.LogWarning("PlayerActionTracker not found in the scene."); }

                    return;
                }

                CrateBehavior crate = hit.collider.GetComponent<CrateBehavior>();
                if (crate != null)
                {
                    Debug.Log("Attacking a crate.");
                    StartCoroutine(AttackCrate(crate, moveDelta));
                    
                    PlayerActionTracker tracker = FindObjectOfType<PlayerActionTracker>();
                    if (tracker != null) { tracker.RegisterAction(); }
                    else { Debug.LogWarning("PlayerActionTracker not found in the scene."); }

                    return;
                }

                ChestBehavior chest = hit.collider.GetComponent<ChestBehavior>();
                if (chest != null)
                {
                    Debug.Log("Interacting with a chest.");
                    chest.TriggerChest();

                    PlayerActionTracker tracker = FindObjectOfType<PlayerActionTracker>();
                    if (tracker != null) { tracker.RegisterAction(); }
                    else { Debug.LogWarning("PlayerActionTracker not found in the scene."); }

                    return;
                }
            }

            if (CanMove(moveDelta))
            {
                Debug.Log($"Moving player to {transform.position + moveDelta}");
                targetPosition = transform.position + moveDelta;
                animator.SetBool("isMoving", true);

                PlayerActionTracker tracker = FindObjectOfType<PlayerActionTracker>();
                if (tracker != null) { tracker.RegisterAction(); }
                else { Debug.LogWarning("PlayerActionTracker not found in the scene."); }

                PlaySound(walkSound);
            }
            else
            {
                Debug.Log("Movement blocked by an obstacle.");
                animator.SetBool("isMoving", false);
            }
        }
    }

    void SmoothMove()
    {
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed);

        if (Vector3.Distance(transform.position, targetPosition) < 0.01f)
        {
            Debug.Log("Player reached the target position.");
            transform.position = targetPosition;
            animator.SetBool("isMoving", false);
        }
    }

    void FaceDirection(Vector3 direction)
    {
        if (direction == Vector3.right && !facingRight)
        {
            facingRight = true;
            FlipSprite();
        }
        else if (direction == Vector3.left && facingRight)
        {
            facingRight = false;
            FlipSprite();
        }
    }

    void FlipSprite()
    {
        spriteRenderer.flipX = !spriteRenderer.flipX;
    }

    bool CanMove(Vector3 direction)
    {
        Vector2 currentPosition = transform.position;
        Vector2 targetPosition = currentPosition + (Vector2)direction;

        RaycastHit2D hit = Physics2D.BoxCast(currentPosition, GetComponent<BoxCollider2D>().size, 0f, direction, 1f, obstacleLayer);

        return hit.collider == null;
    }

    IEnumerator AttackOrc(OrcBehavior orc)
    {
        animator.SetBool("isAttacking01", true);
        PlaySound(attack01Sound);
        yield return new WaitForSeconds(0.5f);

        orc.TriggerDeath();
        yield return new WaitForSeconds(orc.deathDelay);

        animator.SetBool("isAttacking01", false);
    }

    IEnumerator AttackCrate(CrateBehavior crate, Vector3 direction)
    {
        animator.SetBool("isAttacking02", true);
        PlaySound(attack02Sound);
        yield return new WaitForSeconds(0.5f);

        crate.Slide(direction);

        animator.SetBool("isAttacking02", false);
    }

    public void TriggerDeath()
    {
        animator.SetBool("isDead", true);
        PlaySound(deathSound);
    }

    private void PlaySound(AudioClip clip)
    {
        if (clip != null && audioSource != null)
        {
            audioSource.clip = clip;
            audioSource.Play();
        }
    }
}
