using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DigitalDisplay : MonoBehaviour
{
    private TextMeshProUGUI displayText;

    private void OnEnable()
    {
        displayText = GetComponentInChildren<TextMeshProUGUI>();
    }

    public void SetDisplay(string text)
    {
        displayText.text = text;
    }

    public void SetDisplayViaFloat(float text)
    {
        displayText.text = Round(text).ToString();
    }

    public void SetDisplay(int text)
    {
        displayText.text = text.ToString();
    }

    public void SetDisplay(Vector2 value)
    {
        displayText.text = $"X: {Round(value.x)}\nY: {Round(value.y)}";
    }

    public float Round(float input)
    {
        return Mathf.Round(input * 100) / 100.0f;
    }
}
