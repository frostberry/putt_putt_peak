using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MuteButtonManager : MonoBehaviour
{
    public StateManager state;
    public Bounds bounds;

    public Sprite audioOn;
    public Sprite audioOff;

    private SpriteRenderer sr;

    private void Start()
    {
        sr = GetComponent<SpriteRenderer>();
    }

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
                state.ToggleAudioOn();
            }
        }
        sr.sprite = state.GetAudioOn() ? audioOn : audioOff;
    }
}
