using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerManager : MonoBehaviour
{
    public GameObject ball;
    public Sprite[] dots;
    public Sprite[] arrows;
    public Sprite smallDot;

    public float minDistance;
    public float maxDistance;
    public float minPower;
    public float maxPower;
    public float dotSpacing;
    public float dotGravity;

    private Dictionary<int, GameObject> dotsDict;
    private Dictionary<int, GameObject> arrowsDict;
    private Dictionary<int, GameObject> smallDotsDict;
    private bool draw;
    private Vector3 direction;
    private int powerStep;

    private void Start()
    {
        dotsDict = new Dictionary<int, GameObject>();
        arrowsDict = new Dictionary<int, GameObject>();
        smallDotsDict = new Dictionary<int, GameObject>();

        CreateGameObjects();
    }

    private void Update()
    {
        if (dots.Length != arrows.Length)
        {
            Debug.Log("Invalid sprite counts: dots must equal arrows.");
        }
        else
        {
            Vector3 worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition) + Vector3.forward * 10f;
            direction = Vector3.Normalize(ball.transform.position - worldPosition);
            float distance = Vector3.Distance(ball.transform.position, worldPosition);

            if (distance < minDistance) distance = minDistance;
            if (distance > maxDistance) distance = maxDistance;

            //Normalize distance
            float difference = maxDistance - minDistance;
            distance -= minDistance;
            distance /= difference;

            foreach (KeyValuePair<int, GameObject> dot in dotsDict)
            {
                dot.Value.SetActive(false);
            }
            foreach (KeyValuePair<int, GameObject> arrow in arrowsDict)
            {
                arrow.Value.SetActive(false);
            }
            foreach (KeyValuePair<int, GameObject> smallDot in smallDotsDict)
            {
                smallDot.Value.SetActive(false);
            }

            if (draw)
            {
                float step;
                int max = 0;
                Vector3 shot = GetDirection();
                float xVelocity = shot.x;
                float yVelocity = shot.y;
                float xDistance = 0f;
                float yDistance = 0f;
                for (int i = 0; i < dots.Length; i++)
                {
                    xDistance += xVelocity;
                    yDistance += yVelocity;
                    step = (float)i / (float)(dots.Length - 1);
                    dotsDict[i].SetActive(true);
                    //////
                    // dotsDict[i].transform.position = ball.transform.position +
                    //     direction * dotSpacing * (i + 1);
                    //////

                    //////
                    dotsDict[i].transform.position = ball.transform.position +
                        Vector3.right * xDistance * dotSpacing +
                        Vector3.up * yDistance * dotSpacing;

                    yVelocity -= dotGravity / GetPower() * dotSpacing;
                    //////

                    smallDotsDict[i].SetActive(true);
                    smallDotsDict[i].transform.position = ball.transform.position +
                        -direction * dotSpacing * (i + 1);
                    if (distance <= step)
                    {
                        max = i;
                        powerStep = i + 1;
                        break;
                    }
                }
                xDistance += xVelocity;
                yDistance += yVelocity;
                arrowsDict[max].SetActive(true);
                arrowsDict[max].transform.position = ball.transform.position +
                    Vector3.right * xDistance * dotSpacing +
                    Vector3.up * yDistance * dotSpacing;
            }
        }
    }

    private void CreateGameObjects()
    {
        for (int i = 0; i < dots.Length; i++)
        {
            GameObject newDot = new GameObject("Dot" + (i + 1));
            newDot.transform.SetParent(transform);
            SpriteRenderer sr = newDot.AddComponent<SpriteRenderer>();
            sr.sprite = dots[i];
            dotsDict[i] = newDot;
        }

        for (int i = 0; i < arrows.Length; i++)
        {
            GameObject newArrow = new GameObject("Arrow" + (i + 1));
            newArrow.transform.SetParent(transform);
            SpriteRenderer sr = newArrow.AddComponent<SpriteRenderer>();
            sr.sprite = arrows[i];
            arrowsDict[i] = newArrow;
        }

        for (int i = 0; i < dots.Length; i++)
        {
            GameObject newSmallDot = new GameObject("SmallDot" + (i + 1));
            newSmallDot.transform.SetParent(transform);
            SpriteRenderer sr = newSmallDot.AddComponent<SpriteRenderer>();
            sr.sprite = smallDot;
            smallDotsDict[i] = newSmallDot;
        }
    }

    public float GetPower()
    {
        float difference = maxPower - minPower;
        float power = minPower + difference * ((float)(powerStep - 1) / (float)(dots.Length - 1));
        return power;
    }

    public Vector3 GetDirection()
    {
        return direction;
    }

    public void SetDraw(bool draw)
    {
        this.draw = draw;
    }
}
