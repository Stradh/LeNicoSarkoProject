using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private RaycastHit hit;
    private Vector3 rayOffSetY = new Vector3(0, 2, 0);
    private Vector3 floorRayOffSetY = new Vector3(0, 1f, 0);
    private string floorType = "";
    private float Reach = 1;
    private bool RayHit = false;
    private bool isRunning = false;
    private bool hasHitWall = false;
    private float walkingDelay = 0.25f;
    private float turningDelay = 0.5f;
    private List<AudioClip> usedStepSounds = new List<AudioClip>();
    private List<AudioClip> usedWallSounds = new List<AudioClip>();
    public List<AudioClip> stepSounds = new List<AudioClip>();
    private AudioClip lastPlayedWallSound;
    private AudioClip lastPlayedStepSound;

    public bool randomStepSounds;
    public List<AudioClip> woodStepSounds = new List<AudioClip>();
    public List<AudioClip> stoneStepSounds = new List<AudioClip>();
    public List<AudioClip> dirtStepSounds = new List<AudioClip>();
    public List<AudioClip> grassStepSounds = new List<AudioClip>();
    public List<AudioClip> metalStepSounds = new List<AudioClip>();
    public List<AudioClip> mudStepSounds = new List<AudioClip>();
    public List<AudioClip> wallHitSounds = new List<AudioClip>();
    public List<AudioClip> waterStepSounds = new List<AudioClip>();
    public List<AudioClip> carpetStepSounds = new List<AudioClip>();
    public bool smoothTransition = false;
    public float transitionSpeed = 10f;
    public float transitionRotationSpeed = 500f;

    Vector3 targetGridPos;
    Vector3 prevTargetGridPos;
    Vector3 targetRotation;

    private void Start()
    {
        targetGridPos = transform.position;
    }

    private void FixedUpdate()
    {
        GroundCollsion();
        MovePlayer();

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
        if (Physics.Raycast(transform.position + floorRayOffSetY, Vector3.down, out floorHit, 3) && floorHit.transform.tag.Contains("Floor"))
        {
            Vector3 targetLocation = floorHit.point;
            targetGridPos = new Vector3(targetGridPos.x, targetLocation.y, targetGridPos.z);
        }
    }

    private void CheckForGroundType()
    {
        RaycastHit floorHit;
        if (Physics.Raycast(transform.position + floorRayOffSetY, Vector3.down, out floorHit, 3) && floorHit.transform.tag.Contains("Floor"))
        {
            floorType = floorHit.transform.tag.Substring(5);
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
        MoveDirection(transform.forward, "forward");
    }

    public void MoveBackward()
    {
        MoveDirection(-transform.forward, "backward");
    }

    public void MoveLeft()
    {
        MoveDirection(-transform.right, "left");
    }

    public void MoveRight()
    {
        MoveDirection(transform.right, "right");
    }

    private void MoveDirection(Vector3 direction, string dir)
    {
        if (CheckIfMoving() && !CheckForWall(dir))
        {
            targetGridPos += direction;
            StartCoroutine(PlayRandomStepSound());
        }
        else if (isResting && CheckForWall(dir))
        {
            StartCoroutine(PlayWallHitSound());
        }
    }

    private IEnumerator PlayWallHitSound()
    {
        if (!hasHitWall && wallHitSounds.Count > 1)
        {
            hasHitWall = true;
            int randomWallSound = Random.Range(0, wallHitSounds.Count);

            if (usedWallSounds.Count == wallHitSounds.Count)
                usedWallSounds.Clear();

            while (usedWallSounds.Contains(wallHitSounds[randomWallSound]) || lastPlayedWallSound == wallHitSounds[randomWallSound])
            {
                randomWallSound = Random.Range(0, wallHitSounds.Count);
            }

            usedWallSounds.Add(wallHitSounds[randomWallSound]);
            lastPlayedWallSound = wallHitSounds[randomWallSound];
            yield return new WaitForSeconds(SoundManager.Instance.PlaySound(wallHitSounds[randomWallSound], 0.2f));
            hasHitWall = false;
        }
    }

    private bool CheckForWall(string direction)
    {
        switch (direction)
        {
            case "forward":
                if (Physics.Raycast(transform.position + rayOffSetY, transform.forward, out hit, Reach) && hit.transform.tag == "Walls")
                {
                    return true;
                }
                break;
            case "backward":
                if (Physics.Raycast(transform.position + rayOffSetY, transform.forward * -1, out hit, Reach) && hit.transform.tag == "Walls")
                {
                    return true;
                }
                break;
            case "left":
                if (Physics.Raycast(transform.position + rayOffSetY, transform.right * -1, out hit, Reach) && hit.transform.tag == "Walls")
                {
                    return true;
                }
                break;
            case "right":
                if (Physics.Raycast(transform.position + rayOffSetY, transform.right, out hit, Reach) && hit.transform.tag == "Walls")
                {
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

    private IEnumerator PlayRandomStepSound()
    {
        yield return new WaitForSeconds(walkingDelay);
        CheckForGroundType();
        switch (floorType)
        {
            case "Wood":
                stepSounds = woodStepSounds;
                break;
            case "Dirt":
                stepSounds = dirtStepSounds;
                break;
            case "Grass":
                stepSounds = grassStepSounds;
                break;
            case "Metal":
                stepSounds = metalStepSounds;
                break;
            case "Mud":
                stepSounds = mudStepSounds;
                break;
            case "Water":
                stepSounds = waterStepSounds;
                break;
            case "Carpet":
                stepSounds = carpetStepSounds;
                break;
            case "Stone":
                stepSounds = stoneStepSounds;
                break;
        }

        if (randomStepSounds || stepSounds.Count > 1)
        {
            int randomStepSound = Random.Range(0, stepSounds.Count);

            if (usedStepSounds.Count == stepSounds.Count)
                usedStepSounds.Clear();

            while (usedStepSounds.Contains(stepSounds[randomStepSound]))
            {
                randomStepSound = Random.Range(0, stepSounds.Count);
            }

            usedStepSounds.Add(stepSounds[randomStepSound]);
            lastPlayedStepSound = stepSounds[randomStepSound];
            yield return new WaitForSeconds(SoundManager.Instance.PlaySoundBypassTimeException(stepSounds[randomStepSound], 0.15f));
        }
        else
        {
            yield return new WaitForSeconds(SoundManager.Instance.PlaySoundBypassTimeException(stepSounds[5], 0.15f));
        }
    }

    private bool CheckIfMoving()
    {
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
    private Vector3 rayOffSetY = new Vector3(0, 2, 0);
    private Vector3 floorRayOffSetY = new Vector3(0, 1f, 0);
    private Vector3 targetLocation = new Vector3();
    private float Reach = 1;

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
            CheckGroundCollision();
            input = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));

            if (Mathf.Abs(input.x) > Mathf.Abs(input.y))
            {
                input.y = 0;

                if (input.x > 0)
                {
                    if (!CheckForWall("right"))
                    {
                        rayPosition = new Vector3(myTransform.position.x + 1f, myTransform.position.y, myTransform.position.z);
                    }
                    else
                        return;
                }
                else if (input.x < 0)
                {
                    if (!CheckForWall("left"))
                    {
                        rayPosition = new Vector3(myTransform.position.x - 1f, myTransform.position.y, myTransform.position.z);
                    }
                    else
                        return;
                }
            }
            else
            {
                input.x = 0;

                if (input.y > 0)
                {
                    if (!CheckForWall("forward"))
                    {
                        rayPosition = new Vector3(myTransform.position.x, myTransform.position.y, myTransform.position.z + 1f);
                    }
                    else
                        return;
                }
                else if (input.y < 0)
                {
                    if (!CheckForWall("backward"))
                    {
                        rayPosition = new Vector3(myTransform.position.x, myTransform.position.y, myTransform.position.z - 1f);
                    }
                    else
                        return;
                }
            }

            if (input != Vector2.zero)
            {
                

               if (Physics.Raycast(rayPosition, Vector3.down, out hit, 2f))
                {
                    groundPosition = hit.point;
                    groundPosition += new Vector3(0, myTransform.localScale.y, 0);
                }

                StartCoroutine(Move(myTransform));
            }
        }
    }

    private void CheckGroundCollision()
    {
        RaycastHit floorHit;
        Debug.DrawRay(transform.position + floorRayOffSetY, Vector3.down);
        if (Physics.Raycast(transform.position + floorRayOffSetY, Vector3.down, out floorHit, 4) && floorHit.transform.tag.Contains("Floor"))
        {
            Vector3 targetLocation = floorHit.point;
            transform.position = targetLocation;
            //myTransform.position = new Vector3(myTransform.position.x, targetLocation.y, myTransform.position.z);
        }
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

    public IEnumerator Move(Transform myTransform)
    {
        isMoving = true;
        myTransform.position = new Vector3(myTransform.position.x, targetLocation.y, myTransform.position.z);
        startPosition = myTransform.position;
        t = 0;
        endPosition = new Vector3(startPosition.x + System.Math.Sign(input.x) * gridSize,
        myTransform.position.y, startPosition.z + System.Math.Sign(input.y) * gridSize);
        
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