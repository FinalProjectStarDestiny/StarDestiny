using UnityEngine;

public class NewEmptyCSharpScript : BaseChapterScene
{
  protected override void Start()
  {
    base.Start();
    mainCharacterData.ChangeCharacterData(true);
  }
}
