using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// Система затемнения/осветления экрана и плавной смены фона.
/// </summary>
public class FadeSystem : MonoBehaviour
{
    #region Поля и свойства

    /// <summary>
    /// Глобальный доступ к экземпляру.
    /// </summary>
    public static FadeSystem Instance { get; private set; }

    /// <summary>
    /// UI-панель для затемнения.
    /// </summary>
    [SerializeField] private Image fadePanel;

    /// <summary>
    /// Длительность анимации.
    /// </summary>
    [SerializeField] private float fadeDuration = 1.5f;

    #endregion

    #region Методы

    /// <summary>
    /// Инициализирует singleton
    /// </summary>
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (fadePanel != null)
            fadePanel.gameObject.SetActive(false);
    }

    /// <summary>
    /// Плавное затемнение экрана.
    /// </summary>
    /// <param name="onFadeComplete">callback-функция, которая будет вызвана после завершения затемнения.</param>
    public IEnumerator FadeOut(System.Action onFadeComplete = null)
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
        onFadeComplete?.Invoke();
    }

    /// <summary>
    /// Плавное осветление экрана.
    /// </summary>
    /// <returns></returns>
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

    /// <summary>
    /// Плавная смена фона с затемнением.
    /// </summary>
    /// <param name="backgroundImage">Компонент, в котором нужно сменить фон.</param>
    /// <param name="newBackground">Новый спрайт для фона.</param>
    /// <param name="onChangeAction">Действие, выполняемое во время смены фона.</param>
    /// <param name="duration">Длительность анимации смены фона.</param>
    /// <returns></returns>
    public IEnumerator FadeBackground(Image backgroundImage, Sprite newBackground, System.Action onChangeAction = null, float duration = 1.0f)
    {
        if (backgroundImage == null || newBackground == null) yield break;

        yield return FadeOut(() =>
        {
            backgroundImage.sprite = newBackground;

            onChangeAction?.Invoke();
        });

        yield return FadeIn();
    }

    /// <summary>
    /// Мгновенное затемнение.
    /// </summary>
    public void InstantBlack()
    {
        fadePanel.gameObject.SetActive(true);
        fadePanel.color = Color.black;
    }

    /// <summary>
    /// Мгновенное осветление.
    /// </summary>
    public void InstantClear()
    {
        fadePanel.color = new Color(0, 0, 0, 0);
        fadePanel.gameObject.SetActive(false);
    }

    #endregion
}