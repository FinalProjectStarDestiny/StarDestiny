using UnityEngine;

/// <summary>
/// Структура данных для одной реплики диалога.
/// </summary>
[System.Serializable]
public class DialogueLine
{
    /// <summary>
    /// Уникальный ID персонажа.
    /// </summary>
    public string characterID;

    /// <summary>
    /// Текст реплики.
    /// </summary>
    [TextArea(3, 5)] public string text;

    /// <summary>
    /// Позиция персонажа на экране.
    /// </summary>
    public CharacterPosition position = CharacterPosition.Left;

    /// <summary>
    /// Опциональная смена фона при этой реплике.
    /// </summary>
    public Sprite backgroundChange;

    /// <summary>
    /// Состояние отображения персонажа.
    /// </summary>
    public bool showCharacter = true;
}

public enum CharacterPosition
{
    Left,
    Right,
    Center,
    None
}