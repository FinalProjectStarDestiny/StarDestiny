using UnityEngine;

/// <summary>
/// ������� ��������, �������������� ����� ������, ���������� � ����� ��� �������.
/// </summary>
public class EpilogueScene: BaseChapterScene
{
  protected override void Start()
  {
    base.Start();
    mainCharacterData.ChangeCharacterData(true);
  }
}