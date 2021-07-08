using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(PlayerController))]
public class PlayerInput : MonoBehaviour {

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

    private void Update()   {

        if (Input.GetKey(forward)) controller.MoveForward();
        if (Input.GetKey(back)) controller.MoveBackward();
        if (Input.GetKey(left)) controller.RotateLeft();//controller.MoveLeft();
        if (Input.GetKey(right)) controller.RotateRight();//controller.MoveRight();
        /*if (Input.GetKey(turnLeft)) controller.RotateLeft();
        if (Input.GetKey(turnRight)) controller.RotateRight();*/

    }
}
