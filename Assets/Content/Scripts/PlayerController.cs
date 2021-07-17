﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    private RaycastHit hit;
    private Vector3 rayOffSetY = new Vector3(0, 2, 0);
    private Vector3 floorRayOffSetY = new Vector3(0, 1f, 0);
    private float Reach = 1;
    private bool RayHit = false;
    private bool isRunning = false;
    private float walkingDelay = 0.25f;
    private float turningDelay = 0.5f;

    public bool smoothTransition = false;
    public float transitionSpeed = 10f;
    public float transitionRotationSpeed = 500f;

    Vector3 targetGridPos;
    Vector3 prevTargetGridPos;
    Vector3 targetRotation;

    private void Start()
    {
        targetGridPos = Vector3Int.RoundToInt(transform.position);
    }

    private void FixedUpdate()
    {        
        MovePlayer();
        GroundCollsion();

        if (Input.GetKey(KeyCode.LeftShift))
        {
            if (!isRunning)
            {
                isRunning = true;
                walkingDelay = 0.125f;
                turningDelay = 0.25f;
                transitionSpeed *= 2f;
                transitionRotationSpeed *= 2;
            }
        }
        else
        {
            if (isRunning)
            {
                isRunning = false;
                walkingDelay = 0.25f;
                turningDelay = 0.5f;
                transitionSpeed = 5f;
                transitionRotationSpeed = 250;
            }
        }
    }

    private void GroundCollsion()
    {
        RaycastHit floorHit;
        Debug.DrawRay(transform.position + floorRayOffSetY, Vector3.down);
        if (Physics.Raycast(transform.position + floorRayOffSetY, Vector3.down, out floorHit, 1) && floorHit.transform.tag == "Floor")
        {
            //Debug.Log("Flooooor");
            Vector3 targetLocation = floorHit.point;
            //targetLocation += new Vector3(0, transform.localScale.y /0.5f, 0);
            transform.position = targetLocation;
        }
    }

    void MovePlayer()
    {
        prevTargetGridPos = targetGridPos;

        Vector3 targetPosition = targetGridPos;

        if (targetRotation.y > 270f && targetRotation.y < 361f) targetRotation.y = 0f;
        if (targetRotation.y < 0f) targetRotation.y = 270f;

        if (!smoothTransition)
        {
            transform.position = targetPosition;
            transform.rotation = Quaternion.Euler(targetRotation);
        }
        else
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, Time.deltaTime * transitionSpeed);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(targetRotation), Time.deltaTime * transitionRotationSpeed);
        }
    }

    public void RotateLeft() { if (CheckIfTurning()) targetRotation -= Vector3.up * 90f; }
    public void RotateRight() { if (CheckIfTurning()) targetRotation += Vector3.up * 90f; }
    public void MoveForward() { if (CheckIfMoving() && !CheckForWall("forward")) targetGridPos += transform.forward; }
    public void MoveBackward() { if (CheckIfMoving() && !CheckForWall("backward")) targetGridPos -= transform.forward; }
    //public void MoveLeft() { if (CheckIfResting()) targetGridPos -= transform.right; }
    //public void MoveRight() { if (CheckIfResting()) targetGridPos += transform.right; }

    private bool CheckForWall(string direction)
    {
        switch (direction)
        {
            case "forward":
                if (Physics.Raycast(transform.position + rayOffSetY, transform.forward, out hit, Reach) && hit.transform.tag == "Walls")
                {
                    Debug.Log(hit.transform.tag);
                    return true;
                }
                break;
            case "backward":
                if (Physics.Raycast(transform.position + rayOffSetY, transform.forward * -1, out hit, Reach) && hit.transform.tag == "Walls")
                {
                    Debug.Log(hit.transform.tag);
                    return true;
                }
                break;
        }
        return false;
    }

    private IEnumerator NextStep()
    {
        yield return new WaitForSeconds(1);
    }

    public bool isResting = true;
    public bool canWalk = true;

    private IEnumerator WaitForStep()
    {
        yield return new WaitForSeconds(walkingDelay);
        canWalk = true;
        isResting = true;
    }

    private IEnumerator WaitForRotation()
    {
        yield return new WaitForSeconds(turningDelay);
        canWalk = true;
        isResting = true;
    }

    private bool CheckIfMoving()
    {
        if (Vector3.Distance(transform.position, targetGridPos) < 0.5f)
        {
            if (canWalk)
            {
                canWalk = false;
                StartCoroutine(WaitForStep());
            }
        }
        else
        {
            isResting = false;
        }

        return isResting;
    }

    private bool CheckIfTurning()
    {
        if (Vector3.Distance(transform.eulerAngles, targetRotation) < 0.5f)
        {
            if (canWalk)
            {
                canWalk = false;
                StartCoroutine(WaitForRotation());
            }
        }
        else
        {
            isResting = false;
        }
        return isResting;
    }
}