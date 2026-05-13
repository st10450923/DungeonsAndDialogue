using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float interactRange = 1.5f;
    [SerializeField] private LayerMask interactableLayer;
    [SerializeField] private AnimatorController animatorController;
    [SerializeField] private Animator animator;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField]private GameObject interactibleCenter;

    private Rigidbody2D rb;
    private Vector2 moveInput;
    private bool isDialogueOpen = false;
    public static PlayerController Instance { get; private set; }

    private void Awake()
    {
        animator = GetComponent<Animator>();
        animator.runtimeAnimatorController = animatorController;
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        rb = GetComponent<Rigidbody2D>();
    }
    public void OnMove(InputValue value)
    {
        if (!isDialogueOpen)
            moveInput = value.Get<Vector2>();
    }

    private void FixedUpdate()
    {
        rb.linearVelocity = moveInput * moveSpeed;
    }

    private void Update()
    {
        if (Keyboard.current.eKey.wasPressedThisFrame)
            TryInteract();
        UpdateAnimation();
    }
    private void UpdateAnimation()
    {
        if (animatorController == null) return;
        animator.SetBool("Walking", moveInput != Vector2.zero);
        if (moveInput.x >= 0)
            spriteRenderer.flipX = false;
        else
            spriteRenderer.flipX = true;
    }
    private void TryInteract()
    {
        Vector2 center = interactibleCenter != null ? (Vector2)interactibleCenter.transform.position : (Vector2)transform.position;
        Collider2D hit = Physics2D.OverlapCircle(center, interactRange, interactableLayer);

        if (hit != null && hit.TryGetComponent<IInteractable>(out var interactable))
            interactable.Interact();
    }


    public void SetDialogueOpen(bool open)
    {
        isDialogueOpen = open;
        if (open) rb.linearVelocity = Vector2.zero;
        moveInput = Vector2.zero;
    }

    private void OnDrawGizmosSelected()
    {
        Vector2 center = interactibleCenter != null
            ? (Vector2)interactibleCenter.transform.position
            : (Vector2)transform.position;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(center, interactRange);
    }
}