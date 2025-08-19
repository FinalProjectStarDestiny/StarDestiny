using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class DialogueSystem : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Image backgroundImage;
    [SerializeField] private Text speakerNameText;
    [SerializeField] private Text dialogueText;
    [SerializeField] private GameObject dialoguePanel;

    [Header("Character Slots")]
    [SerializeField] private Image leftCharacterSlot;
    [SerializeField] private Image rightCharacterSlot;
    [SerializeField] private Image centerCharacterSlot;

    [Header("Prologue Settings")]
    [SerializeField] private ChapterData prologueChapter;
    [SerializeField] private GameObject contentPanel; // Панель с фоном и персонажами

    private Dictionary<string, CharacterData> characterDatabase;
    private int currentLineIndex = 0;
    private bool waitingForClick = false;

    private void Start()
    {
        // Добавляем обработчик клика на панель контента
        if (contentPanel != null)
        {
            // Убедимся, что панель может получать клики
            Image panelImage = contentPanel.GetComponent<Image>();
            if (panelImage == null)
            {
                panelImage = contentPanel.AddComponent<Image>();
            }

            // Добавляем обработчик кликов
            ContentPanelClickHandler clickHandler = contentPanel.GetComponent<ContentPanelClickHandler>();
            if (clickHandler == null)
            {
                clickHandler = contentPanel.AddComponent<ContentPanelClickHandler>();
            }
            clickHandler.OnClick += OnContentPanelClicked;
        }

        // Инициализируем базу данных персонажей
        characterDatabase = new Dictionary<string, CharacterData>();
        LoadCharacters();

        // Запускаем пролог
        StartPrologue();
    }

    private void OnDestroy()
    {
        // Отписываемся от события при уничтожении объекта
        if (contentPanel != null)
        {
            ContentPanelClickHandler clickHandler = contentPanel.GetComponent<ContentPanelClickHandler>();
            if (clickHandler != null)
            {
                clickHandler.OnClick -= OnContentPanelClicked;
            }
        }
    }

    // Обработчик клика по панели контента
    private void OnContentPanelClicked()
    {
        if (waitingForClick)
        {
            waitingForClick = false; // Сбрасываем флаг ожидания
        }
    }

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

    public void StartPrologue()
    {
        if (prologueChapter == null)
        {
            Debug.LogError("Prologue chapter is not assigned!");
            return;
        }

        Debug.Log($"Starting prologue: {prologueChapter.name}");

        // Устанавливаем фон
        if (backgroundImage != null && prologueChapter.background != null)
        {
            backgroundImage.sprite = prologueChapter.background;
        }

        // Скрываем всех персонажей
        SafeSetActive(leftCharacterSlot, false);
        SafeSetActive(rightCharacterSlot, false);
        SafeSetActive(centerCharacterSlot, false);

        // Запускаем воспроизведение пролога
        currentLineIndex = 0;
        StartCoroutine(PlayPrologue());
    }

    IEnumerator PlayPrologue()
    {
        if (prologueChapter.lines == null || prologueChapter.lines.Count == 0)
        {
            Debug.LogError("Prologue has no lines!");
            yield break;
        }

        // Проходим через все реплики пролога
        while (currentLineIndex < prologueChapter.lines.Count)
        {
            // Показываем текущую реплику
            ShowDialogueLine(prologueChapter.lines[currentLineIndex]);

            // Ждем клика по панели
            waitingForClick = true;
            yield return WaitForClick();

            currentLineIndex++;
        }

        // Пролог завершен
        Debug.Log("Prologue completed!");

        // Затемнение и переход на следующую сцену
        yield return FadeSystem.Instance.FadeOut();
        SceneManager.LoadScene("NextScene");
    }

    // Ожидание клика через флаг
    IEnumerator WaitForClick()
    {
        // Ждем, пока флаг waitingForClick не станет false
        yield return new WaitWhile(() => waitingForClick);
    }

    void ShowDialogueLine(DialogueLine line)
    {
        if (line == null)
        {
            Debug.LogError("Trying to show null dialogue line!");
            return;
        }

        // Смена фона
        if (line.backgroundChange != null && backgroundImage != null)
        {
            backgroundImage.sprite = line.backgroundChange;
            StartCoroutine(FadeBackground(0.5f));
        }

        // Устанавливаем текст реплики
        SafeSetText(dialogueText, line.text);

        // Сначала скрываем всех персонажей
        SafeSetActive(leftCharacterSlot, false);
        SafeSetActive(rightCharacterSlot, false);
        SafeSetActive(centerCharacterSlot, false);

        // Показываем персонажа, если нужно
        if (!string.IsNullOrEmpty(line.characterID) && line.showCharacter)
        {
            if (characterDatabase.TryGetValue(line.characterID, out CharacterData character))
            {
                SafeSetText(speakerNameText, character.displayName);

                // Показываем персонажа в нужной позиции
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
                Debug.LogWarning($"Character not found: {line.characterID}");
                SafeSetText(speakerNameText, "???");
            }
        }
        else
        {
            SafeSetText(speakerNameText, "");
        }
    }

    IEnumerator ForceRefreshNextFrame(Image img)
    {
        yield return null;
        img.enabled = false;
        img.enabled = true;
    }

    // Корутина для плавной смены фона
    private IEnumerator FadeBackground(float duration)
    {
        if (backgroundImage == null) yield break;

        GameObject fadePanel = new GameObject("FadePanel");
        fadePanel.transform.SetParent(backgroundImage.transform.parent);
        fadePanel.transform.SetAsFirstSibling();

        Image fadeImage = fadePanel.AddComponent<Image>();
        fadeImage.color = Color.black;

        RectTransform rt = fadePanel.GetComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.sizeDelta = Vector2.zero;

        // Затемнение
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            fadeImage.color = new Color(0, 0, 0, Mathf.Lerp(0, 1, elapsed / duration));
            yield return null;
        }

        // Осветление
        elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            fadeImage.color = new Color(0, 0, 0, Mathf.Lerp(1, 0, elapsed / duration));
            yield return null;
        }

        Destroy(fadePanel);
    }

    // Вспомогательные методы
    void SafeSetText(Text textComponent, string value)
    {
        if (textComponent != null) textComponent.text = value;
    }

    void SafeSetActive(Image image, bool state)
    {
        if (image != null) image.gameObject.SetActive(state);
    }

    void SafeSetSprite(Image image, Sprite sprite)
    {
        if (image != null) image.sprite = sprite;
    }
}

// Класс для обработки кликов по панели контента
public class ContentPanelClickHandler : MonoBehaviour, IPointerClickHandler
{
    public event System.Action OnClick;

    public void OnPointerClick(PointerEventData eventData)
    {
        OnClick?.Invoke();
    }
}