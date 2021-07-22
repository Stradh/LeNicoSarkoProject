using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(PlayerController))]
public class PlayerInput : MonoBehaviour
{

    [SerializeField] private KeyCode forward = KeyCode.W;
    public KeyCode back = KeyCode.S;
    public KeyCode left = KeyCode.A;
    public KeyCode right = KeyCode.D;
    public KeyCode turnLeft = KeyCode.Q;
    public KeyCode turnRight = KeyCode.E;

    PlayerController controller;

    private void Awake()
    {
        controller = GetComponent<PlayerController>();
    }

    private void Update()
    {
        if (Input.GetKey(forward)) controller.MoveForward();
        else if (Input.GetKey(back)) controller.MoveBackward();
        else if (Input.GetKey(left)) controller.RotateLeft();
        else if (Input.GetKey(right)) controller.RotateRight();
        else if (Input.GetKey(turnLeft)) controller.MoveLeft();
        else if (Input.GetKey(turnRight)) controller.MoveRight();

    }
}
