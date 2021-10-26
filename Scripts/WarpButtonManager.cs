using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarpButtonManager : MonoBehaviour
{
    public StateManager state;
    public Vector3 warpPosition;
    public Bounds bounds;
    public BallManager ball;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0) && !state.GetTransitioning())
        {
            Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition) + Vector3.forward * 10f;
            if (pos.x > transform.position.x - bounds.extents.x &&
                pos.x < transform.position.x + bounds.extents.x &&
                pos.y > transform.position.y - bounds.extents.y &&
                pos.y < transform.position.y + bounds.extents.y)
            {
                state.SetState(1);
                ball.Reset(warpPosition);
            }
        }
    }
}
