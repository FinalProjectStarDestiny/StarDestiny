using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Assets.TutorialInfo.Scripts;
using System.Text.RegularExpressions;
using TMPro;

/// <summary>
/// Система диалогов, обрабатывающая показ реплик и ежедневного расклада Таро.
/// </summary>
public class Chapter3Scene : BaseChapterScene
{
  #region Специфичные поля

  [Header("Tarot API Settings")]

  /// <summary>
  /// Ссылка на менеджер API для взаимодействия с Vedic Astrology API для получения предсказаний таро.
  /// </summary>
  [SerializeField] private VedicAstroAPIManager tarotAPIManager;

  [Header("Tarot Draw Panel")]

  /// <summary>
  /// Панель, отображающая карты таро во время розыгрыша.
  /// </summary>
  [SerializeField] private GameObject tarotDrawPanel;

  /// <summary>
  /// Изображение обратной стороны карты, отображаемое на панели.
  /// </summary>
  [SerializeField] private Image cardBackImage;

  /// <summary>
  /// Изображение лицевой стороны карты, отображаемое на панели.
  /// </summary>
  [SerializeField] private Image cardFrontImage;

  /// <summary>
  /// Индикатор загрузки, отображаемый во время выполнения операций.
  /// </summary>
  [SerializeField] private GameObject loadingIndicator;

  [Header("Timing Settings")]

  /// <summary>
  /// Задержка перед раскрытием карты, в секундах.
  /// </summary>
  [SerializeField] private float cardRevealDelay = 1.0f;

  /// <summary>
  /// Максимальное количество символов, отображаемых в одном сегменте текста предсказания.
  /// </summary>
  [SerializeField] private int maxCharactersPerSegment = 90;

  [Header("Tarot UI Elements")]

  /// <summary>
  /// Текстовое сообщение, которое предлагается нажать для розыгрыша карты.
  /// </summary>
  [SerializeField] private GameObject tapToDrawText;

  /// <summary>
  /// Текстовое сообщение, которое предлагается нажать для раскрытия карты.
  /// </summary>
  [SerializeField] private GameObject tapToRevealText;

  [Header("Music")]

  /// <summary>
  /// Звук перелистывания карт.
  /// </summary>
  [SerializeField] private AudioClip tarotMusic;

  /// <summary>
  /// Базовая музыка главы.
  /// </summary>
  [SerializeField] private AudioClip baseChapterMusic;

  /// <summary>
  /// Флаг, указывающий, показывается ли результат таро в текущий момент.
  /// </summary>
  private bool isShowingTarotResult = false;

  /// <summary>
  /// Задача для выполнения предсказания таро в виде корутины.
  /// </summary>
  private IEnumerator tarotReadingTask;

  /// <summary>
  /// Индекс сообщения, используемого в процессе чтения таро.
  /// </summary>
  private int tarotReadingReplicaIndex = 14;

  /// <summary>
  /// Данные предсказания таро, полученные из API.
  /// </summary>
  private TarotPredictionData predictionData;

  /// <summary>
  /// Флаг, указывающий, была ли карта успешно вытянута.
  /// </summary>
  private bool isCardDrawn = false;

  /// <summary>
  /// Обработчик кликов для панели таро.
  /// </summary>
  private ContentPanelClickHandler tarotPanelClickHandler;

  /// <summary>
  /// Флаг, указывающий, ожидается ли клик на карте.
  /// </summary>
  private bool waitingForCardClick = false;

  #endregion

  #region Специфичные методы

  /// <summary>
  /// Обработчик нажатия на рубашку карты.
  /// </summary>
  public void OnCardBackClicked()
  {
    if (!isCardDrawn && !isShowingTarotResult)
    {
      if (tapToDrawText != null)
      {
        tapToDrawText.SetActive(false);
      }

      AudioManager.Instance.ChangeMusic(tarotMusic);

      if (loadingIndicator != null)
      {
        loadingIndicator.SetActive(true);
      }

      StartCoroutine(DrawTarotCard());
    }
  }

  /// <summary>
  /// Обработчик нажатия на панель Таро.
  /// </summary>
  public void OnTarotPanelClicked()
  {
    if (waitingForCardClick)
    {
      waitingForCardClick = false;
    }
    else if (isCardDrawn && cardFrontImage != null && cardFrontImage.gameObject.activeSelf)
    {
      HideTarotDrawPanel();
    }
  }

  /// <summary>
  /// Дополнительная инициализация для сцены с Таро.
  /// </summary>
  protected override void Start()
  {
    base.Start();

    if (tarotDrawPanel != null)
    {
      tarotDrawPanel.SetActive(false);
      SetupTarotDrawPanel();
    }

    if (loadingIndicator != null)
    {
      loadingIndicator.SetActive(false);
    }

    if (cardFrontImage != null)
    {
      cardFrontImage.gameObject.SetActive(false);
    }
  }

