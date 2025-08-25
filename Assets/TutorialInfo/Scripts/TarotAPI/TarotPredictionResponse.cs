using System;

/// <summary>
/// Представляет ответ от Vedic Astrology API для предсказания таро.
/// </summary>
[System.Serializable]
public class TarotPredictionResponse
{
  /// <summary>
  /// Код состояния ответа API (например, 200 для успешного запроса, 400 для ошибки и т.д.).
  /// </summary>
  public int status;

  /// <summary>
  /// Объект, содержащий данные о предсказании таро.
  /// </summary>
  public TarotPredictionData response;

  /// <summary>
  /// Количество оставшихся вызовов API, доступных в текущий период.
  /// </summary>
  public int remaining_api_calls;
}
