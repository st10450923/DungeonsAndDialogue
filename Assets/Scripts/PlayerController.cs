using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float interactRange = 1.5f;
    [SerializeField] private LayerMask interactableLayer;
    [SerializeField] private Animator animator;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private GameObject interactibleCenter;

    private Rigidbody2D rb;
    private Vector2 moveInput;
    private bool isDialogueOpen = false;

    public static PlayerController Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    private void OnEnable() => SceneManager.sceneLoaded += OnSceneLoaded;
    private void OnDisable() => SceneManager.sceneLoaded -= OnSceneLoaded;

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        GameObject spawnPoint = GameObject.FindGameObjectWithTag("Spawn");
        if (spawnPoint != null)
            transform.SetPositionAndRotation(
                spawnPoint.transform.position,
                spawnPoint.transform.rotation);

        rb.linearVelocity = Vector2.zero;
        moveInput = Vector2.zero;
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
        if (Keyboard.current.eKey.wasPressedThisFrame && !isDialogueOpen)
            TryInteract();
        UpdateAnimation();
    }

    private void UpdateAnimation()
    {
        if (animator == null) return;
        bool isMoving = moveInput != Vector2.zero;
        animator.SetBool("Walking", isMoving);
        if (isMoving)
        {
            animator.SetFloat("DirectionX", moveInput.x);
            animator.SetFloat("DirectionY", moveInput.y);
        }
    }

    private void TryInteract()
    {
        Vector2 center = interactibleCenter != null
            ? (Vector2)interactibleCenter.transform.position
            : (Vector2)transform.position;

        ContactFilter2D filter = new ContactFilter2D();
        filter.SetLayerMask(interactableLayer);
        filter.useTriggers = true;

        Collider2D[] results = new Collider2D[5];
        int count = Physics2D.OverlapCircle(center, interactRange, filter, results);

        for (int i = 0; i < count; i++)
        {
            if (results[i].TryGetComponent<IInteractable>(out var interactable))
            {
                interactable.Interact();
                return;
            }
        }
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