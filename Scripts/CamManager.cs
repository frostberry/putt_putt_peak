using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamManager : MonoBehaviour
{
    public Transform target;
    public Bounds levelBounds;
    public float speed;
    public float shiftAmount;
    public StateManager state;

    private bool followTarget;
    private Vector2 camDimensions;

    private Camera cam;
    private int xMovement;
    private int yMovement;

    private void Start()
    {
        cam = GetComponent<Camera>();

        followTarget = false;
        CalculateCamDimensions();

        xMovement = 0;
        yMovement = 0;
    }

    private void Update()
    {
        if (state.GetState() == GameState.InGame)
        {
            xMovement = 0;
            yMovement = 0;
            if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D)) xMovement++;
            if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A)) xMovement--;
            if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W)) yMovement++;
            if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S)) yMovement--;
        }
    }

    private void FixedUpdate()
    {
        CalculateCamDimensions();
        if (followTarget)
        {
            Vector3 newTarget = CalculateTarget(target.position +
                new Vector3(xMovement * shiftAmount, yMovement * shiftAmount * 9f/16f, 0f));
            Vector3 direction = Vector3.Normalize(newTarget + Vector3.back * 10f - transform.position);
            float distance = Vector3.Distance(newTarget + Vector3.back * 10f, transform.position);
            
            transform.position += direction * speed * distance * Time.deltaTime; 
        }
    }

    private void CalculateCamDimensions()
    {
        float x = cam.orthographicSize * 2f;
        float y = x * Screen.width / Screen.height;
        camDimensions = new Vector2(x, y);
    }

    private Vector3 CalculateTarget(Vector3 target)
    {
        float newTargetX = target.x;
        float newTargetY = target.y;
        if (true)
        {
            if (levelBounds.extents.Equals(Vector3.zero))
            {
                newTargetX = levelBounds.center.x;
                newTargetY = levelBounds.center.y;
            }
            else
            {
                Bounds currentBounds = levelBounds;
                if (newTargetX + camDimensions.x / 2f > currentBounds.center.x + currentBounds.extents.x)
                {
                    newTargetX -= (newTargetX + camDimensions.x / 2f) -
                        (currentBounds.center.x + currentBounds.extents.x);
                }
                if (newTargetX - camDimensions.x / 2f < currentBounds.center.x - currentBounds.extents.x)
                {
                    newTargetX += (currentBounds.center.x - currentBounds.extents.x) -
                        (newTargetX - camDimensions.x / 2f);
                }

                if (newTargetY + camDimensions.y / 2f > currentBounds.center.y + currentBounds.extents.y)
                {
                    newTargetY -= (newTargetY + camDimensions.y / 2f) -
                        (currentBounds.center.y + currentBounds.extents.y);
                }
                if (newTargetY - camDimensions.y / 2f < currentBounds.center.y - currentBounds.extents.y)
                {
                    newTargetY += (currentBounds.center.y - currentBounds.extents.y) - 
                        (newTargetY - camDimensions.y / 2f);
                }
            }
        }

        Vector3 newTarget = new Vector3(newTargetX, newTargetY, target.z);
        return newTarget;
    }

    public void SetPosition()
    {
        Vector3 position = CalculateTarget(target.position);
        transform.position = position + Vector3.back * 10f;
    }

    public void StartFollowing()
    {
        followTarget = true;
    }

    public void StopFollowing()
    {
        followTarget = false;
        transform.position = Vector3.back * 10f;
    }
}
