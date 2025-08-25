using System;

/// <summary>
/// Представляет запрос к API для получения ежедневного таро.
/// </summary>
[System.Serializable]
public class DailyTarotRequest
{
  /// <summary>
  /// Ключ API для аутентификации запроса. 
  /// Необходим для доступа к функционалу API.
  /// </summary>
  public string api_key;

  /// <summary>
  /// Язык, на котором будет возвращён ответ. 
  /// Например, "en" для английского, "ru" для русского и т.д.
  /// </summary>
  public string lang;
}