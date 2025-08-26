using System;

/// <summary>
/// ������������ ������ ������������ ����, ������� ������� ����� ��������.
/// </summary>
[System.Serializable]
public class TarotPredictionData
{
  /// <summary>
  /// ������� ��������� ��������.
  /// </summary>
  public string health;

  /// <summary>
  /// ������� ��������� � ����������.
  /// </summary>
  public string relationship;

  /// <summary>
  /// ������� �������.
  /// </summary>
  public string career;

  /// <summary>
  /// ������� ����������� ���������.
  /// </summary>
  public string finance;

  /// <summary>
  /// ������������� ������������.
  /// </summary>
  public string id;

  /// <summary>
  /// ��� ��� �������� ������������.
  /// </summary>
  public string name;

  /// <summary>
  /// ������, ���������� ����������� ����.
  /// </summary>
  public CardImages card_image;

  /// <summary>
  /// ������, ���������� �������� ����������� ����.
  /// </summary>
  public CardImagesBack card_images_back;
}
