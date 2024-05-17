using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FPSDisplay : MonoBehaviour
{
    public TMP_Text fpsText;

    private float deltaTime = 0.0f;
    private float totalTime = 0.0f;
    private int frameCount = 0;

    private void Update()
    {
        deltaTime += Time.deltaTime;
        totalTime += Time.timeScale / Time.deltaTime;
        frameCount++;

        if (deltaTime >= 0.5f)
        {
            float fps = totalTime / frameCount;
            fpsText.text = "FPS: " + Mathf.RoundToInt(fps);

            // Reset variables
            deltaTime = 0.0f;
            totalTime = 0.0f;
            frameCount = 0;
        }
    }
}