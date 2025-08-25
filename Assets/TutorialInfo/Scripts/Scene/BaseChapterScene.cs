using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using Assets.TutorialInfo.Scripts;

/// <summary>
/// Базовый класс для всех сцен с диалогами, содержащий общую функциональность.
/// </summary>
public class BaseChapterScene : MonoBehaviour
{
  #region Общие поля

  [Header("UI References")]

  /// <summary>
  /// Изображение фона, отображаемое на панели диалога.
  /// </summary>
  [SerializeField] protected Image backgroundImage;

  /// <summary>
  /// Текстовое поле, отображающее имя говорящего персонажа.
  /// </summary>
  [SerializeField] protected Text speakerNameText;

  /// <summary>
  /// Текстовое поле, отображающее текущий диалог.
  /// </summary>
  [SerializeField] protected Text dialogueText;

  /// <summary>
  /// Панель, содержащая элементы UI для диалога.
  /// </summary>
  [SerializeField] protected GameObject dialoguePanel;

  [Header("Character Slots")]

  /// <summary>
  /// Слот для изображения персонажа слева.
  /// </summary>
  [SerializeField] protected Image leftCharacterSlot;

  /// <summary>
  /// Слот для изображения персонажа справа.
  /// </summary>
  [SerializeField] protected Image rightCharacterSlot;

  /// <summary>
  /// Слот для изображения центрального персонажа.
  /// </summary>
  [SerializeField] protected Image centerCharacterSlot;

  [Header("Scene Settings")]

  /// <summary>
  /// Данные по главе, используемые для управления прогрессом истории.
  /// </summary>
  [SerializeField] protected ChapterData chapterData;

  /// <summary>
  /// Панель контента, содержащая все элементы UI, связанные с диалогами.
  /// </summary>
  [SerializeField] protected GameObject contentPanel;

  [Header("Final Screen Settings")]

  /// <summary>
  /// Финальная экранная панель, отображаемая по завершении диалога.
  /// </summary>
  [SerializeField] protected GameObject finalScreen;

  /// <summary>
  /// Имя следующей сцены, к которой будет осуществлён переход.
  /// </summary>
  [SerializeField] protected string nextSceneName;

  /// <summary>
  /// Текст, отображаемый на финальном экране.
  /// </summary>
  [SerializeField] protected string finalScreenText;

  [Header("Main Character")]

  /// <summary>
  /// Данные основного персонажа, используемого в диалогах.
  /// </summary>
  [SerializeField] protected CharacterData mainCharacterData;

  /// <summary>
  /// База данных персонажей, хранящая информацию о всех персонажах в игре.
  /// </summary>
  protected Dictionary<string, CharacterData> characterDatabase;

  /// <summary>
  /// Индекс текущей линии диалога для отображения.
  /// </summary>
  protected int currentLineIndex = 0;

  /// <summary>
  /// Флаг, указывающий, ожидается ли клик для продолжения диалога.
  /// </summary>
  protected bool waitingForClick = false;

  /// <summary>
  /// Текущая линия диалога, отображаемая на экране.
  /// </summary>
  protected DialogueLine currentLine;

  /// <summary>
  /// Обработчик кликов для финального экрана.
  /// </summary>
  protected ContentPanelClickHandler finalScreenClickHandler;

  #endregion

  #region Общие методы

  /// <summary>
  /// Инициализация сцены.
  /// </summary>
  protected virtual void Start()
  {
    contentPanel.SetActive(true);
    if (finalScreen != null) finalScreen.SetActive(false);

    SetupContentPanel();
    SetupFinalScreen();

    if (mainCharacterData != null)
    {
      mainCharacterData.ChangeCharacterData();
    }

    if (SceneTransitionManager.Instance == null)
    {
      GameObject transitionManagerObj = new GameObject("SceneTransitionManager");
      transitionManagerObj.AddComponent<SceneTransitionManager>();
      DontDestroyOnLoad(transitionManagerObj);
    }

    characterDatabase = new Dictionary<string, CharacterData>();
    LoadCharacters();
    StartChapter();
  }

  /// <summary>
  /// Настройка панели контента.
  /// </summary>
  private void SetupContentPanel()
  {
    if (contentPanel != null)
    {
      ContentPanelClickHandler clickHandler = contentPanel.GetComponent<ContentPanelClickHandler>();
      if (clickHandler == null)
      {
        clickHandler = contentPanel.AddComponent<ContentPanelClickHandler>();
      }
      clickHandler.OnClick += OnContentPanelClicked;
    }
  }

