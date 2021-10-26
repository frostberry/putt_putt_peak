using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundManager : MonoBehaviour
{
    public Transform cam;
    public Sprite[] bgs;
    public float[] yCutoffs;

    private SpriteRenderer sr;

    private void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        
    }

    private void Update()
    {
        sr.sprite = bgs[0];
        for (int i = yCutoffs.Length - 1; i > -1; i--)
        {
            if (cam.position.y > yCutoffs[i])
            {
                sr.sprite = bgs[i];
                break;
            }
        }
    }
}
