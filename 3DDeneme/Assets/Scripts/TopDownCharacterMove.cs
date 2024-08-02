using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(InputManager))]
public class TopDownCharacterMove : MonoBehaviour
{
    private InputManager _input;

    [SerializeField]
    private float MovementSpeed = 5f;
    [SerializeField]
    private float RotationSpeed = 700f;
    [SerializeField]
    private float JumpForce = 10f;

    [SerializeField]
    private Camera Camera;

    [SerializeField]
    private Vector3 CameraOffset;
    [SerializeField]
    private float CameraFollowSpeed = 10f;

    [SerializeField]
    private ParticleSystem hitEffect;
    
    [SerializeField]
    private float AttackRange = 2f; 

    private bool isGrounded = true;

    private void Awake()
    {
        _input = GetComponent<InputManager>();
    }

    private void Update()
    {
        var targetVector = new Vector3(_input.InputVector.x, 0, _input.InputVector.y);
        var movementVector = MoveTowardTarget(targetVector);

        RotateTowardMovementVector(movementVector);

        if (_input.IsJumping && isGrounded)
        {
            ApplyJump();
            _input.FinishJump();
        }

        if (_input.IsAttacking)
        {
            Attack();
            _input.FinishAttack();
        }
    }

    //private void LateUpdate()
    //{
    //    FollowWithCamera();
    //}

    private Vector3 MoveTowardTarget(Vector3 targetVector)
    {
        var speed = MovementSpeed * Time.deltaTime;
        targetVector = Quaternion.Euler(0, Camera.gameObject.transform.rotation.eulerAngles.y, 0) * targetVector;
        var targetPosition = transform.position + targetVector * speed;
        transform.position = targetPosition;
        return targetVector;
    }

    private void RotateTowardMovementVector(Vector3 movementDirection)
    {
        if (movementDirection.magnitude == 0) return;
        var rotation = Quaternion.LookRotation(movementDirection);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, rotation, RotationSpeed * Time.deltaTime);
    }

    //private void FollowWithCamera()
    //{
    //    if (Camera != null)
    //    {
    //        Vector3 targetPosition = transform.position + CameraOffset;
    //        Camera.transform.position = Vector3.Lerp(Camera.transform.position, targetPosition, CameraFollowSpeed * Time.deltaTime);
    //    }
    //}

    private void ApplyJump()
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.AddForce(Vector3.up * JumpForce, ForceMode.Impulse);
        }
    }
    
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground")) 
        {
            isGrounded = true;
            _input.SetGroundedState(true);
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
            _input.SetGroundedState(false);
        }
    }

    private void Attack()
    {
        Ray ray = Camera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            Interactable interactable = hit.collider.GetComponent<Interactable>();
            if (interactable != null && interactable.interactionType == InteractableType.Enemy)
            {
                float distanceToEnemy = Vector3.Distance(transform.position, interactable.transform.position);
                if (distanceToEnemy <= AttackRange)
                {
                    interactable.myActor?.TakeDamage(1); 
                    
                    if (hitEffect != null)
                    {
                        ParticleSystem effect = Instantiate(hitEffect, hit.point, Quaternion.identity);
                        effect.transform.SetParent(null); 
                        effect.Play(); 
                        
                        Destroy(effect.gameObject, effect.main.duration + effect.main.startLifetime.constant);
                    }
                }
            }
        }
    }

}
