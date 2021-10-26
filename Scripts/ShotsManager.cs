using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShotsManager : MonoBehaviour
{
    public BallManager ball;
    public TextManager text;

    private void Update()
    {
        text.SetText("" + ball.GetShots());
    }
}
