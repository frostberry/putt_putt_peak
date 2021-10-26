using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleTimerManager : MonoBehaviour
{
    public StateManager state;
    public Sprite yes;
    public Sprite no;

    private SpriteRenderer sr;

    private void Start()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        sr.sprite = state.GetIsInGameTimer() ? yes : no;
    }
}
