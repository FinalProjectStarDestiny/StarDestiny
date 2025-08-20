using UnityEngine;
using static TMPro.Examples.TMP_ExampleScript_01;

/// <summary>
/// ��������� ��� ������ � ���������.
/// </summary>
[CreateAssetMenu(fileName = "Char_", menuName = "Visual Novel/Character Data")]
public class CharacterData : ScriptableObject
{
    /// <summary>
    /// ���������� ID ���������.
    /// </summary>
    public string characterID;

    /// <summary>
    /// ��� ���������, ������������ �� ������.
    /// </summary>
    public string displayName;

    /// <summary>
    /// ����������� ���������.
    /// </summary>
    public Sprite sprite;

    /// <summary>
    /// ��� �������.
    /// </summary>
    public ObjectType objectType = ObjectType.Character;
}

public enum ObjectType
{
    Character,
    Item
}
