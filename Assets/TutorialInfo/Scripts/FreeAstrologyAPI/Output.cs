using System.Collections.Generic;
using System;

[Serializable]
/// <summary>
/// Представляет основной объект ответа, содержащий информацию о домах.
/// </summary>
public class Output
{
  /// <summary>
  /// Список данных о домах, содержащий информацию о каждом доме.
  /// </summary>
  public List<HouseData> Houses { get; set; }
}