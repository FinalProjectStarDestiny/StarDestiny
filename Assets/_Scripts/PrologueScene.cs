using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

/// <summary>
/// Система диалогов, обрабатывающая показ реплик, персонажей и фонов.
/// </summary>
public class PrologueScene : MonoBehaviour
{
  #region Поля и свойства

  [Header("UI References")]
  /// <summary>Фоновое изображение.</summary>
  [SerializeField] private Image backgroundImage;

  /// <summary>Имя персонажа.</summary>
  [SerializeField] private Text speakerNameText;

  /// <summary>Текст реплики персонажа.</summary>
  [SerializeField] private Text dialogueText;

  /// <summary>Панель для отображения реплики персонажа.</summary>
  [SerializeField] private GameObject dialoguePanel;

  [Header("Character Slots")]
  /// <summary>Слот для персонажа слева.</summary>
  [SerializeField] private Image leftCharacterSlot;

  /// <summary>Слот для персонажа справа.</summary>
  [SerializeField] private Image rightCharacterSlot;

  /// <summary>Слот для центрального персонажа.</summary>
  [SerializeField] private Image centerCharacterSlot;

  [Header("Prologue Settings")]

  /// <summary>Данные пролога.</summary>
  [SerializeField] private ChapterData prologueChapter;

  /// <summary>Панель котента.</summary>
  [SerializeField] private GameObject contentPanel;

  [Header("Fade Settings")]
  /// <summary>Продолжительность фонового перехода.</summary>
  [SerializeField] private float backgroundTransitionDuration = 1.0f;

  [Header("Final Screen Settings")]
  /// <summary>Панель для финального экрана.</summary>
  [SerializeField] private GameObject finalScreen;

  /// <summary>Название следующей сцены.</summary>
  [SerializeField] private string nextSceneName = "NextScene";

  /// <summary>Текст финального экрана.</summary>
  [SerializeField] private string finalScreenText;

  [Header("Main Character")]

  /// <summary>Объект основного персонажа</summary>
  public CharacterData mainCharacterData;

  /// <summary>Хранилище данных персонажей.</summary>
  private Dictionary<string, CharacterData> characterDatabase;

  /// <summary>Индекс текущей реплики в диалоге.</summary>
  private int currentLineIndex = 0;

  /// <summary>Состояние ожидания отклика.</summary>
  private bool waitingForClick = false;

  /// <summary>Текущая реплика в диалоге.</summary>
  private DialogueLine currentLine;

  /// <summary>Обработчик кликов для финального окна.</summary>
  private ContentPanelClickHandler finalScreenClickHandler;

  #endregion

  #region Методы
  /// <summary>
  /// Инициализация системы диалогов при запуске сцены.
  /// </summary>
  private void Start()
  {
    if (contentPanel != null)
    {
      Image panelImage = contentPanel.GetComponent<Image>();

      if (panelImage == null)
      {
        panelImage = contentPanel.AddComponent<Image>();
      }

      ContentPanelClickHandler clickHandler = contentPanel.GetComponent<ContentPanelClickHandler>();

      if (clickHandler == null)
      {
        clickHandler = contentPanel.AddComponent<ContentPanelClickHandler>();
      }

      clickHandler.OnClick += OnContentPanelClicked;
    }

    if (finalScreen != null)
    {
      finalScreenClickHandler = finalScreen.GetComponent<ContentPanelClickHandler>();

      if (finalScreenClickHandler == null)
      {
        finalScreenClickHandler = finalScreen.AddComponent<ContentPanelClickHandler>();
      }

      Text finalText = finalScreen.GetComponentInChildren<Text>();

      if (finalText != null)
      {
        finalText.text = finalScreenText;
      }

      finalScreen.SetActive(false);
    }

    if(mainCharacterData!=null)
      mainCharacterData.ChangeCharacterData();

    characterDatabase = new Dictionary<string, CharacterData>();
    LoadCharacters();
    StartPrologue();
  }

  /// <summary>
  /// Очистка ресурсов при уничтожении объекта.
  /// </summary>
  private void OnDestroy()
  {
    if (contentPanel != null)
    {
      ContentPanelClickHandler clickHandler = contentPanel.GetComponent<ContentPanelClickHandler>();
      if (clickHandler != null)
      {
        clickHandler.OnClick -= OnContentPanelClicked;
      }
    }

    if (finalScreenClickHandler != null)
    {
      finalScreenClickHandler.OnClick -= OnFinalScreenClicked;
    }
  }

  /// <summary>
  /// Обработчик клика по панели контента.
  /// </summary>
  private void OnContentPanelClicked()
  {
    if (waitingForClick)
    {
      waitingForClick = false;
    }
  }

  /// <summary>
  /// Загрузка данных персонажей из ресурсов.
  /// </summary>
  void LoadCharacters()
  {
    CharacterData[] characters = Resources.LoadAll<CharacterData>("Characters");

    if (characters == null || characters.Length == 0)
    {
      Debug.LogWarning("No characters found in Resources/Characters!");
      return;
    }

    foreach (CharacterData character in characters)
    {
      if (!characterDatabase.ContainsKey(character.characterID))
      {
        characterDatabase.Add(character.characterID, character);
      }
      else
      {
        Debug.LogWarning($"Duplicate character ID: {character.characterID}");
      }
    }
  }

  /// <summary>
  /// Запуск пролога игры.
  /// </summary>
  public void StartPrologue()
  {
    if (prologueChapter == null)
    {
      Debug.LogError("Prologue chapter is not assigned!");
      return;
    }

    if (backgroundImage != null && prologueChapter.background != null)
    {
      backgroundImage.sprite = prologueChapter.background;
    }

    SafeSetActive(leftCharacterSlot, false);
    SafeSetActive(rightCharacterSlot, false);
    SafeSetActive(centerCharacterSlot, false);

    currentLineIndex = 0;
    StartCoroutine(PlayPrologue());
  }

