using System.Collections;
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


    public AudioClip stepSound;
    public AudioClip wallHitSound;
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
        if (Physics.Raycast(transform.position + floorRayOffSetY, Vector3.down, out floorHit, 3) && floorHit.transform.tag == "Floor")
        {
            Vector3 targetLocation = floorHit.point;
            transform.position = targetLocation;
            targetGridPos = new Vector3(targetGridPos.x, targetLocation.y, targetGridPos.z);
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
    public void MoveForward()
    {
        if (CheckIfMoving() && !CheckForWall("forward"))
        {
            targetGridPos += transform.forward;
            PlayRandomStepSound();
        }
        else if (CheckForWall("forward"))
        {
            PlayWallHitSound();
        }
    }

    public void MoveBackward()
    {
        if (CheckIfMoving() && !CheckForWall("backward"))
        {
            targetGridPos -= transform.forward;
            PlayRandomStepSound();
        }
        else if (CheckForWall("backward"))
        {
            PlayWallHitSound();
        }
    }

    public void MoveLeft()
    {
        if (CheckIfMoving() && !CheckForWall("left"))
        {
            targetGridPos -= transform.right;
            PlayRandomStepSound();
        }
        else if (CheckForWall("left"))
        {
            PlayWallHitSound();
        }
    }

    public void MoveRight()
    {
        if (CheckIfMoving() && !CheckForWall("right"))
        {
            targetGridPos += transform.right;
            PlayRandomStepSound();
        }
        else if (CheckForWall("right"))
        {
            PlayWallHitSound();
        }
    }

    private void PlayWallHitSound()
    {
        float randPitch = Random.Range(0.8f, 1);
        SoundManager.Instance.PlaySoundWithPitch(wallHitSound, randPitch);
    }

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
            case "left":
                if (Physics.Raycast(transform.position + rayOffSetY, transform.right * -1, out hit, Reach) && hit.transform.tag == "Walls")
                {
                    Debug.Log(hit.transform.tag);
                    return true;
                }
                break;
            case "right":
                if (Physics.Raycast(transform.position + rayOffSetY, transform.right, out hit, Reach) && hit.transform.tag == "Walls")
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

    private void PlayRandomStepSound()
    {
        SoundManager.Instance.PlaySoundBypassTimeException(stepSound);
    }

    private bool CheckIfMoving()
    {
        Debug.Log(transform.position);
        Debug.Log(targetGridPos);

        if (Vector3.Distance(new Vector3(transform.position.x, transform.position.y, transform.position.z), new Vector3(targetGridPos.x, targetGridPos.y, targetGridPos.z)) < 0.5f)
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

    #region
    /*public float moveSpeed = 3f;
    public float gridSize = 1f;

    private Vector2 input;
    private bool isMoving = false;
    private Vector3 startPosition;
    private Vector3 endPosition;
    private float t;
    private float factor;

    public Vector3 groundPosition;

    Vector3 rayPosition;
    RaycastHit hit;

    Transform myTransform;

    void Start()
    {
        myTransform = transform;
    }

    public void Update()
    {
        if (!isMoving)
        {
            input = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));

            if (Mathf.Abs(input.x) > Mathf.Abs(input.y))
            {
                input.y = 0;

                if (input.x > 0)
                {
                    rayPosition = new Vector3(myTransform.position.x + 1f, myTransform.position.y, myTransform.position.z);
                }
                else if (input.x < 0)
                {
                    rayPosition = new Vector3(myTransform.position.x - 1f, myTransform.position.y, myTransform.position.z);
                }
            }
            else
            {
                input.x = 0;

                if (input.y > 0)
                {
                    rayPosition = new Vector3(myTransform.position.x, myTransform.position.y, myTransform.position.z + 1f);
                }
                else if (input.y < 0)
                {
                    rayPosition = new Vector3(myTransform.position.x, myTransform.position.y, myTransform.position.z - 1f);
                }
            }

            if (input != Vector2.zero)
            {
                if (Physics.Raycast(rayPosition, Vector3.down, out hit, 2f))
                {
                    groundPosition = hit.point;

                    groundPosition += new Vector3(0, myTransform.localScale.y, 0);

                }

                StartCoroutine(move(myTransform));
            }
        }
    }

    public IEnumerator move(Transform myTransform)
    {
        isMoving = true;
        startPosition = myTransform.position;
        t = 0;

        endPosition = new Vector3(startPosition.x + System.Math.Sign(input.x) * gridSize,
        groundPosition.y, startPosition.z + System.Math.Sign(input.y) * gridSize);

        factor = 1f;

        while (t < 1f)
        {
            t += Time.deltaTime * (moveSpeed / gridSize) * factor;
            myTransform.position = Vector3.Lerp(startPosition, endPosition, t);
            yield return null;
        }

        isMoving = false;
        yield return 0;
    }*/
    #endregion
}