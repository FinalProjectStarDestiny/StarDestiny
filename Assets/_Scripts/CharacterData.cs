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

  /// <summary>
  /// Меняет имя и спрайт для героя
  /// </summary>
  public void ChangeCharacterData()
  {
    Sprite spriteMan = Resources.Load<Sprite>("Sprites/mainMan");
    Sprite spriteWoman = Resources.Load<Sprite>("Sprites/mainWoman");
    this.displayName = PlayerPrefs.GetString("UserName");
    if (string.Equals(PlayerPrefs.GetString("Gender"), "Женский"))
      this.sprite = spriteWoman;
    else
      this.sprite = spriteMan;
  }
}

public enum ObjectType
{
  Character,
  Item
}