  /// <summary>
  /// Настройка финального экрана.
  /// </summary>
  private void SetupFinalScreen()
  {
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
    }
  }

  /// <summary>
  /// Загрузка данных персонажей из ресурсов.
  /// </summary>
  protected void LoadCharacters()
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
  /// Запуск главы.
  /// </summary>
  protected void StartChapter()
  {
    if (chapterData == null)
    {
      Debug.LogError("Chapter data is not assigned!");
      return;
    }

    if (backgroundImage != null && chapterData.background != null)
    {
      backgroundImage.sprite = chapterData.background;
    }

    SafeSetActive(leftCharacterSlot, false);
    SafeSetActive(rightCharacterSlot, false);
    SafeSetActive(centerCharacterSlot, false);

    currentLineIndex = 0;
    StartCoroutine(PlayChapter());
  }

  /// <summary>
  /// Корутина воспроизведения главы.
  /// </summary>
  protected virtual IEnumerator PlayChapter()
  {
    if (chapterData.lines == null || chapterData.lines.Count == 0)
    {
      Debug.LogError("Chapter has no lines!");
      yield break;
    }

    while (currentLineIndex < chapterData.lines.Count)
    {
      currentLine = chapterData.lines[currentLineIndex];
      yield return ShowDialogueLine(currentLine);

      waitingForClick = true;
      yield return WaitForClick();

      currentLineIndex++;
    }

    yield return ShowFinalScreen();
  }

  /// <summary>
  /// Показ отдельной реплики диалога.
  /// </summary>
  /// <param name="line">Данные реплики.</param>
  /// <returns></returns>
  protected IEnumerator ShowDialogueLine(DialogueLine line)
  {
    if (line == null)
    {
      Debug.LogError("Trying to show null dialogue line!");
      yield break;
    }

    if (line.backgroundChange != null && backgroundImage != null)
    {
      yield return SceneTransitionManager.Instance.FadeBackground(
          backgroundImage,
          line.backgroundChange,
          () => ApplyDialogueLineChanges(line)
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
  protected void ApplyDialogueLineChanges(DialogueLine line)
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
  /// Показ финального экрана.
  /// </summary>
  protected IEnumerator ShowFinalScreen()
  {
    if (finalScreen == null)
    {
      Debug.LogError("Final screen is not assigned!");
      yield break;
    }

    yield return SceneTransitionManager.Instance.FadeOut();

    finalScreenClickHandler.OnClick += OnFinalScreenClicked;
    finalScreen.SetActive(true);

    yield return SceneTransitionManager.Instance.FadeIn();

    waitingForClick = true;
    yield return WaitForClick();

    yield return SceneTransitionManager.Instance.FadeOut();

    finalScreen.SetActive(false);
    finalScreenClickHandler.OnClick -= OnFinalScreenClicked;
    SceneManager.LoadScene(nextSceneName);
  }

  /// <summary>
  /// Обработчик клика по панели контента.
  /// </summary>
  protected void OnContentPanelClicked()
  {
    if (waitingForClick)
    {
      waitingForClick = false;
    }
  }

  /// <summary>
  /// Обработчик клика по финальному экрану.
  /// </summary>
  protected void OnFinalScreenClicked()
  {
    if (waitingForClick)
    {
      waitingForClick = false;
    }
  }

  #endregion

  #region Вспомогательные методы

  /// <summary>
  /// Безопасно устанавливает текст в UI-компонент с проверкой на null ссылку.
  /// </summary>
  /// <param name="textComponent">Компонент Text для установки текста.</param>
  /// <param name="value">Текстовая строка для отображения.</param>
  protected void SafeSetText(Text textComponent, string value)
  {
    if (textComponent != null) textComponent.text = value;
  }

  /// <summary>
  /// Безопасно изменяет активность GameObject компонента Image с проверкой на null ссылку.
  /// </summary>
  /// <param name="image">Компонент Image, активность которого нужно изменить.</param>
  /// <param name="state">Целевое состояние активности: true - активировать, false - деактивировать.</param>
  protected void SafeSetActive(Image image, bool state)
  {
    if (image != null) image.gameObject.SetActive(state);
  }

  /// <summary>
  /// Безопасно устанавливает спрайт в компонент Image с проверкой на null ссылку.
  /// </summary>
  /// <param name="image">Компонент Image для установки спрайта.</param>
  /// <param name="sprite">Спрайт для отображения в компоненте.</param>
  protected void SafeSetSprite(Image image, Sprite sprite)
  {
    if (image != null) image.sprite = sprite;
  }

  /// <summary>
  /// Принудительно обновляет отображение компонента Image в следующем кадре.
  /// </summary>
  /// <param name="img">Компонент Image, который нужно визуально обновить.</param>
  /// <returns></returns>
  protected IEnumerator ForceRefreshNextFrame(Image img)
  {
    yield return null;
    img.enabled = false;
    img.enabled = true;
  }

  /// <summary>
  /// Ожидает клика пользователя по сцене.
  /// </summary>
  /// <returns></returns>
  protected IEnumerator WaitForClick()
  {
    yield return new WaitWhile(() => waitingForClick);
  }

  #endregion

  #region Очистка ресурсов

  /// <summary>
  /// Выполняет очистку ресурсов и отписывается от событий при уничтожении объекта.
  /// </summary>
  protected virtual void OnDestroy()
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

  #endregion
}