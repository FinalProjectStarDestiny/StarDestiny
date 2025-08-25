using System;

/// <summary>
/// Представляет запрос к API астрологии, содержащий информацию о времени и местоположении.
/// </summary>
[System.Serializable]
public class FreeAstrologyRequest
{
  /// <summary>
  /// Год, используемый для запроса астрологических данных.
  /// </summary>
  public int year;

  /// <summary>
  /// Месяц, используемый для запроса астрологических данных (от 1 до 12).
  /// </summary>
  public int month;

  /// <summary>
  /// Число, используемое для запроса астрологических данных (от 1 до 31 в зависимости от месяца).
  /// </summary>
  public int date;

  /// <summary>
  /// Часы, используемые для запроса астрологических данных (от 0 до 23).
  /// </summary>
  public int hours;

  /// <summary>
  /// Минуты, используемые для запроса астрологических данных (от 0 до 59).
  /// </summary>
  public int minutes;

  /// <summary>
  /// Секунды, используемые для запроса астрологических данных (от 0 до 59).
  /// </summary>
  public int seconds;

  /// <summary>
  /// Широта местоположения для астрологического расчета.
  /// </summary>
  public float latitude;

  /// <summary>
  /// Долгота местоположения для астрологического расчета.
  /// </summary>
  public float longitude;

  /// <summary>
  /// Часовой пояс, используемый для расчета (от -12 до +14, с возможностью 2,5 (полчаса)).
  public float timezone;
}