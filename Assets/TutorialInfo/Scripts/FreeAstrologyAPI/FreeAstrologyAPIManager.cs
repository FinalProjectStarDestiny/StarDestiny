using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System;
using Newtonsoft.Json;

/// <summary>
/// Менеджер для работы с API астрологии. 
/// Обеспечивает взаимодействие с внешним API для получения астрологических данных.
/// </summary>
public class FreeAstrologyAPIManager : MonoBehaviour
{
  #region Поля

  /// <summary>
  /// URL-адрес API для получения астрологических данных о домах.
  /// </summary>
  private const string API_URL = "https://json.freeastrologyapi.com/western/houses";

  /// <summary>
  /// Ключ API для аутентификации запросов. 
  /// Необходимо для доступа к функционалу API.
  /// </summary>
  private string apiKey = "POgggZ2PCw1l3DxdPthF35TtcxtgGlNT6VnzzqMI";

  #endregion

  #region Методы

  /// <summary>
  /// Получает знак зодиака в 10 доме из API
  /// </summary>
  public IEnumerator GetTenthHouseSign(Action<string> callback)
  {
    if (!PlayerPrefs.HasKey("BirthDate") || !PlayerPrefs.HasKey("BirthTime") ||
        !PlayerPrefs.HasKey("Latitude") || !PlayerPrefs.HasKey("Longitude") ||
        !PlayerPrefs.HasKey("Timezone"))
    {
      callback?.Invoke(GetRandomZodiacSign());
      yield break;
    }

    DateTime birthDate = DateTime.Parse(PlayerPrefs.GetString("BirthDate"));
    DateTime birthTime = DateTime.Parse(PlayerPrefs.GetString("BirthTime"));
    float latitude = PlayerPrefs.GetFloat("Latitude");
    float longitude = PlayerPrefs.GetFloat("Longitude");
    float timezone = PlayerPrefs.GetFloat("Timezone");

    FreeAstrologyRequest requestData = new FreeAstrologyRequest
    {
      year = birthDate.Year,
      month = birthDate.Month,
      date = birthDate.Day,
      hours = birthTime.Hour,
      minutes = birthTime.Minute,
      seconds = birthTime.Second,
      latitude = latitude,
      longitude = longitude,
      timezone = timezone,
    };

    string jsonData = JsonUtility.ToJson(requestData);
    byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);

    using (UnityWebRequest request = new UnityWebRequest(API_URL, "POST"))
    {
      request.uploadHandler = new UploadHandlerRaw(bodyRaw);
      request.downloadHandler = new DownloadHandlerBuffer();
      request.SetRequestHeader("Content-Type", "application/json");
      request.SetRequestHeader("x-api-key", apiKey);

      yield return request.SendWebRequest();

      if (request.result != UnityWebRequest.Result.Success)
      {
        Debug.LogError($"Ошибка API: {request.error}");
        callback?.Invoke(GetRandomZodiacSign());
        yield break;
      }

      try
      {
        FreeAstrologyResponse responseData = JsonConvert.DeserializeObject<FreeAstrologyResponse>(request.downloadHandler.text);
       
        Debug.Log("API Response: " + request.downloadHandler.text);
        Debug.Log("Parsed response: " + (responseData != null ? "Success" : "Null"));
  
        if (responseData == null)
        {
          Debug.LogError("Не удалось десериализовать ответ API");
          callback?.Invoke(GetRandomZodiacSign());
          yield break;
        }

        if (responseData.output == null)
        {
          Debug.LogError("Ответ API не содержит output данных");
          callback?.Invoke(GetRandomZodiacSign());
          yield break;
        }

        if (responseData.output.Houses != null)
        {
          foreach (HouseData house in responseData.output.Houses)
          {
            if (house.House == 10)
            {
              callback?.Invoke(house.zodiac_sign.name.en);
              yield break;
            }
          }

          Debug.LogError("Не удалось найти 10 дом в ответе API");
          callback?.Invoke(GetRandomZodiacSign());
        }
        else
        {
          Debug.LogError("Ответ API не содержит данных о домах");
          callback?.Invoke(GetRandomZodiacSign());
        }
      }
      catch (System.Exception e)
      {
        Debug.LogError($"Ошибка парсинга ответа: {e.Message}");
        callback?.Invoke(GetRandomZodiacSign());
      }
    }
  }

  /// <summary>
  /// Проверяет, установлен ли API ключ
  /// </summary>
  public bool HasValidApiKey()
  {
    return !string.IsNullOrEmpty(apiKey) && apiKey != "YOUR_API_KEY_HERE";
  }

  /// <summary>
  /// Возвращает случайный знак зодиака (заглушка при ошибках)
  /// </summary>
  private string GetRandomZodiacSign()
  {
    string[] signs = {
            "Овен", "Телец", "Близнецы", "Рак", "Лев", "Дева",
            "Весы", "Скорпион", "Стрелец", "Козерог", "Водолей", "Рыбы"
        };

    return signs[UnityEngine.Random.Range(0, signs.Length)];
  }

  #endregion
}