  /// <summary>
  /// Корутина воспроизведения пролога.
  /// </summary>
  IEnumerator PlayPrologue()
  {
    if (prologueChapter.lines == null || prologueChapter.lines.Count == 0)
    {
      Debug.LogError("Prologue has no lines!");
      yield break;
    }

    while (currentLineIndex < prologueChapter.lines.Count)
    {
      currentLine = prologueChapter.lines[currentLineIndex];

      yield return ShowDialogueLine(currentLine);

      waitingForClick = true;
      yield return WaitForClick();

      currentLineIndex++;
    }

    yield return ShowFinalScreen();
  }

  /// <summary>
  /// Ожидание клика пользователя.
  /// </summary>
  IEnumerator WaitForClick()
  {
    yield return new WaitWhile(() => waitingForClick);
  }

  /// <summary>
  /// Показ отдельной реплики диалога.
  /// </summary>
  /// <param name="line">Данные реплики.</param>
  IEnumerator ShowDialogueLine(DialogueLine line)
  {
    if (line == null)
    {
      Debug.LogError("Trying to show null dialogue line!");
      yield break;
    }

    if (line.backgroundChange != null && backgroundImage != null)
    {

      yield return FadeSystem.Instance.FadeBackground(
              backgroundImage,
              line.backgroundChange,
              () => ApplyDialogueLineChanges(line),
              backgroundTransitionDuration
          );
    }
    else
    {
      ApplyDialogueLineChanges(line);
    }
  }

  /// <summary>
  /// Применение изменений для отображения реплики.
  /// </summary>
  /// <param name="line">Данные реплики.</param>
  void ApplyDialogueLineChanges(DialogueLine line)
  {
    SafeSetText(dialogueText, line.text);
    SafeSetActive(leftCharacterSlot, false);
    SafeSetActive(rightCharacterSlot, false);
    SafeSetActive(centerCharacterSlot, false);

    if (!string.IsNullOrEmpty(line.characterID))
    {
      if (characterDatabase.TryGetValue(line.characterID, out CharacterData character))
      {
        if (line.showCharacter)
        {
          SafeSetText(speakerNameText, character.displayName);
          Debug.Log(character.displayName);
          Debug.Log(character);
          switch (line.position)
          {
            case CharacterPosition.Left:
              SafeSetActive(leftCharacterSlot, true);
              SafeSetSprite(leftCharacterSlot, character.sprite);
              break;
            case CharacterPosition.Right:
              SafeSetActive(rightCharacterSlot, true);
              SafeSetSprite(rightCharacterSlot, character.sprite);
              break;
            case CharacterPosition.Center:
              SafeSetActive(centerCharacterSlot, true);
              SafeSetSprite(centerCharacterSlot, character.sprite);
              StartCoroutine(ForceRefreshNextFrame(centerCharacterSlot));
              break;
          }
        }
        else
        {
          SafeSetText(speakerNameText, "...");
        }
      }
      else
      {
        Debug.LogWarning($"Character not found: {line.characterID}");
        SafeSetText(speakerNameText, "???");
      }
    }
    else
    {
      SafeSetText(speakerNameText, "...");
    }
  }

  /// <summary>
  /// Принудительное обновление отображения центрального слота.
  /// </summary>
  /// <param name="img">Слот персонажа.</param>
  IEnumerator ForceRefreshNextFrame(Image img)
  {
    yield return null;
    img.enabled = false;
    img.enabled = true;
  }

  /// <summary>
  /// Показ финального экрана после завершения пролога.
  /// </summary>
  IEnumerator ShowFinalScreen()
  {
    if (finalScreen == null)
    {
      Debug.LogError("Final screen is not assigned!");
      yield break;
    }

    yield return FadeSystem.Instance.FadeOut();

    finalScreenClickHandler.OnClick += OnFinalScreenClicked;
    finalScreen.SetActive(true);

    yield return FadeSystem.Instance.FadeIn();

    waitingForClick = true;
    yield return WaitForClick();

    yield return FadeSystem.Instance.FadeOut();

    finalScreen.SetActive(false);
    finalScreenClickHandler.OnClick -= OnFinalScreenClicked;
    SceneManager.LoadScene(nextSceneName);
  }

  /// <summary>
  /// Обработчик клика по финальному экрану.
  /// </summary>
  void OnFinalScreenClicked()
  {
    if (waitingForClick)
    {
      waitingForClick = false;
    }
  }

  /// <summary>
  /// Безопасная установка текста с проверкой на null.
  /// </summary>
  /// <param name="textComponent">Текстовый компонент.</param>
  /// <param name="value">Строка для установки текста.</param>
  void SafeSetText(Text textComponent, string value)
  {
    if (textComponent != null) textComponent.text = value;
  }

  /// <summary>
  /// Безопасное изменение активности объекта с проверкой на null.
  /// </summary>
  /// <param name="image">Слот персонажа.</param>
  /// <param name="state">Состояние активности объекта.</param>
  void SafeSetActive(Image image, bool state)
  {
    if (image != null) image.gameObject.SetActive(state);
  }

  /// <summary>
  /// Безопасная установка спрайта с проверкой на null.
  /// </summary>
  /// <param name="image">Слот персонажа.</param>
  /// <param name="sprite">Спрайт персонажа.</param>
  void SafeSetSprite(Image image, Sprite sprite)
  {
    if (image != null) image.sprite = sprite;
  }

  #endregion
}