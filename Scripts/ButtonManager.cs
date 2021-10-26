using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonManager : MonoBehaviour
{
    public StateManager state;
    public int triggeredState;
    public Bounds bounds;

    public bool isTimerToggle;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition) + Vector3.forward * 10f;
            if (pos.x > transform.position.x - bounds.extents.x &&
                pos.x < transform.position.x + bounds.extents.x &&
                pos.y > transform.position.y - bounds.extents.y &&
                pos.y < transform.position.y + bounds.extents.y)
            {
                Trigger();
            }
        }
    }

    private void Trigger()
    {
        if (isTimerToggle)
        {
            state.ToggleTimer();
        }
        else if (!state.GetTransitioning())
        {
            state.SetState(triggeredState);
        }
    }
}