  /// <summary>
  /// Воспроизведение диалога с добавлением логики показа расклада Таро.
  /// </summary>
  protected override IEnumerator PlayChapter()
  {
    if (chapterData.lines == null || chapterData.lines.Count == 0)
    {
      Debug.LogError("Chapter has no lines!");
      yield break;
    }

    while (currentLineIndex < chapterData.lines.Count)
    {
      if (isShowingTarotResult)
      {
        yield return new WaitWhile(() => isShowingTarotResult);
        continue;
      }

      currentLine = chapterData.lines[currentLineIndex];

      if (currentLine.audioChange != null)
      {
        AudioManager.Instance.ChangeMusic(currentLine.audioChange);
      }

      if (currentLineIndex == tarotReadingReplicaIndex)
      {
        yield return ShowDialogueLine(currentLine);
        waitingForClick = true;
        yield return WaitForClick();

        currentLineIndex++;

        yield return SceneTransitionManager.Instance.FadeOut();

        ShowTarotDrawPanel();

        yield return SceneTransitionManager.Instance.FadeIn();

        yield return new WaitUntil(() => isCardDrawn);

        yield return SceneTransitionManager.Instance.FadeOut();

        HideTarotDrawPanel();

        DialogueLine transitionLine = new DialogueLine
        {
          characterID = "oldster",
          text = "Посмотрим... тебе выпала карта...",
          position = CharacterPosition.Left,
          showCharacter = true
        };

        yield return ShowDialogueLine(transitionLine);

        yield return SceneTransitionManager.Instance.FadeIn();

        waitingForClick = true;
        yield return WaitForClick();

        isShowingTarotResult = true;
        yield return ShowTarotPredictionResponse(predictionData);

        currentLineIndex++;
        continue;
      }
      else
      {
        yield return ShowDialogueLine(currentLine);
        waitingForClick = true;
        yield return WaitForClick();
        currentLineIndex++;
      }
    }

    yield return ShowFinalScreen();
  }

  /// <summary>
  /// Очистка ресурсов.
  /// </summary>
  protected override void OnDestroy()
  {
    base.OnDestroy();

    if (tarotPanelClickHandler != null)
    {
      tarotPanelClickHandler.OnClick -= OnTarotPanelClicked;
    }

    if (tarotReadingTask != null)
    {
      StopCoroutine(tarotReadingTask);
    }
  }

  /// <summary>
  /// Настройка панели для вытягивания карты Таро.
  /// </summary>
  private void SetupTarotDrawPanel()
  {
    if (tarotDrawPanel != null)
    {
      tarotPanelClickHandler = tarotDrawPanel.GetComponent<ContentPanelClickHandler>();
      if (tarotPanelClickHandler == null)
      {
        tarotPanelClickHandler = tarotDrawPanel.AddComponent<ContentPanelClickHandler>();
      }
      tarotPanelClickHandler.OnClick += OnTarotPanelClicked;
    }
  }

  /// <summary>
  /// Показывает панель для вытягивания карты Таро.
  /// </summary>
  private void ShowTarotDrawPanel()
  {
    if (tarotDrawPanel != null)
    {
      tarotDrawPanel.SetActive(true);
    }

    isCardDrawn = false;
    waitingForCardClick = false;

    if (cardBackImage != null)
    {
      cardBackImage.gameObject.SetActive(true);
    }

    if (cardFrontImage != null)
    {
      cardFrontImage.gameObject.SetActive(false);
    }

    if (loadingIndicator != null)
    {
      loadingIndicator.SetActive(false);
    }

    if (tapToDrawText != null)
    {
      tapToDrawText.SetActive(true);
    }

    if (tapToRevealText != null)
    {
      tapToRevealText.SetActive(false);
    }
  }

  /// <summary>
  /// Процесс вытягивания карты Таро.
  /// </summary>
  private IEnumerator DrawTarotCard()
  {
    isShowingTarotResult = true;

    string error = null;

    yield return tarotAPIManager.GetDailyTarotPrediction((data, err) => {
      predictionData = data;
      error = err;
    });

    if (loadingIndicator != null)
    {
      loadingIndicator.SetActive(false);
    }

    AudioManager.Instance.ChangeMusic(baseChapterMusic);

    if (!string.IsNullOrEmpty(error) || predictionData == null)
    {
      Debug.LogError(error ?? "Не удалось получить данные от API");

      DialogueLine errorLine = new DialogueLine
      {
        characterID = "oldster",
        text = "Извините, не удалось получить расклад Таро. Попробуйте позже.",
        position = CharacterPosition.Left,
        showCharacter = true
      };

      StartCoroutine(ShowDialogueLine(errorLine));
      waitingForClick = true;
      yield return WaitForClick();

      yield return HideTarotDrawPanelWithFade();
      isShowingTarotResult = false;
      yield break;
    }

    if (cardFrontImage != null && predictionData.card_image != null)
    {
      yield return tarotAPIManager.LoadCardImage(predictionData.card_image.classic, (texture) =>
      {
        if (texture != null)
        {
          cardFrontImage.sprite = Sprite.Create(
              texture,
              new Rect(0, 0, texture.width, texture.height),
              new Vector2(0.5f, 0.5f)
          );
          cardFrontImage.preserveAspect = true;
          cardFrontImage.gameObject.SetActive(true);
        }
      });
    }

    if (cardBackImage != null)
    {
      cardBackImage.gameObject.SetActive(false);
    }

    if (tapToRevealText != null)
    {
      tapToRevealText.SetActive(true);
    }

    yield return new WaitForSeconds(cardRevealDelay);

    waitingForCardClick = true;
    yield return new WaitWhile(() => waitingForCardClick);

    if (tapToRevealText != null)
    {
      tapToRevealText.SetActive(false);
    }

    isCardDrawn = true;
    isShowingTarotResult = false;
  }

