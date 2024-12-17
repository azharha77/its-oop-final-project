using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeController : MonoBehaviour
{
    public Image fadeImage;
    public float fadeDuration = 2f;

    private void Start()
    {
        if (fadeImage == null) { Debug.LogError("Fade Image is not assigned in the FadeController script."); }
        else { StartCoroutine(FadeIn()); }
    }

    public IEnumerator FadeIn()
    {
        float timer = 0f;

        fadeImage.color = new Color(0, 0, 0, 1);

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            float alpha = Mathf.Lerp(1, 0, timer / fadeDuration);
            fadeImage.color = new Color(0, 0, 0, alpha);
            yield return null;
        }

        fadeImage.color = new Color(0, 0, 0, 0);
    }

    public IEnumerator FadeOut()
    {
        float timer = 0f;

        fadeImage.color = new Color(0, 0, 0, 0);

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            float alpha = Mathf.Lerp(0, 1, timer / fadeDuration);
            fadeImage.color = new Color(0, 0, 0, alpha);
            yield return null;
        }

        fadeImage.color = new Color(0, 0, 0, 1);
    }
}
