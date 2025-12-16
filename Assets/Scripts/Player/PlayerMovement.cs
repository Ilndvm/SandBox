using System;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField]
    private CharacterController controller;
    [SerializeField]
    private float playerSpeed = 5, playerRunSpeed = 8;
    [SerializeField]
    private float jumpHeight = 1;
    [SerializeField]
    private float gravity = -9.81f;
    [SerializeField]
    private float flySpeed = 3;

    private Vector3 playerVelocity;

    [SerializeField]
    private LayerMask groundMask;
    [SerializeField]
    private float rayDistance = 1;
    [field: SerializeField]
    public bool IsGrounded { get; private set; }

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
    }

    private Vector3 GetMovementDirection(Vector3 movementInput)
    { 
        return transform.right * movementInput.normalized.x + transform.forward * movementInput.normalized.z;
    }

    public void Fly(Vector3 movementInput, bool ascendInput, bool descentInput)
    { 
        Vector3 movementDirection = GetMovementDirection(movementInput);
        
        if (ascendInput)
        {
            movementDirection += Vector3.up * flySpeed;
        }
        else if (descentInput)
        {
            movementDirection -= Vector3.up * flySpeed;
        }
        controller.Move(movementDirection * playerSpeed * Time.deltaTime);
    }
    public void Walk(Vector3 movementInput, bool isRunning)
    {
        Vector3 movementDirection = GetMovementDirection(movementInput);
        float speed = isRunning ? playerRunSpeed : playerSpeed;
        controller.Move(movementDirection * speed * Time.deltaTime);
    }

    public void HandleGravity(bool isJumping)
    {
        if (controller.isGrounded && playerVelocity.y < 0) 
        {
            playerVelocity.y = 0;
        }
        if (isJumping && IsGrounded)
        {
            AddJumpForce();
        }
        ApplyGravityForce();
        controller.Move(playerVelocity * Time.deltaTime);
    }

    private void AddJumpForce()
    {
        playerVelocity.y = jumpHeight;
    }

    private void ApplyGravityForce()
    {
        playerVelocity.y += gravity * Time.deltaTime;
        playerVelocity.y = Mathf.Clamp(playerVelocity.y, gravity, 10);
    }

    private void Update()
    {
        IsGrounded = Physics.Raycast(transform.position, Vector3.down, rayDistance, groundMask);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawRay(transform.position, Vector3.down * rayDistance);
    }
}