using System;
using System.Collections.Generic;

/// <summary>
/// Представляет респонс с информацией о доме, градусах и знаке зодиака.
/// </summary>
[Serializable]
public class HouseData
{
  /// <summary>
  /// Номер дома.
  /// </summary>
  public int House;

  /// <summary>
  /// Градус.
  /// </summary>
  public double degree;

  /// <summary>
  /// Нормализованный градус.
  /// </summary>
  public double normDegree;

  /// <summary>
  /// Знак зодиака.
  /// </summary>
  public ZodiacSign zodiac_sign;
}