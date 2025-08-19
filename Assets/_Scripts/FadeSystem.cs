using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FadeSystem : MonoBehaviour
{
    public static FadeSystem Instance { get; private set; }

    [SerializeField] private Image fadePanel;
    [SerializeField] private float fadeDuration = 1.5f;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Скрываем панель при старте
        if (fadePanel != null)
            fadePanel.gameObject.SetActive(false);
    }

    // Затемнение экрана
    public IEnumerator FadeOut()
    {
        fadePanel.gameObject.SetActive(true);
        fadePanel.color = new Color(0, 0, 0, 0);

        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Clamp01(elapsed / fadeDuration);
            fadePanel.color = new Color(0, 0, 0, alpha);
            yield return null;
        }

        fadePanel.color = Color.black;
    }

    // Осветление экрана
    public IEnumerator FadeIn()
    {
        fadePanel.gameObject.SetActive(true);
        fadePanel.color = Color.black;

        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float alpha = 1 - Mathf.Clamp01(elapsed / fadeDuration);
            fadePanel.color = new Color(0, 0, 0, alpha);
            yield return null;
        }

        fadePanel.color = new Color(0, 0, 0, 0);
        fadePanel.gameObject.SetActive(false);
    }

    // Мгновенное затемнение
    public void InstantBlack()
    {
        fadePanel.gameObject.SetActive(true);
        fadePanel.color = Color.black;
    }

    // Мгновенное осветление
    public void InstantClear()
    {
        fadePanel.color = new Color(0, 0, 0, 0);
        fadePanel.gameObject.SetActive(false);
    }
}