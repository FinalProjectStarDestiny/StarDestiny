using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System;

/// <summary>
/// Система диалогов, обрабатывающая показ реплик, персонажей и фонов для первой главы.
/// </summary>
public class Chapter2Scene : BaseChapterScene
{
  #region Специфичные поля

  [Header("Input Settings")]

  /// <summary>
  /// Поле ввода для ЧЖП, введённого пользователем.
  /// </summary>
  [SerializeField] private InputField lifePathNumberInput;

  /// <summary>
  /// Панель, содержащая элементы ввода, отображаемая пользователю.
  /// </summary>
  [SerializeField] private GameObject inputPanel;

  [Header("Zodiac Responses")]

  /// <summary>
  /// Объект, содержащий данные ответов на каждый возможный вариант ЧЖП.
  /// </summary>
  [SerializeField] private AstrologicalIdentifierResponses lifePathNumberResponsesData;

  /// <summary>
  /// Словарь, хранящий ответы на каждый возможный вариант ЧЖП, сгруппированные по числу жизненного пути.
  /// Ключ — Число жизненного пути, значение — список ответов.
  /// </summary>
  private Dictionary<string, List<string>> lifePathNumberResponses;

  /// <summary>
  /// Флаг, указывающий, ожидается ли ввод ЧЖП.
  /// </summary>
  private bool waitingForLifePathNumberInput = false;

  /// <summary>
  /// Флаг, указывающий, отображается ли ответ ЧЖП в текущий момент.
  /// </summary>
  private bool isShowingLifePathNumberResponse = false;

  /// <summary>
  /// Индекс сообщения, используемого в процессе вопросов о ЧЖП.
  /// </summary>
  private int lifePathNumberQuestionReplicaIndex = 21;

  /// <summary>
  /// Индекс сообщения, используемого в процессе отображения ответов ЧЖП.
  /// </summary>
  private int lifePathNumberResponseReplicaIndex = 28;

  /// <summary>
  /// Список ответов ЧЖП, который будет показан пользователю.
  /// </summary>
  private List<string> lifePathNumberResponseToShow = null;

  #endregion

  #region Специфичные методы

  /// <summary>
  /// Обработчик нажатия кнопки подтверждения ввода ЧЖП.
  /// </summary>
  public void OnLifePathNumberSubmit()
  {
    isShowingLifePathNumberResponse = true;
    inputPanel.SetActive(false);

    if (!int.TryParse(lifePathNumberInput.text, out int number))
    {
      ShowErrorResponse();
      return;
    }

    if (number <= 0 || number >= 10)
    {
      ShowErrorResponse();
      return;
    }

    DateTime birthDate;
    birthDate = DateTime.Parse(PlayerPrefs.GetString("BirthDate"));

    if (CalculateDateNumber(birthDate) == number)
    {
      lifePathNumberResponseToShow = GetLifePathNumberResponse(number.ToString());
      waitingForLifePathNumberInput = false;
      isShowingLifePathNumberResponse = false;
    }
    else
    {
      ShowErrorResponse();
    }
  }

  /// <summary>
  /// Дополнительная инициализация для первой главы.
  /// </summary>
  protected override void Start()
  {
    base.Start();

    InitializeLifePathNumberResponses();

    if (inputPanel != null)
    {
      inputPanel.SetActive(false);
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
      if (isShowingLifePathNumberResponse)
      {
        yield return new WaitWhile(() => isShowingLifePathNumberResponse);
        continue;
      }

      currentLine = chapterData.lines[currentLineIndex];

      if (currentLineIndex == lifePathNumberResponseReplicaIndex)
      {
        yield return ShowLifePathNumberResponse("oldster", lifePathNumberResponseToShow);
        currentLineIndex++;
        continue;
      }

      if (currentLineIndex == lifePathNumberQuestionReplicaIndex)
      {
        yield return ShowDialogueLine(currentLine);

        waitingForLifePathNumberInput = true;
        inputPanel.SetActive(true);

        yield return WaitForLifePathNumberInput();
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
  /// Инициализация словаря ответов по знакам зодиака.
  /// </summary>
  private void InitializeLifePathNumberResponses()
  {
    lifePathNumberResponses = new Dictionary<string, List<string>>();

    if (lifePathNumberResponsesData != null && lifePathNumberResponsesData.responses != null)
    {
      foreach (var response in lifePathNumberResponsesData.responses)
      {
        if (!lifePathNumberResponses.ContainsKey(response.astrologicalIdentifier))
        {
          lifePathNumberResponses.Add(response.astrologicalIdentifier, response.sentences);
        }
      }
    }
    else
    {
      Debug.LogWarning("Zodiac responses data is not assigned!");
    }
  }

  /// <summary>
  /// Ожидание ввода ЧЖП.
  /// </summary>
  private IEnumerator WaitForLifePathNumberInput()
  {
    lifePathNumberInput.text = "";
    yield return new WaitWhile(() => waitingForLifePathNumberInput);
  }

  /// <summary>
  /// Показывает сообщение об ошибке
  /// </summary>
  private void ShowErrorResponse()
  {
    List<string> responseSentences = new List<string> { "Нет, попробуй ещё." };
    StartCoroutine(ShowLifePathNumberResponse("oldster", responseSentences));
  }

  /// <summary>
  /// Вычисляет ЧЖП путем сложения всех цифр до получения однозначного числа
  /// </summary>
  /// <param name="date">Дата для вычисления</param>
  /// <returns>Однозначное ЧЖП</returns>
  private int CalculateDateNumber(DateTime date)
  {
    int day = date.Day;
    int month = date.Month;
    int year = date.Year;

    int sum = SumDigits(day) + SumDigits(month) + SumDigits(year);

    while (sum > 9)
    {
      sum = SumDigits(sum);
    }

    return sum;
  }

  /// <summary>
  /// Суммирует все цифры числа
  /// </summary>
  /// <param name="number">Исходное число</param>
  /// <returns>Сумма цифр числа</returns>
  private int SumDigits(int number)
  {
    int sum = 0;
    while (number > 0)
    {
      sum += number % 10;
      number /= 10;
    }
    return sum;
  }

  /// <summary>
  /// Корутина для показа ответа по предложениям.
  /// </summary>
  /// <param name="characterID">ID персонажа.</param>
  /// <param name="sentences">Реплики ответа.</param>
  /// <returns></returns>
  private IEnumerator ShowLifePathNumberResponse(string characterID, List<string> sentences)
  {
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

    isShowingLifePathNumberResponse = false;

    if (sentences.Count == 1 && sentences[0] == "Нет, попробуй ещё.")
    {
      inputPanel.SetActive(true);
    }
  }

  /// <summary>
  /// Получает ответ старика в зависимости от ЧЖП.
  /// </summary>
  /// <param name="zodiacSign">ЧЖП</param>
  /// <returns></returns>
  private List<string> GetLifePathNumberResponse(string zodiacSign)
  {
    if (lifePathNumberResponses != null && lifePathNumberResponses.ContainsKey(zodiacSign))
    {
      return lifePathNumberResponses[zodiacSign];
    }

    return new List<string>
        {
            "Это многое объясняет!"
        };
  }
  #endregion
}