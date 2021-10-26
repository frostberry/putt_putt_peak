using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrystalManager : MonoBehaviour
{
    public StateManager state;
    public Transform player;
    public Bounds bounds;

    private SpriteRenderer sr;

    private void Start()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        CheckInbounds();
    }

    private void CheckInbounds()
    {
        if (player.position.x > transform.position.x - bounds.extents.x &&
            player.position.x < transform.position.x + bounds.extents.x &&
            player.position.y > transform.position.y - bounds.extents.y &&
            player.position.y < transform.position.y + bounds.extents.y &&
            state.GetState() != GameState.Win && !state.GetTransitioning())
        {
            state.SetState(3);
            sr.enabled = false;
        }
        else
        {
            sr.enabled = true;
        }
    }
}
