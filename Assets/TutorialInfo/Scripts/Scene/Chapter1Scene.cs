using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Система диалогов, обрабатывающая показ реплик, персонажей и фонов для первой главы.
/// </summary>
public class Chapter1Scene : BaseChapterScene
{
  #region Специфичные поля

  [Header("Input Settings")]

  /// <summary>
  /// Поле ввода для номера дома, введённого пользователем.
  /// </summary>
  [SerializeField] private InputField houseNumberInput;

  /// <summary>
  /// Панель, содержащая элементы ввода, отображаемая пользователю.
  /// </summary>
  [SerializeField] private GameObject inputPanel;

  [Header("Zodiac Responses")]

  /// <summary>
  /// Объект, содержащий данные ответов знаков зодиака.
  /// </summary>
  [SerializeField] private AstrologicalIdentifierResponses zodiacResponsesData;

  [Header("API Settings")]

  /// <summary>
  /// Менеджер API для взаимодействия с Free Astrology API для получения данных о знаках зодиака.
  /// </summary>
  [SerializeField] private FreeAstrologyAPIManager astrologyAPIManager;

  [Header("API Response Waiting Panel")]

  /// <summary>
  /// Панель, отображаемая в ожидании ответа от API.
  /// </summary>
  [SerializeField] private GameObject APIResponseWaitingPanel;

  /// <summary>
  /// Словарь, хранящий ответы знаков зодиака, сгруппированные по домам.
  /// Ключ — номер дома, значение — список ответов.
  /// </summary>
  private Dictionary<string, List<string>> zodiacResponses;

  /// <summary>
  /// Флаг, указывающий, ожидается ли ввод номера дома пользователем.
  /// </summary>
  private bool waitingForHouseInput = false;

  // <summary>
  /// Флаг, указывающий, отображается ли ответ знака зодиака в текущий момент.
  /// </summary>
  private bool isShowingZodiacResponse = false;

  /// <summary>
  /// Задача для выполнения операций, связанных с запросом знаков зодиака.
  /// </summary>
  private IEnumerator zodiacSignTask;

  /// <summary>
  /// Индекс сообщения, используемого в процессе вопросов о номере дома.
  /// </summary>
  private int houseNumberQuestionReplicaIndex = 78;

  #endregion

  #region Специфичные методы

  /// <summary>
  /// Обработчик нажатия кнопки подтверждения ввода номера дома.
  /// </summary>
  public void OnHouseNumberSubmit()
  {
    inputPanel.SetActive(false);

    if (houseNumberInput.text == "10")
    {
      APIResponseWaitingPanel.SetActive(true);
      waitingForHouseInput = false;
      StartCoroutine(GetZodiacSignFromAPI());
    }
    else
    {
      List<string> responseSentences = new List<string> { "Нет, попробуй ещё." };
      isShowingZodiacResponse = true;
      StartCoroutine(ShowZodiacResponse("oldster", responseSentences));
    }
  }

  /// <summary>
  /// Дополнительная инициализация для первой главы.
  /// </summary>
  protected override void Start()
  {
    base.Start();

    InitializeZodiacResponses();

    if (inputPanel != null)
    {
      inputPanel.SetActive(false);
    }
    if (inputPanel != null)
    {
      APIResponseWaitingPanel.SetActive(false);
    }
  }

  /// <summary>
  /// Воспроизведение диалога с добавлением логики ввода номера дома.
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
      if (isShowingZodiacResponse)
      {
        yield return new WaitWhile(() => isShowingZodiacResponse);
        continue;
      }

      currentLine = chapterData.lines[currentLineIndex];

      if (currentLineIndex == houseNumberQuestionReplicaIndex)
      {
        yield return ShowDialogueLine(currentLine);

        waitingForHouseInput = true;
        inputPanel.SetActive(true);

        yield return WaitForHouseInput();

        waitingForClick = true;
        yield return WaitForClick();
      }
      else
      {
        yield return ShowDialogueLine(currentLine);
        waitingForClick = true;
        yield return WaitForClick();
      }

      currentLineIndex++;
    }

    yield return ShowFinalScreen();
  }

  /// <summary>
  /// Очистка ресурсов.
  /// </summary>
  protected override void OnDestroy()
  {
    base.OnDestroy();

    // Останавливаем корутину, если она запущена
    if (zodiacSignTask != null)
    {
      StopCoroutine(zodiacSignTask);
    }
  }

  /// <summary>
  /// Инициализация словаря ответов по знакам зодиака.
  /// </summary>
  private void InitializeZodiacResponses()
  {
    zodiacResponses = new Dictionary<string, List<string>>();

    if (zodiacResponsesData != null && zodiacResponsesData.responses != null)
    {
      foreach (var response in zodiacResponsesData.responses)
      {
        if (!zodiacResponses.ContainsKey(response.astrologicalIdentifier))
        {
          zodiacResponses.Add(response.astrologicalIdentifier, response.sentences);
        }
      }
    }
    else
    {
      Debug.LogWarning("Zodiac responses data is not assigned!");
    }
  }

  /// <summary>
  /// Ожидание ввода номера дома.
  /// </summary>
  private IEnumerator WaitForHouseInput()
  {
    houseNumberInput.text = "";
    yield return new WaitWhile(() => waitingForHouseInput);
  }

  /// <summary>
  /// Получение знака зодиака из API.
  /// </summary>
  private IEnumerator GetZodiacSignFromAPI()
  {
    string zodiacSign = "";

    yield return astrologyAPIManager.GetTenthHouseSign((sign) => {
      zodiacSign = sign;
    });

    List<string> responseSentences = GetZodiacResponse(zodiacSign);
    isShowingZodiacResponse = true;
    StartCoroutine(ShowZodiacResponse("oldster", responseSentences));
  }

  /// <summary>
  /// Корутина для показа ответа по предложениям.
  /// </summary>
  /// <param name="characterID">ID персонажа.</param>
  /// <param name="sentences">Реплики ответа старика.</param>
  /// <returns></returns>
  private IEnumerator ShowZodiacResponse(string characterID, List<string> sentences)
  {
    APIResponseWaitingPanel.SetActive(false);
    foreach (string sentence in sentences)
    {
      DialogueLine responseLine = new DialogueLine
      {
        characterID = characterID,
        text = sentence,
        position = CharacterPosition.Left,
        showCharacter = true
      };

      yield return ShowDialogueLine(responseLine);
      waitingForClick = true;
      yield return WaitForClick();
    }

    isShowingZodiacResponse = false;

    if (sentences.Count == 1 && sentences[0] == "Нет, попробуй ещё.")
    {
      inputPanel.SetActive(true);
    }
  }

  /// <summary>
  /// Получает ответ старика в зависимости от знака зодиака.
  /// </summary>
  /// <param name="zodiacSign">Знак зодиака.</param>
  /// <returns></returns>
  private List<string> GetZodiacResponse(string zodiacSign)
  {
    if (zodiacResponses != null && zodiacResponses.ContainsKey(zodiacSign))
    {
      return zodiacResponses[zodiacSign];
    }

    return new List<string>
        {
            "Верно! Нужно посмотреть на 10 дом! А он у тебя. Ах, вот оно что! Это многое объясняет!"
        };
  }

  #endregion
}