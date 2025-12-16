using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField]
    private Camera mainCamera;
    [SerializeField]
    private PlayerInput playerInput;
    [SerializeField]
    private PlayerMovement playerMovement;

    public float interactionRayLength = 5f;

    public LayerMask groundMask;
    public bool fly = false;

    private void Awake()
    {
        if (mainCamera == null)
        { 
            mainCamera = Camera.main;
        }
        playerInput = GetComponent<PlayerInput>();
        playerMovement = GetComponent<PlayerMovement>();
    }

    private void Start()
    {
        playerInput.OnMouseClick += HandleMouseClick;
        playerInput.OnFly += HandleFly;
    }
    private void Update()
    {
        if (fly)
        {
            playerMovement.Fly(playerInput.MovementInput, playerInput.IsJumping, playerInput.IsRunning);
        }
        else 
        {
            if (playerMovement.IsGrounded && playerInput.IsJumping /* && isWaiting == false*/)
            {
                //isWaiting == true;
                //StopAllCoroutines();
                //StartCoroutine(ResetWaiting());
            }
            playerMovement.HandleGravity(playerInput.IsJumping);
            playerMovement.Walk(playerInput.MovementInput, playerInput.IsRunning);
        }
    }
    private void HandleFly()
    {
        fly = !fly;
    }
    private void HandleMouseClick()
    {

    }
}