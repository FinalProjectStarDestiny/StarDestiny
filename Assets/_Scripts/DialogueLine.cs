using UnityEngine;

// Описывает одну реплику в диалоге
[System.Serializable]
public class DialogueLine
{
    public string characterID;
    [TextArea(3, 5)] public string text;
    public CharacterPosition position = CharacterPosition.Left;
    public Sprite backgroundChange; // Новое поле для смены фона
    public bool showCharacter = true;
}

public enum CharacterPosition
{
    Left,
    Right,
    Center,
    None
}