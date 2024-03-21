using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class characterMovement : MonoBehaviour
{
    private CharacterController characterController;
    [SerializeField] float movementSpeed;
    [SerializeField] float rotateSpeed;

    private InputAction move;

    private float rotateVelocity = 0; //used by smoothdampangle()

    void OnEnable()
    {
        move = subwayManager.instance.playerControls.Player.Move;
        move.Enable();
        
    }

    void OnDisable()
    {
        move.Disable();
    }

    void Start()
    {
        characterController = GetComponent<CharacterController>();
    }

    void Update()
    {
        movePlayer();
        
    }

    private void movePlayer()
    {
        if (move.ReadValue<Vector2>() == Vector2.zero) return;

        Vector3 direction = new Vector3 (move.ReadValue<Vector2>().x, -2f, move.ReadValue<Vector2>().y); // y is for gravity
        direction = Quaternion.Euler(0f, Camera.main.transform.eulerAngles.y, 0f) * direction;

        characterController.Move(direction*movementSpeed*Time.deltaTime);


        float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
        float smoothedAngle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref rotateVelocity, rotateSpeed);
        transform.rotation = Quaternion.Euler(0, smoothedAngle, 0);
    }
}
