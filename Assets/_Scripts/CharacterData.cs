using UnityEngine;
using static TMPro.Examples.TMP_ExampleScript_01;

/// <summary>
/// Контейнер для данных о персонаже.
/// </summary>
[CreateAssetMenu(fileName = "Char_", menuName = "Visual Novel/Character Data")]
public class CharacterData : ScriptableObject
{
    /// <summary>
    /// Уникальный ID персонажа.
    /// </summary>
    public string characterID;

    /// <summary>
    /// Имя персонажа, отображаемое на экране.
    /// </summary>
    public string displayName;

    /// <summary>
    /// Изображение персонажа.
    /// </summary>
    public Sprite sprite;

    /// <summary>
    /// Тип объекта.
    /// </summary>
    public ObjectType objectType = ObjectType.Character;
}

public enum ObjectType
{
    Character,
    Item
}
