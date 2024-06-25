using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneTransition : MonoBehaviour
{
    public Image fadeImage;
    public float fadeDuration = 1f;

    void Start()
    {
        fadeImage.color = new Color(fadeImage.color.r, fadeImage.color.g, fadeImage.color.b, 0);
    }

    public void FadeToNextScene(int sceneIndex)
    {
        StartCoroutine(FadeOut(sceneIndex));
    }

    IEnumerator FadeOut(int sceneIndex)
    {
        float timer = 0f;
        Color fadeColor = fadeImage.color;
        while (timer <= fadeDuration)
        {
            float alpha = Mathf.Lerp(0, 1, timer / fadeDuration);
            fadeColor.a = alpha;
            fadeImage.color = fadeColor;
            timer += Time.deltaTime;
            yield return null;
        }

        fadeColor.a = 1;
        fadeImage.color = fadeColor;

        SceneManager.LoadScene(sceneIndex);
        StartCoroutine(FadeIn());
    }

    IEnumerator FadeIn()
    {
        float timer = 0f;
        Color fadeColor = fadeImage.color;
        while (timer <= fadeDuration)
        {
            float alpha = Mathf.Lerp(1, 0, timer / fadeDuration);
            fadeColor.a = alpha;
            fadeImage.color = fadeColor;
            timer += Time.deltaTime;
            yield return null;
        }

        fadeColor.a = 0;
        fadeImage.color = fadeColor;
    }
}
