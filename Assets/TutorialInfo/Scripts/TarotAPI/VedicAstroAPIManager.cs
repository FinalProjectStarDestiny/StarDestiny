using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System;
using Newtonsoft.Json;

/// <summary>
/// Менеджер для работы с Vedic Astrology API.
/// Обеспечивает взаимодействие с внешним API для получения ежедневных данных о таро.
/// </summary>
public class VedicAstroAPIManager : MonoBehaviour
{
  /// <summary>
  /// URL-адрес API для получения ежедневных таро.
  /// </summary>
  private const string API_URL = "https://api.vedicastroapi.com/v3-json/tarot/daily";

  /// <summary>
  /// Ключ API для аутентификации запросов. 
  /// Необходим для доступа к функционалу API.
  /// </summary>
  private string apiKey = "9964ad6c-5175-5aff-8a88-cb624cf67846";

  [Header("Debug")]
  public bool debugMode = true;

  /// <summary>
  /// Получает ежедневный прогноз Таро
  /// </summary>
  public IEnumerator GetDailyTarotPrediction(Action<TarotPredictionData, string> callback)
  {
    string url = API_URL + "?api_key=" + apiKey + "&lang=ru";

    if (debugMode) Debug.Log($"Запрос к API: {url}");

    UnityWebRequest request = UnityWebRequest.Get(url);
    yield return request.SendWebRequest();

    if (request.result != UnityWebRequest.Result.Success)
    {
      string errorMessage = $"Ошибка API: {request.error}\nКод ответа: {request.responseCode}\nURL: {API_URL}";
      
      if (debugMode) 
        Debug.LogError(errorMessage);

      string responseText = request.downloadHandler.text;
      
      if (!string.IsNullOrEmpty(responseText))
      {
        errorMessage += $"\nОтвет сервера: {responseText}";
      }

      callback?.Invoke(null, errorMessage);
      yield break;
    }

    try
    {
      string responseText = request.downloadHandler.text;
      if (debugMode) Debug.Log($"Ответ API: {responseText}");

      TarotPredictionResponse responseData = JsonConvert.DeserializeObject<TarotPredictionResponse>(responseText);

      if (responseData.status != 200)
      {
        string errorMessage = $"Ошибка API: статус {responseData.status}";
        if (debugMode) Debug.LogError(errorMessage);

        callback?.Invoke(null, $"Ошибка: статус {responseData.status}");
        yield break;
      }

      if (debugMode) Debug.Log("Успешный ответ от API");
      callback?.Invoke(responseData.response, null);
    }
    catch (System.Exception e)
    {
      string errorMessage = $"Ошибка парсинга ответа: {e.Message}";
      if (debugMode) Debug.LogError(errorMessage);
      callback?.Invoke(null, errorMessage);
    }
  }

  /// <summary>
  /// Загружает изображение карты по URL
  /// </summary>
  public IEnumerator LoadCardImage(string imageUrl, Action<Texture2D> callback)
  {
    using (UnityWebRequest imageRequest = UnityWebRequestTexture.GetTexture(imageUrl))
    {
      yield return imageRequest.SendWebRequest();

      if (imageRequest.result != UnityWebRequest.Result.Success)
      {
        Debug.LogError($"Ошибка загрузки изображения: {imageRequest.error}");
        callback?.Invoke(null);
      }
      else
      {
        Texture2D texture = DownloadHandlerTexture.GetContent(imageRequest);
        callback?.Invoke(texture);
      }
    }
  }

  /// <summary>
  /// Проверяет, установлен ли API ключ
  /// </summary>
  public bool HasValidApiKey()
  {
    return !string.IsNullOrEmpty(apiKey) && apiKey != "YOUR_API_KEY";
  }
}