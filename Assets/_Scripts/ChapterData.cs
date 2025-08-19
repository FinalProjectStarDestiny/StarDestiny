using UnityEngine;
using System.Collections.Generic;

// Хранит данные всей главы (пролога)
[CreateAssetMenu(fileName = "Chapter_", menuName = "Visual Novel/Chapter Data")]
public class ChapterData : ScriptableObject
{
    public Sprite background;   // Фон для всей главы
    public List<DialogueLine> lines = new List<DialogueLine>(); // Все реплики главы
}