using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Контейнер для данных пролога.
/// </summary>
[CreateAssetMenu(fileName = "Chapter_", menuName = "Visual Novel/Chapter Data")]
public class ChapterData : ScriptableObject
{
  /// <summary>
  /// Изначальный фон главы.
  /// </summary>
  public Sprite background;

  /// <summary>
  /// Cписок реплик диалога.
  /// </summary>
  public List<DialogueLine> lines = new List<DialogueLine>(); // Все реплики главы
}