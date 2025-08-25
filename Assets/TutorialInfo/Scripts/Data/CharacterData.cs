using System.ComponentModel;
using UnityEngine;
//using static TMPro.Examples.TMP_ExampleScript_01;

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

  /// <summary>
  /// ������ ��� � ������ ��� �����
  /// </summary>
  public void ChangeCharacterData(bool isMainGlad = false)
  {
    Sprite spriteMan = Resources.Load<Sprite>("Sprites/mainMan");
    Sprite spriteWoman = Resources.Load<Sprite>("Sprites/mainWoman");
    Sprite spriteManGlad = Resources.Load<Sprite>("Sprites/mainManGlad");
    Sprite spriteWomanGlad = Resources.Load<Sprite>("Sprites/mainWomanGlad");
    this.displayName = PlayerPrefs.GetString("UserName");
    if (string.Equals(PlayerPrefs.GetString("Gender"), "�������"))
      if (isMainGlad)
        this.sprite = spriteWomanGlad;
      else
        this.sprite = spriteWoman;
    else
      if (isMainGlad)
      this.sprite = spriteManGlad;
    else
      this.sprite = spriteMan;
  }
}

/// <summary>
/// ������������, �������������� ��������� ���� ������� �� ������� ������.
/// </summary>
public enum ObjectType
{
  Character,
  Item
}
