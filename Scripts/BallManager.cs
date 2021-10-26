using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallManager : MonoBehaviour
{
    public bool devMode;
    public PowerManager power;
    public GameObject ring;
    public Vector3 startPosition;
    public StateManager state;
    public AudioSource hit;
    public AudioSource bounce;

    public float gravity;
    public float terminalVelocity;
    public Vector2 hitbox;
    public Vector2 hitboxOffset;
    public float separationOffset;
    public float floorDetectionDistance;
    public float friction;
    public float xBounceDamp;
    public float yBounceDamp;
    public float yBounceLimit;
    public float cheatSpeed;

    private float xVelocity;
    private float yVelocity;
    private bool xBounce;
    private bool yBounce;
    private float xLastVelocity;
    private float yLastVelocity;
    private int nShots;
    
    private bool grounded;
    private bool canShoot;
    private bool isShooting;

    private List<int> collisionLayers;
    private List<int> collisionLayersSpace;

    private void Start()
    {
        collisionLayers = new List<int>();
        collisionLayers.Add(LayerMask.GetMask("Default"));

        collisionLayersSpace = new List<int>();
        collisionLayersSpace.Add(LayerMask.GetMask("Space"));

        Reset(startPosition);
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.Mouse0) && canShoot &&
            Vector3.Distance(Camera.main.ScreenToWorldPoint(Input.mousePosition) +
                Vector3.forward * 10f, transform.position) < 2f &&
                !state.GetTransitioning())
        {
            power.SetDraw(true);
            isShooting = true;
        }
        else if (Input.GetKeyUp(KeyCode.Mouse0) && isShooting)
        {
            Vector3 forceVector = power.GetDirection() * power.GetPower();
            xVelocity = forceVector.x;
            yVelocity = forceVector.y;
            power.SetDraw(false);
            isShooting = false;
            canShoot = false;
            nShots++;

            hit.Play();
        }
        ring.SetActive(canShoot && !isShooting);

        if (Input.GetKeyDown(KeyCode.R))
        {
            Reset(startPosition);
        }
        if (Input.GetKeyDown(KeyCode.Escape) && !state.GetTransitioning())
        {
            state.SetState(2);
        }
    }

    private void FixedUpdate()
    {   
        if (Input.GetKey(KeyCode.Space) && devMode) // DEV MODE
        {
            DoCheating();
        }
        else
        {
            if (CheckSpace())
            {
                DoSpace();
            }
            else
            {
                DoPhysics();
            }
        }
    }

    private void DoPhysics()
    {
        //Y Slope
        float slopeAngle = RayCastUtilities.RayCastAngle(transform.position, Vector3.down,
            hitbox.x / 2f - separationOffset, collisionLayers);

        //X Movement
        if (Mathf.Abs(slopeAngle) >= 0.01f && grounded)
        {
            Debug.Log(slopeAngle);
            xVelocity += Mathf.Cos(slopeAngle * Mathf.Deg2Rad - Mathf.PI/2f) * gravity * Time.deltaTime;
        }
        else if (Mathf.Abs(xVelocity) > friction * Time.deltaTime && grounded)
        {
            xVelocity += -Mathf.Sign(xVelocity) * friction * Time.deltaTime;
        }
        else if (grounded)
        {
            xVelocity = 0f;
        }

        //X Collision
        xBounce = false;
        xLastVelocity = 0f;
        float distanceRight = RayCastUtilities.RayCast(3, transform.position, Vector2.right,
            new Vector3(hitboxOffset.x, hitboxOffset.y + hitbox.y / 2f - separationOffset, 0f),
            new Vector3(hitboxOffset.x, -hitbox.y / 2f + separationOffset, 0f),
            collisionLayers);

        float distanceLeft = RayCastUtilities.RayCast(3, transform.position, Vector2.left,
            new Vector3(hitboxOffset.x, hitboxOffset.y + hitbox.y / 2f - separationOffset, 0f),
            new Vector3(hitboxOffset.x, hitboxOffset.y - hitbox.y / 2f + separationOffset, 0f),
            collisionLayers);

        if (xVelocity * Time.deltaTime > (distanceRight + hitboxOffset.x - hitbox.x / 2f))
        {
            xLastVelocity = xVelocity;
            xVelocity = (distanceRight + hitboxOffset.x - hitbox.x / 2f) / Time.deltaTime;
            xBounce = true;
        }
        else if (xVelocity * Time.deltaTime < -(distanceLeft + hitboxOffset.x - hitbox.x / 2f))
        {
            xLastVelocity = xVelocity;
            xVelocity = -(distanceLeft + hitboxOffset.x - hitbox.x / 2f) / Time.deltaTime;
            xBounce = true;
        }

        //MOVE X
        transform.position += Vector3.right * xVelocity * Time.deltaTime;
        if (xBounce)
        {
            xVelocity = -xLastVelocity * xBounceDamp;
            bounce.Play();
        }

        //Y Movement
        yVelocity -= gravity * Time.deltaTime;
        if (yVelocity < -terminalVelocity) yVelocity = -terminalVelocity;

        //Y Collision
        yBounce = false;
        yLastVelocity = 0f;
        float distanceDown = RayCastUtilities.RayCast(3, transform.position, Vector3.down,
            new Vector3(hitboxOffset.x + hitbox.x / 2f - separationOffset, hitboxOffset.y, 0f),
            new Vector3(hitboxOffset.x - hitbox.x / 2f + separationOffset, hitboxOffset.y, 0f),
            collisionLayers);
        grounded = (distanceDown - hitbox.y / 2f) < floorDetectionDistance;

        float distanceUp = RayCastUtilities.RayCast(3, transform.position, Vector2.up,
            new Vector3(hitboxOffset.x + hitbox.x / 2f - separationOffset, hitboxOffset.y, 0f),
            new Vector3(hitboxOffset.x - hitbox.x / 2f + separationOffset, hitboxOffset.y, 0f),
            collisionLayers);

        if (yVelocity * Time.deltaTime > (distanceUp + hitboxOffset.y - hitbox.y / 2f))
        {
            yLastVelocity = yVelocity;
            yVelocity = (distanceUp + hitboxOffset.y - hitbox.y / 2f) / Time.deltaTime;
            yBounce = true;
        }
        else if (yVelocity * Time.deltaTime < -(distanceDown + hitboxOffset.y - hitbox.y / 2f))
        {
            yLastVelocity = yVelocity;
            yVelocity = -(distanceDown + hitboxOffset.y - hitbox.y / 2f) / Time.deltaTime;
            yBounce = true;
        }
        
        //MOVE Y
        transform.position += Vector3.up * yVelocity * Time.deltaTime;
        if (yBounce && Mathf.Abs(yLastVelocity) > yBounceLimit)
        {
            yVelocity = -yLastVelocity * yBounceDamp;
            bounce.Play();
        }

        canShoot = Mathf.Abs(xVelocity) < 0.01f && Mathf.Abs(yVelocity) < 0.01f;
    }

    private bool CheckSpace()
    {
        float distanceRight = RayCastUtilities.RayCast(3, transform.position, Vector2.right,
            new Vector3(hitboxOffset.x, hitboxOffset.y + hitbox.y / 2f - separationOffset, 0f),
            new Vector3(hitboxOffset.x, -hitbox.y / 2f + separationOffset, 0f),
            collisionLayersSpace);

        float distanceLeft = RayCastUtilities.RayCast(3, transform.position, Vector2.left,
            new Vector3(hitboxOffset.x, hitboxOffset.y + hitbox.y / 2f - separationOffset, 0f),
            new Vector3(hitboxOffset.x, hitboxOffset.y - hitbox.y / 2f + separationOffset, 0f),
            collisionLayersSpace);

        float distanceDown = RayCastUtilities.RayCast(3, transform.position, Vector3.down,
            new Vector3(hitboxOffset.x + hitbox.x / 2f - separationOffset, hitboxOffset.y, 0f),
            new Vector3(hitboxOffset.x - hitbox.x / 2f + separationOffset, hitboxOffset.y, 0f),
            collisionLayersSpace);

        float distanceUp = RayCastUtilities.RayCast(3, transform.position, Vector2.up,
            new Vector3(hitboxOffset.x + hitbox.x / 2f - separationOffset, hitboxOffset.y, 0f),
            new Vector3(hitboxOffset.x - hitbox.x / 2f + separationOffset, hitboxOffset.y, 0f),
            collisionLayersSpace);

        float max = Mathf.Max(Mathf.Max(distanceRight, distanceLeft), Mathf.Max(distanceDown, distanceUp));
        return max < 0.5f;
    }

    private void DoCheating()
    {
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition) + Vector3.forward * 10f;
        Vector3 direction = Vector3.Normalize(worldPosition - transform.position);

        transform.position += direction * cheatSpeed * Time.deltaTime;
        xVelocity = 0f;
        yVelocity = 0f;
        isShooting = false;
    }

    private void DoSpace()
    {
        //X Collision
        xBounce = false;
        xLastVelocity = 0f;
        float distanceRight = RayCastUtilities.RayCast(3, transform.position, Vector2.right,
            new Vector3(hitboxOffset.x, hitboxOffset.y + hitbox.y / 2f - separationOffset, 0f),
            new Vector3(hitboxOffset.x, -hitbox.y / 2f + separationOffset, 0f),
            collisionLayers);

        float distanceLeft = RayCastUtilities.RayCast(3, transform.position, Vector2.left,
            new Vector3(hitboxOffset.x, hitboxOffset.y + hitbox.y / 2f - separationOffset, 0f),
            new Vector3(hitboxOffset.x, hitboxOffset.y - hitbox.y / 2f + separationOffset, 0f),
            collisionLayers);

        if (xVelocity * Time.deltaTime > (distanceRight + hitboxOffset.x - hitbox.x / 2f))
        {
            xLastVelocity = xVelocity;
            xVelocity = (distanceRight + hitboxOffset.x - hitbox.x / 2f) / Time.deltaTime;
            xBounce = true;
        }
        else if (xVelocity * Time.deltaTime < -(distanceLeft + hitboxOffset.x - hitbox.x / 2f))
        {
            xLastVelocity = xVelocity;
            xVelocity = -(distanceLeft + hitboxOffset.x - hitbox.x / 2f) / Time.deltaTime;
            xBounce = true;
        }

        //MOVE X
        transform.position += Vector3.right * xVelocity * Time.deltaTime;
        if (xBounce)
        {
            xVelocity = -xLastVelocity;
            bounce.Play();
        }

        //Y Collision
        yBounce = false;
        yLastVelocity = 0f;
        float distanceDown = RayCastUtilities.RayCast(3, transform.position, Vector3.down,
            new Vector3(hitboxOffset.x + hitbox.x / 2f - separationOffset, hitboxOffset.y, 0f),
            new Vector3(hitboxOffset.x - hitbox.x / 2f + separationOffset, hitboxOffset.y, 0f),
            collisionLayers);
        grounded = (distanceDown - hitbox.y / 2f) < floorDetectionDistance;

        float distanceUp = RayCastUtilities.RayCast(3, transform.position, Vector2.up,
            new Vector3(hitboxOffset.x + hitbox.x / 2f - separationOffset, hitboxOffset.y, 0f),
            new Vector3(hitboxOffset.x - hitbox.x / 2f + separationOffset, hitboxOffset.y, 0f),
            collisionLayers);

        if (yVelocity * Time.deltaTime > (distanceUp + hitboxOffset.y - hitbox.y / 2f))
        {
            yLastVelocity = yVelocity;
            yVelocity = (distanceUp + hitboxOffset.y - hitbox.y / 2f) / Time.deltaTime;
            yBounce = true;
        }
        else if (yVelocity * Time.deltaTime < -(distanceDown + hitboxOffset.y - hitbox.y / 2f))
        {
            yLastVelocity = yVelocity;
            yVelocity = -(distanceDown + hitboxOffset.y - hitbox.y / 2f) / Time.deltaTime;
            yBounce = true;
        }
        
        //MOVE Y
        transform.position += Vector3.up * yVelocity * Time.deltaTime;
        if (yBounce && Mathf.Abs(yLastVelocity) > yBounceLimit)
        {
            yVelocity = -yLastVelocity;
            bounce.Play();
        }
    }

    public void Reset(Vector3 position)
    {
        if (position.Equals(Vector3.zero))
        {
            transform.position = startPosition;
        }
        else
        {
            transform.position = position;
        }
        xVelocity = 0f;
        yVelocity = 0f;
        isShooting = false;
        nShots = 0;
        state.ResetInGameTimer();
    }

    public int GetShots()
    {
        return nShots;
    }
}
