using System.Security.Cryptography;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class warning_manager : MonoBehaviour
{
    public float fadeDuration = 2f;
    private float elapsedTime = 0f;

    private CanvasRenderer r1, r2;

    void Start()
    {
        // Get the CanvasRenderer component
        r1 = GetComponent<CanvasRenderer>();
        r2 = transform.Find("msg").GetComponent<CanvasRenderer>();

        r1.SetAlpha(0);
        r2.SetAlpha(0);
    }

    public void start_fading()
    {
        elapsedTime = 0f;
    }

    void Update()
    {
        if (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;

            // Calculate the alpha based on elapsed time
            float alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeDuration);

            // Update the alpha of the CanvasRenderer
            r1.SetAlpha(alpha);
            r2.SetAlpha(alpha);
        }
        else
        {
            // Ensure alpha is exactly 0 at the end
            r1.SetAlpha(0f);
            r2.SetAlpha(0f);
        }
    }
}