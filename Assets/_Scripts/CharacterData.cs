using UnityEngine;
using static TMPro.Examples.TMP_ExampleScript_01;

// Хранит данные персонажа (только ID, имя и спрайт)
[CreateAssetMenu(fileName = "Char_", menuName = "Visual Novel/Character Data")]
public class CharacterData : ScriptableObject
{
    public string characterID;  // Уникальный идентификатор персонажа (например "hero")
    public string displayName;  // Имя для отображения в игре
    public Sprite sprite;       // Спрайт персонажа (одно изображение на все случаи)
    public ObjectType objectType = ObjectType.Character;
}

public enum ObjectType
{
    Character,
    Item
}
