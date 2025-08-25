using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System;

/// <summary>
/// ������� ��������, �������������� ����� ������, ���������� � ����� ��� ������ �����.
/// </summary>
public class Chapter2Scene : BaseChapterScene
{
  #region ����������� ����

  [Header("Input Settings")]

  /// <summary>
  /// ���� ����� ��� ���, ��������� �������������.
  /// </summary>
  [SerializeField] private InputField lifePathNumberInput;

  /// <summary>
  /// ������, ���������� �������� �����, ������������ ������������.
  /// </summary>
  [SerializeField] private GameObject inputPanel;

  [Header("Zodiac Responses")]

  /// <summary>
  /// ������, ���������� ������ ������� �� ������ ��������� ������� ���.
  /// </summary>
  [SerializeField] private AstrologicalIdentifierResponses lifePathNumberResponsesData;

  /// <summary>
  /// �������, �������� ������ �� ������ ��������� ������� ���, ��������������� �� ����� ���������� ����.
  /// ���� � ����� ���������� ����, �������� � ������ �������.
  /// </summary>
  private Dictionary<string, List<string>> lifePathNumberResponses;

  /// <summary>
  /// ����, �����������, ��������� �� ���� ���.
  /// </summary>
  private bool waitingForLifePathNumberInput = false;

  /// <summary>
  /// ����, �����������, ������������ �� ����� ��� � ������� ������.
  /// </summary>
  private bool isShowingLifePathNumberResponse = false;

  /// <summary>
  /// ������ ���������, ������������� � �������� �������� � ���.
  /// </summary>
  private int lifePathNumberQuestionReplicaIndex = 21;

  /// <summary>
  /// ������ ���������, ������������� � �������� ����������� ������� ���.
  /// </summary>
  private int lifePathNumberResponseReplicaIndex = 28;

  /// <summary>
  /// ������ ������� ���, ������� ����� ������� ������������.
  /// </summary>
  private List<string> lifePathNumberResponseToShow = null;

  #endregion

  #region ����������� ������

  /// <summary>
  /// ���������� ������� ������ ������������� ����� ���.
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
  /// �������������� ������������� ��� ������ �����.
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
  /// ��������������� ������� � ����������� ������ ����� ������ ����.
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
  /// ������������� ������� ������� �� ������ �������.
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
  /// �������� ����� ���.
  /// </summary>
  private IEnumerator WaitForLifePathNumberInput()
  {
    lifePathNumberInput.text = "";
    yield return new WaitWhile(() => waitingForLifePathNumberInput);
  }

  /// <summary>
  /// ���������� ��������� �� ������
  /// </summary>
  private void ShowErrorResponse()
  {
    List<string> responseSentences = new List<string> { "���, �������� ���." };
    StartCoroutine(ShowLifePathNumberResponse("oldster", responseSentences));
  }

  /// <summary>
  /// ��������� ��� ����� �������� ���� ���� �� ��������� ������������ �����
  /// </summary>
  /// <param name="date">���� ��� ����������</param>
  /// <returns>����������� ���</returns>
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
  /// ��������� ��� ����� �����
  /// </summary>
  /// <param name="number">�������� �����</param>
  /// <returns>����� ���� �����</returns>
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
  /// �������� ��� ������ ������ �� ������������.
  /// </summary>
  /// <param name="characterID">ID ���������.</param>
  /// <param name="sentences">������� ������.</param>
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

    if (sentences.Count == 1 && sentences[0] == "���, �������� ���.")
    {
      inputPanel.SetActive(true);
    }
  }

  /// <summary>
  /// �������� ����� ������� � ����������� �� ���.
  /// </summary>
  /// <param name="zodiacSign">���</param>
  /// <returns></returns>
  private List<string> GetLifePathNumberResponse(string zodiacSign)
  {
    if (lifePathNumberResponses != null && lifePathNumberResponses.ContainsKey(zodiacSign))
    {
      return lifePathNumberResponses[zodiacSign];
    }

    return new List<string>
        {
            "��� ������ ���������!"
        };
  }
  #endregion
}