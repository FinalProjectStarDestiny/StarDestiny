using System;

/// <summary>
/// ������������ �������� ������ ������ �� API, ������� ��� ��������� � �������� ������.
/// </summary>
[Serializable]
public class Root
{
  /// <summary>
  /// ��� ��������� ������
  /// </summary>
  public int statusCode { get; set; }

  /// <summary>
  /// ������, ���������� �������� ������, ������� ���������� � �����.
  /// </summary>
  public Output output { get; set; }
}
