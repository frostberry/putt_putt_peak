using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TipManager : MonoBehaviour
{
    public Transform player;
    public Bounds bounds;

    private void Update()
    {
        CheckInbounds();
    }

    private void Enable()
    {
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(true);
        }
    }

    private void Disable()
    {
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(false);
        }
    }

    private void CheckInbounds()
    {
        if (player.position.x > bounds.center.x - bounds.extents.x &&
            player.position.x < bounds.center.x + bounds.extents.x &&
            player.position.y > bounds.center.y - bounds.extents.y &&
            player.position.y < bounds.center.y + bounds.extents.y)
        {
            Enable();
        }
        else
        {
            Disable();
        }
    }
}
