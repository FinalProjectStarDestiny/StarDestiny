using System.Collections.Generic;
using System;

[Serializable]
/// <summary>
/// ������������ �������� ������ ������, ���������� ���������� � �����.
/// </summary>
public class Output
{
  /// <summary>
  /// ������ ������ � �����, ���������� ���������� � ������ ����.
  /// </summary>
  public List<HouseData> Houses { get; set; }
}