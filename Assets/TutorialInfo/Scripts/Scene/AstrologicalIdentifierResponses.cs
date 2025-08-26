using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Представляет ответ знак зодиака или ЧЖП, содержащий знак зодиака или ЧЖП и связанные с ним предложения.
/// </summary>
[System.Serializable]
public class AstrologicalIdentifierResponse
{
  /// <summary>
  /// Знак зодиака или ЧЖП, к которому относится этот ответ.
  /// </summary>
  public string astrologicalIdentifier;

  /// <summary>
  /// Список предложений, связанных с данным ключом.
  /// </summary>
  public List<string> sentences;
}

/// <summary>
/// Скриптовый объект для хранения ответов зодиака или ЧЖП.
/// Позволяет создавать список ответов для различных знаков зодиака или ЧЖП.
/// </summary>
[CreateAssetMenu(fileName = "AstrologicalIdentifierResponses", menuName = "Dialogue/Astrological Identifier Responses")]
public class AstrologicalIdentifierResponses : ScriptableObject
{
  /// <summary>
  /// Список ответов зодиака, каждый из которых содержит знак и соответствующие предложения.
  /// </summary>
  public List<AstrologicalIdentifierResponse> responses;
}
