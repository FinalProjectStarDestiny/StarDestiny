using UnityEngine;
using static TMPro.Examples.TMP_ExampleScript_01;

// ������ ������ ��������� (������ ID, ��� � ������)
[CreateAssetMenu(fileName = "Char_", menuName = "Visual Novel/Character Data")]
public class CharacterData : ScriptableObject
{
    public string characterID;  // ���������� ������������� ��������� (�������� "hero")
    public string displayName;  // ��� ��� ����������� � ����
    public Sprite sprite;       // ������ ��������� (���� ����������� �� ��� ������)
    public ObjectType objectType = ObjectType.Character;
}

public enum ObjectType
{
    Character,
    Item
}
