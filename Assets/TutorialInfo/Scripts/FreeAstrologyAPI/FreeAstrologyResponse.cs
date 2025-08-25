using System;

/// <summary>
/// Представляет ответ от API астрологии, включая выходные данные и код состояния.
/// </summary>
[System.Serializable]
public class FreeAstrologyResponse
{
  /// <summary>
  /// Объект, содержащий выходные данные ответа, включая информацию о гороскопах или астрологических данных.
  /// </summary>
  public Output output;

  /// <summary>
  /// Код состояния ответа
  /// </summary>
  public int StatusCode;
}