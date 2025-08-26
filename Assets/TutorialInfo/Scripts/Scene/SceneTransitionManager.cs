using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

namespace Assets.TutorialInfo.Scripts
{
  /// <summary>
  /// Менеджер для управления переходами между сценами и эффектами затемнения/осветления.
  /// </summary>
  public class SceneTransitionManager : MonoBehaviour
  {
    /// <summary>
    /// Глобальная точка доступа к экземпляру менеджера.
    /// </summary>
    public static SceneTransitionManager Instance;

    /// <summary>
    /// Панель затемнения.
    /// </summary>
    private Image fadePanel;

    /// <summary>
    /// Длительность анимации.
    /// </summary>
    private float fadeDuration = 1.5f;

    /// <summary>
    /// Установка панели затемнения.
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

      CreateFadePanel();
      SceneManager.sceneLoaded += OnSceneLoaded;
    }

    /// <summary>
    /// Создает canvas и панель затемнения.
    /// </summary>
    void CreateFadePanel()
    {
      GameObject canvasObj = new GameObject("TransitionCanvas");
      Canvas canvas = canvasObj.AddComponent<Canvas>();
      CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
      canvasObj.AddComponent<GraphicRaycaster>();

      canvas.renderMode = RenderMode.ScreenSpaceOverlay;
      canvas.sortingOrder = 9999;

      scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
      scaler.referenceResolution = new Vector2(1920, 1080);
      scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
      scaler.matchWidthOrHeight = 0.5f;

      GameObject panelObj = new GameObject("FadePanel");
      panelObj.transform.SetParent(canvasObj.transform);

      fadePanel = panelObj.AddComponent<Image>();
      fadePanel.color = Color.black;

      RectTransform rt = panelObj.GetComponent<RectTransform>();

      rt.anchorMin = Vector2.zero;
      rt.anchorMax = Vector2.one;

      rt.offsetMin = Vector2.zero;
      rt.offsetMax = Vector2.zero;
      rt.pivot = new Vector2(0.5f, 0.5f);

      rt.anchoredPosition = Vector2.zero;
      rt.sizeDelta = Vector2.zero;

      rt.localScale = Vector3.one;

      panelObj.SetActive(false);

      DontDestroyOnLoad(canvasObj);
    }

    /// <summary>
    /// Обработчик события загрузки сцены.
    /// </summary>
    /// <param name="scene">Информация о загруженной сцене.</param>
    /// <param name="mode">Режим загрузки сцены.</param>
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
      StartCoroutine(FadeIn());
    }

    /// <summary>
    /// Загружает указанную сцену с эффектом плавного затемнения.
    /// </summary>
    /// <param name="sceneName">Имя сцены для загрузки.</param>
    public void LoadSceneWithFade(string sceneName)
    {
      StartCoroutine(TransitionCoroutine(sceneName));
    }

    /// <summary>
    /// Внутренняя корутина для обработки перехода между сценами.
    /// </summary>
    /// <param name="sceneName">Имя сцены для загрузки.</param>
    /// <returns></returns>
    private IEnumerator TransitionCoroutine(string sceneName)
    {
      yield return FadeOut();
      SceneManager.LoadScene(sceneName);
    }

    /// <summary>
    /// Плавное затемнение экрана.
    /// </summary>
    /// <param name="onComplete">callback-функция, которая будет вызвана после завершения затемнения.</param>
    public IEnumerator FadeOut(System.Action onComplete = null)
    {
      if (fadePanel == null) yield break;

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
      onComplete?.Invoke();
    }

    /// <summary>
    /// Плавное осветление экрана.
    /// </summary>
    /// <param name="onComplete">callback-функция, которая будет вызвана после завершения осветления.</param>
    public IEnumerator FadeIn(System.Action onComplete = null)
    {
      if (fadePanel == null) yield break;

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
      onComplete?.Invoke();
    }

    /// <summary>
    /// Плавная смена фона с затемнением.
    /// </summary>
    /// <param name="backgroundImage">Компонент, в котором нужно сменить фон.</param>
    /// <param name="newBackground">Новый спрайт для фона.</param>
    /// <param name="onChangeAction">Действие, выполняемое во время смены фона.</param>
    /// <returns></returns>
    public IEnumerator FadeBackground(Image backgroundImage, Sprite newBackground, System.Action onChangeAction = null)
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
      if (fadePanel == null) return;

      fadePanel.color = Color.black;
      fadePanel.gameObject.SetActive(true);
    }

    /// <summary>
    /// Мгновенное осветление.
    /// </summary>
    public void InstantClear()
    {
      if (fadePanel == null) return;

      fadePanel.color = new Color(0, 0, 0, 0);
      fadePanel.gameObject.SetActive(false);
    }
  }
}
