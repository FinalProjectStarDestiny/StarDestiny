using System;

/// <summary>
/// Представляет корневой объект ответа от API, включая код состояния и выходные данные.
/// </summary>
[Serializable]
public class Root
{
  /// <summary>
  /// Код состояния ответа
  /// </summary>
  public int statusCode { get; set; }

  /// <summary>
  /// Объект, содержащий выходные данные, включая информацию о домах.
  /// </summary>
  public Output output { get; set; }
}
