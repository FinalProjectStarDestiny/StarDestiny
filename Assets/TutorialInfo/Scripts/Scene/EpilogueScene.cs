using UnityEngine;

/// <summary>
/// Система диалогов, обрабатывающая показ реплик, персонажей и фонов для эпилога.
/// </summary>
public class EpilogueScene: BaseChapterScene
{
  protected override void Start()
  {
    base.Start();
    mainCharacterData.ChangeCharacterData(true);
  }
}