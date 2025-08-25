using System;

/// <summary>
/// Представляет данные предсказаний таро, включая аспекты жизни человека.
/// </summary>
[System.Serializable]
public class TarotPredictionData
{
  /// <summary>
  /// Прогноз состояния здоровья.
  /// </summary>
  public string health;

  /// <summary>
  /// Прогноз состояния в отношениях.
  /// </summary>
  public string relationship;

  /// <summary>
  /// Прогноз карьеры.
  /// </summary>
  public string career;

  /// <summary>
  /// Прогноз финансового состояния.
  /// </summary>
  public string finance;

  /// <summary>
  /// Идентификатор предсказания.
  /// </summary>
  public string id;

  /// <summary>
  /// Имя или название предсказания.
  /// </summary>
  public string name;

  /// <summary>
  /// Объект, содержащий изображения карт.
  /// </summary>
  public CardImages card_image;

  /// <summary>
  /// Объект, содержащий обратные изображения карт.
  /// </summary>
  public CardImagesBack card_images_back;
}
