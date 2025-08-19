using System;

/// <summary>
/// Класс для хранения пользовательских данных
/// </summary>
[System.Serializable]
public class UserData
{
  /// <summary>Имя пользователя</summary>
  public string Name;

  /// <summary>Пол пользователя</summary>
  public string Gender;

  /// <summary>Дата рождения</summary>
  public DateTime BirthDate;

  /// <summary>Время рождения</summary>
  public DateTime BirthTime;

  /// <summary>Город рождения</summary>
  public string BirthCity;

  /// <summary>Географическая широта города рождения</summary>
  public float Latitude;

  /// <summary>Географическая долгота города рождения</summary>
  public float Longitude;

  public override string ToString()
  {
    return $"Имя: {Name}\n" +
           $"Пол: {Gender}\n" +
           $"Дата рождения: {BirthDate.ToShortDateString()}\n" +
           $"Время рождения: {BirthTime.ToShortTimeString()}\n" +
           $"Город: {BirthCity}\n" +
           $"Координаты: {Latitude}, {Longitude}";
  }
}