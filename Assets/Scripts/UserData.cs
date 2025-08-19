using System;

/// <summary>
/// ����� ��� �������� ���������������� ������
/// </summary>
[System.Serializable]
public class UserData
{
  /// <summary>��� ������������</summary>
  public string Name;

  /// <summary>��� ������������</summary>
  public string Gender;

  /// <summary>���� ��������</summary>
  public DateTime BirthDate;

  /// <summary>����� ��������</summary>
  public DateTime BirthTime;

  /// <summary>����� ��������</summary>
  public string BirthCity;

  /// <summary>�������������� ������ ������ ��������</summary>
  public float Latitude;

  /// <summary>�������������� ������� ������ ��������</summary>
  public float Longitude;

  public override string ToString()
  {
    return $"���: {Name}\n" +
           $"���: {Gender}\n" +
           $"���� ��������: {BirthDate.ToShortDateString()}\n" +
           $"����� ��������: {BirthTime.ToShortTimeString()}\n" +
           $"�����: {BirthCity}\n" +
           $"����������: {Latitude}, {Longitude}";
  }
}