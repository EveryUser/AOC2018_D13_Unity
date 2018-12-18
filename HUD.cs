using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HUD : MonoBehaviour {

    public UnityEngine.UI.Text SpeedText;
    public UnityEngine.UI.Text TickText;
    public UnityEngine.UI.Text RemainingCartsText;
    public UnityEngine.UI.Text PartOneText;
    public UnityEngine.UI.Text PartTwoText;

    private bool FirstCollisionSet = false;
    private bool LastCartSet = false;

    public void UpdateSpeedText(int speed)
    {
        SpeedText.text = "Speed: " + speed.ToString();
    }

    public void SetTicksText(int ticks)
    {
        TickText.text = "Ticks: " + ticks.ToString("N0");
    }

    public void SetRemainingCartText(int remainingCarts)
    {
        RemainingCartsText.text = "Remaining Carts: " + remainingCarts.ToString();
    }

    public void UpdateFirstCollisionText(Vector2Int coordinate)
    {
        if (!FirstCollisionSet)
        {
            PartOneText.text = "FirstCollision: " + coordinate.x + "," + coordinate.y;
            FirstCollisionSet = true;
        }
    }

    public void UpdateLastCartText(Vector2Int coordinate)
    {
        if (!LastCartSet)
        {
            PartTwoText.text = "Last Cart: " + coordinate.x + "," + coordinate.y;
            LastCartSet = true;
        }
    }
}