  /// <summary>
  /// Скрывает панель для вытягивания карты Таро.
  /// </summary>
  private void HideTarotDrawPanel()
  {
    if (tarotDrawPanel != null)
    {
      tarotDrawPanel.SetActive(false);
    }

    if (tapToDrawText != null)
    {
      tapToDrawText.SetActive(false);
    }

    if (tapToRevealText != null)
    {
      tapToRevealText.SetActive(false);
    }

    if (loadingIndicator != null)
    {
      loadingIndicator.SetActive(false);
    }
  }

  /// <summary>
  /// Скрывает панель для вытягивания карты Таро с затемнением.
  /// </summary>
  private IEnumerator HideTarotDrawPanelWithFade()
  {
    yield return SceneTransitionManager.Instance.FadeOut();

    if (tarotDrawPanel != null)
    {
      tarotDrawPanel.SetActive(false);
    }

    yield return SceneTransitionManager.Instance.FadeIn();
  }

  /// <summary>
  /// Разделяет текст на предложения по точкам, восклицательным и вопросительным знакам.
  /// </summary>
  private List<string> SplitIntoSentences(string text)
  {
    List<string> sentences = new List<string>();

    if (string.IsNullOrEmpty(text))
      return sentences;

    string pattern = @"(?<=[.!?])\s+";
    string[] splitSentences = Regex.Split(text, pattern);

    foreach (string sentence in splitSentences)
    {
      if (sentence.Length > maxCharactersPerSegment)
      {
        List<string> parts = SplitLongText(sentence, maxCharactersPerSegment);
        foreach(string part in parts)
        {
          sentences.Add(part);
        }
      }
      else
      {
        if (!string.IsNullOrWhiteSpace(sentence))
        {
          sentences.Add(sentence.Trim());
        }
      }
    }

    return sentences;
  }

  /// <summary>
  /// Разделяет длинный текст на части по максимальному количеству символов.
  /// </summary>
  private List<string> SplitLongText(string text, int maxLength)
  {
    List<string> parts = new List<string>();

    string[] words = text.Split(' ');
    string currentPart = "";

    foreach (string word in words)
    {
      if ((currentPart + " " + word).Length > maxLength && !string.IsNullOrEmpty(currentPart))
      {
        parts.Add(currentPart.Trim());
        currentPart = "";
      }

      currentPart += " " + word;
    }

    if (!string.IsNullOrEmpty(currentPart.Trim()))
    {
      parts.Add(currentPart.Trim());
    }

    return parts;
  }

  /// <summary>
  /// Корутина для показа ответа по предложениям с разделением ответа API на части.
  /// </summary>
  /// <param name="characterID">ID персонажа.</param>
  /// <param name="predictionData">Данные предсказания Таро.</param>
  /// <returns></returns>
  private IEnumerator ShowTarotPredictionResponse(TarotPredictionData predictionData)
  {
    isShowingTarotResult = true;

    List<string> predictionSentences = new List<string>();

    predictionSentences.Add($"Это карта - {predictionData.name}");

    if (!string.IsNullOrEmpty(predictionData.health))
    {
      predictionSentences.Add("О здоровье твоём и внутренних силах...");
      predictionSentences.AddRange(SplitIntoSentences(predictionData.health));
    }

    if (!string.IsNullOrEmpty(predictionData.relationship))
    {
      predictionSentences.Add("Сердечные дела и взаимоотношения...");
      predictionSentences.AddRange(SplitIntoSentences(predictionData.relationship));
    }

    if (!string.IsNullOrEmpty(predictionData.career))
    {
      predictionSentences.Add("В вопросах карьеры и предназначения...");
      predictionSentences.AddRange(SplitIntoSentences(predictionData.career));
    }

    if (!string.IsNullOrEmpty(predictionData.finance))
    {
      predictionSentences.Add("Что ждёт тебя в вопросах финансового положения..");
      predictionSentences.AddRange(SplitIntoSentences(predictionData.finance));
    }

    foreach (string sentence in predictionSentences)
    {
      DialogueLine responseLine = new DialogueLine
      {
        characterID = "oldster",
        text = sentence,
        position = CharacterPosition.Left,
        showCharacter = true
      };

      yield return ShowDialogueLine(responseLine);
      waitingForClick = true;
      yield return WaitForClick();
    }

    isShowingTarotResult = false;
  }

  #endregion
}