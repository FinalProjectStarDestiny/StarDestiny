using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Text.RegularExpressions;
using System.Globalization;
using System.Xml;
using UnityEngine.SceneManagement;
using UnityEngine.TextCore.Text;
using Assets.TutorialInfo.Scripts;

namespace UnityEngine
{
  /// <summary>
  /// Основной класс для сбора и обработки пользовательских данных
  /// </summary>
  public class DataCollector : MonoBehaviour
  {
    #region Константы

    /// <summary>Имя для доступа к GeoNames API</summary>
    private const string GEONAMES_USERNAME = "StarDestiny";

    #endregion

    #region Вложенные типы

    /// <summary>Класс для десериализации ответа от GeoNames API</summary>
    [System.Serializable]
    private class GeoNamesResponse
    {
      /// <summary>Список найденных городов</summary>
      public List<CityData> geonames;
    }

    /// <summary>Класс для хранения данных о городе</summary>
    [System.Serializable]
    private class CityData
    {
      /// <summary>Название города</summary>
      public string name;

      /// <summary>Название страны</summary>
      public string countryName;

      /// <summary>Код страны</summary>
      public string countryCode;

      /// <summary>Географическая широта</summary>
      public float lat;

      /// <summary>Географическая долгота</summary>
      public float lng;
    }

    #endregion

    #region Поля и свойства

    [Header("UI Panels")]

    /// <summary>Панель для ввода имени</summary>
    public GameObject namePanel;

    /// <summary>Панель для выбора пола</summary>
    public GameObject genderPanel;

    /// <summary>Панель для ввода даты рождения</summary>
    public GameObject datePanel;

    /// <summary>Панель для ввода времени рождения</summary>
    public GameObject timePanel;

    /// <summary>Панель для ввода часового пояса</summary>
    public GameObject timezonePanel;

    /// <summary>Панель для выбора города</summary>
    public GameObject cityPanel;

    /// <summary>Панель с итоговыми данными</summary>
    public GameObject summaryPanel;

    /// <summary>Панель успешного сохранения</summary>
    public GameObject successPanel;

    [Header("Name Input")]

    /// <summary>Поле ввода имени</summary>
    public InputField nameInput;

    /// <summary>Текстовый элемент для отображения ошибок имени</summary>
    public Text nameError;

    [Header("Gender Selection")]

    /// <summary> Группа переключателей для выбора пола пользователя  </summary>
    public ToggleGroup genderToggleGroup;

    /// <summary> Переключатель для выбора мужского пола </summary>
    public Toggle maleToggle;

    /// <summary> Переключатель для выбора женского пола </summary>
    public Toggle femaleToggle;

    /// <summary>Текстовый элемент для отображения ошибок выбора пола</summary>
    public Text genderError;

    [Header("Date Input")]

    /// <summary>Поле ввода даты рождения</summary>
    public InputField dateInput;

    /// <summary>Текстовый элемент для отображения ошибок даты</summary>
    public Text dateError;

    [Header("Time Input")]

    /// <summary>Поле ввода времени рождения</summary>
    public InputField timeInput;

    /// <summary>Текстовый элемент для отображения ошибок времени</summary>
    public Text timeError;

    [Header("Tine Zone Input")]

    /// <summary>Поле ввода временной зоны</summary>
    public InputField timezoneInput;

    /// <summary>Текстовый элемент для отображения ошибок временной зоны</summary>
    public Text timezoneError;

    [Header("City Input")]

    /// <summary>Поле ввода города</summary>
    public InputField cityInput;

    /// <summary>Контейнер для списка городов</summary>
    public Transform cityListContainer;

    /// <summary>Префаб элемента списка городов</summary>
    public GameObject cityItemPrefab;

    /// <summary>Текстовый элемент для отображения ошибок города</summary>
    public Text cityError;

    [Header("Summary")]

    /// <summary>Текстовый элемент для отображения итоговых данных</summary>
    public Text summaryText;

    /// <summary>Кнопка редактирования данных</summary>
    public Button editButton;

    /// <summary>Кнопка подтверждения данных</summary>
    public Button confirmButton;

    [Header("Success Panel")]

    /// <summary>Текстовый элемент сообщения об успехе</summary>
    public Text successText;

    /// <summary>Экземпляр данных пользователя</summary>
    private UserData userData = new UserData();

    /// <summary>Флаг для предотвращения рекурсии при вводе даты и времени</summary>
    private bool isFormatting = false;

    /// <summary>Минимальлная временная зон/summary>
    private float minTimezone = -12f;

    /// <summary>Максимальная временная зон/summary>
    private float maxTimezone = 14f;

    #endregion

    #region Методы

    /// <summary>
    /// Проверяет введенное имя и переходит к панели пола
    /// </summary>
    public void ValidateName()
    {
      if (string.IsNullOrWhiteSpace(nameInput.text))
      {
        nameError.text = "Имя не может быть пустым!";
        return;
      }

      userData.Name = nameInput.text.Trim();
      nameError.text = "";
      ShowPanel(genderPanel);
    }

    /// <summary>
    /// Проверяет выбор пола и переходит к панели даты
    /// </summary>
    public void ValidateGender()
    {
      Toggle selectedToggle = genderToggleGroup.GetFirstActiveToggle();

      if (selectedToggle == null)
      {
        genderError.text = "Пожалуйста, выберите пол!";
        return;
      }

      userData.Gender = selectedToggle == maleToggle ? "Мужской" : "Женский";
      genderError.text = "";
      ShowPanel(datePanel);
    }

    /// <summary>
    /// Проверяет введенную дату и переходит к панели времени
    /// </summary>
    public void ValidateDate()
    {
      dateError.text = "";

      if (dateInput.text.Length < 8)
      {
        dateError.text = "Введите полную дату (8 цифр)";
        return;
      }

      if (!DateTime.TryParseExact(
          dateInput.text,
          "dd.MM.yyyy",
          CultureInfo.InvariantCulture,
          DateTimeStyles.None,
          out DateTime result))
      {
        dateError.text = "Неверный формат даты! Используйте дд:мм:гггг";
        return;
      }

      userData.BirthDate = result;
      ShowPanel(timePanel);
    }

    /// <summary>
    /// Проверяет введенное время и переходит к панели временной зоны
    /// </summary>
    public void ValidateTime()
    {
      if (!DateTime.TryParseExact(
          timeInput.text,
          "HH:mm",
          CultureInfo.InvariantCulture,
          DateTimeStyles.None,
          out DateTime result))
      {
        timeError.text = "Неверный формат времени! Используйте чч:мм";
        return;
      }

      userData.BirthTime = result;
      timeError.text = "";
      ShowPanel(timezonePanel);
    }

    /// <summary>
    /// Форматирует ввод даты с автоматической расстановкой точек
    /// </summary>
    public void FormatDateInput(string value)
    {
      if (isFormatting) return;
      isFormatting = true;

      int caretPos = dateInput.caretPosition;
      bool wasAtEnd = caretPos == value.Length;

      string cleanText = Regex.Replace(value, "[^0-9]", "");
      if (cleanText.Length > 8) cleanText = cleanText.Substring(0, 8);

      string formatted = "";
      for (int i = 0; i < cleanText.Length; i++)
      {
        formatted += cleanText[i];

        if (i == 1 && cleanText.Length > 2)
          formatted += ".";
        else if (i == 3 && cleanText.Length > 4)
          formatted += ".";
      }

      if (cleanText.Length == 2)
        formatted += ".";
      else if (cleanText.Length == 4)
        formatted += ".";

      dateInput.text = formatted;
      dateInput.caretPosition = wasAtEnd ? formatted.Length : Mathf.Clamp(caretPos, 0, formatted.Length);
      isFormatting = false;
    }

    /// <summary>
    /// Форматирует ввод времени с автоматической расстановкой двоеточия
    /// </summary>
    public void FormatTimeInput(string value)
    {
      if (isFormatting) return;
      isFormatting = true;

      int caretPos = timeInput.caretPosition;
      bool wasAtEnd = caretPos == value.Length;

      string cleanText = Regex.Replace(value, "[^0-9]", "");
      if (cleanText.Length > 4) cleanText = cleanText.Substring(0, 4);

      string formatted = "";
      for (int i = 0; i < cleanText.Length; i++)
      {
        formatted += cleanText[i];

        if (i == 1 && cleanText.Length > 2)
          formatted += ":";
      }

      if (cleanText.Length == 2)
        formatted += ":";

      timeInput.text = formatted;
      timeInput.caretPosition = wasAtEnd ? formatted.Length : Mathf.Clamp(caretPos, 0, formatted.Length);
      isFormatting = false;
    }

    /// <summary>
    /// Проверяет введенную временную зону и переходит к панели города
    /// </summary>
    public void ValidateTimezone()
    {

      timezoneError.text = "";

      if (string.IsNullOrWhiteSpace(timezoneInput.text))
      {
        timezoneError.text = "Временная зона не может быть пустой!";
        return;
      }

      if (!float.TryParse(timezoneInput.text.Replace(',', '.'), NumberStyles.Any, CultureInfo.InvariantCulture, out float timezoneValue))
      {
        timezoneError.text = "Неверный формат! Введите число (например, 4 или -5.5)";
        return;
      }

      if (timezoneValue < minTimezone || timezoneValue > maxTimezone)
      {
        timezoneError.text = $"Временная зона должна быть в диапазоне от {minTimezone} до {maxTimezone}";
        return;
      }

      float fractionalPart = Math.Abs(timezoneValue) % 1;
      if (fractionalPart != 0 && fractionalPart != 0.5f)
      {
        timezoneError.text = "Неверное значение! Допустимые значения: целые числа и полчаса (например, 4, -5.5)";
        return;
      }

      userData.TimeZone = timezoneValue;
      timezoneError.text = "";

      ShowPanel(cityPanel);

      Debug.Log($"Временная зона сохранена: {timezoneValue}");
    }

    /// <summary>
    /// Форматирует ввод временной зоны с автоматической заменой запятой на точку
    /// </summary>
    public void FormatTimezoneInput(string value)
    {
      if (string.IsNullOrEmpty(value)) return;

      if (value.Contains(","))
      {
        timezoneInput.text = value.Replace(',', '.');
        timezoneInput.caretPosition = timezoneInput.text.Length;
      }

      if (value.Length > 5)
      {
        timezoneInput.text = value.Substring(0, 5);
        timezoneInput.caretPosition = 5;
      }
    }

    /// <summary>
    /// Обрабатывает изменение ввода города (вызывается при изменении текста)
    /// </summary>
    public void OnCityInputChanged()
    {
      if (cityInput.text.Length > 2)
      {
        StartCoroutine(SearchCities(cityInput.text));
        cityError.text = "Идет поиск...";
      }
      else
      {
        ClearCityList();
        cityError.text = cityInput.text.Length == 0 ? "" : "Введите хотя бы 3 символа";
      }
    }

    /// <summary>
    /// Проверяет выбор города и переходит к сводке
    /// </summary>
    public void ValidateCity()
    {
      if (string.IsNullOrEmpty(userData.BirthCity) || !string.Equals(cityInput.text, userData.BirthCity))
      {
        cityError.text = "Пожалуйста, выберите город из списка!";
        return;
      }

      ShowSummary();
    }

    /// <summary>
    /// Выполняет поиск городов через GeoNames API
    /// </summary>
    /// <param name="query">Поисковый запрос (название города)</param>
    /// <returns>Корутина для асинхронного выполнения</returns>
    private IEnumerator SearchCities(string query)
    {
      string url = $"http://api.geonames.org/searchJSON?name_startsWith=" +
        $"{UnityWebRequest.EscapeURL(query)}&maxRows=10&username={GEONAMES_USERNAME}&lang=ru&featureClass=P";

      using (UnityWebRequest request = UnityWebRequest.Get(url))
      {
        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
          Debug.LogError($"Ошибка: {request.error}");
          cityError.text = "Ошибка сети";
          yield break;
        }

        ProcessCityData(request.downloadHandler.text);
      }
    }

    /// <summary>
    /// Обрабатывает JSON-данные о городах
    /// </summary>
    /// <param name="json">JSON-строка с данными о городах</param>
    private void ProcessCityData(string json)
    {
      try
      {
        GeoNamesResponse response = JsonUtility.FromJson<GeoNamesResponse>(json);

        if (string.Equals(cityInput.text, userData.BirthCity))
          return;

        if (response == null || response.geonames == null || response.geonames.Count == 0)
        {
          cityError.text = "Города не найдены. Попробуйте другое название";
          return;
        }

        cityError.text = "";
        ClearCityList();

        foreach (CityData city in response.geonames)
        {
          string countryName = GetRussianCountryName(city.countryCode);
          Debug.Log(countryName);
          GameObject item = Instantiate(cityItemPrefab, cityListContainer);

          RectTransform rt = item.GetComponent<RectTransform>();
          rt.anchoredPosition = Vector2.zero;
          rt.sizeDelta = new Vector2(0, 80);

          item.GetComponentInChildren<Text>().text = $"{city.name}, {countryName}";
          item.GetComponent<Button>().onClick.AddListener(() => SelectCity(city));
          item.SetActive(true);
        }

        StartCoroutine(RefreshLayout());
      }
      catch (Exception e)
      {
        Debug.LogError($"Ошибка обработки данных: {e.Message}");
        cityError.text = "Ошибка загрузки данных";
      }
    }

    /// <summary>
    /// Корутина для обновления компоновки элементов в контейнере списка городов.
    /// Запускает одну итерацию обновления компоновки и вызывает принудительное перерисовывание компоновки.
    /// </summary>
    /// <returns>IEnumerator для управления корутиной.</returns>
    private IEnumerator RefreshLayout()
    {
      yield return new WaitForEndOfFrame();

      if (cityListContainer.TryGetComponent<ContentSizeFitter>(out var fitter))
      {
        fitter.enabled = false;
      }

      LayoutRebuilder.ForceRebuildLayoutImmediate(cityListContainer as RectTransform);

      yield return new WaitForEndOfFrame();

      if (fitter != null)
      {
        fitter.enabled = true;
      }
    }

    /// <summary>
    /// Очищает текущий список городов
    /// </summary>
    private void ClearCityList()
    {
      foreach (Transform child in cityListContainer)
      {
        Destroy(child.gameObject);
      }
    }

    /// <summary>
    /// Обрабатывает выбор города из списка
    /// </summary>
    /// <param name="city">Выбранный город</param>
    private void SelectCity(CityData city)
    {
      userData.BirthCity = $"{city.name}, {GetRussianCountryName(city.countryCode)}";
      userData.Latitude = city.lat;
      userData.Longitude = city.lng;

      cityInput.text = userData.BirthCity;
      cityError.text = "";
      ClearCityList();
    }

    /// <summary>
    /// Получает русское название страны по коду страны.
    /// </summary>
    /// <param name="countryCode">Код страны (например, "RU", "US").</param>
    /// <returns>Русское название страны, если код найден, иначе возвращает сам код страны.</returns>
    private string GetRussianCountryName(string countryCode)
    {
      var countryNames = new Dictionary<string, string>
          {
              {"RU", "Россия"}, {"US", "США"}, {"DE", "Германия"},
              {"FR", "Франция"}, {"CN", "Китай"}, {"JP", "Япония"},
              {"KZ", "Казахстан"}, {"UA", "Украина"}, {"BY", "Беларусь"},
              {"GB", "Великобритания"}, {"IT", "Италия"}, {"ES", "Испания"},
              {"CA", "Канада"}, {"AU", "Австралия"}, {"TR", "Турция"}
          };

      return countryNames.TryGetValue(countryCode, out string name) ?
          name :
          countryCode;
    }

    /// <summary>
    /// Сохраняет данные пользователя в PlayerPrefs
    /// </summary>
    private void SaveUserData()
    {
      PlayerPrefs.SetString("UserName", userData.Name);
      PlayerPrefs.SetString("Gender", userData.Gender);
      PlayerPrefs.SetString("BirthDate", userData.BirthDate.ToString("o"));
      PlayerPrefs.SetString("BirthTime", userData.BirthTime.ToString("o"));
      PlayerPrefs.SetFloat("Timezone", userData.TimeZone);
      PlayerPrefs.SetString("BirthCity", userData.BirthCity);
      PlayerPrefs.SetFloat("Latitude", userData.Latitude);
      PlayerPrefs.SetFloat("Longitude", userData.Longitude);
      PlayerPrefs.Save();
    }

    /// <summary>
    /// Заполняет поля ввода сохраненными данными
    /// </summary>
    private void FillFormWithSavedData()
    {
      nameInput.text = userData.Name;

      if (!string.IsNullOrEmpty(userData.Gender))
      {
        if (userData.Gender == "Мужской")
          maleToggle.isOn = true;
        else if (userData.Gender == "Женский")
          femaleToggle.isOn = true;
      }

      if (userData.BirthDate != default(DateTime))
        dateInput.text = userData.BirthDate.ToString("dd.MM.yyyy");

      if (userData.BirthTime != default(DateTime))
        timeInput.text = userData.BirthTime.ToString("HH:mm");

      timezoneInput.text = userData.TimeZone.ToString(CultureInfo.InvariantCulture);

      if (!string.IsNullOrEmpty(userData.BirthCity))
        cityInput.text = userData.BirthCity;
    }

    /// <summary>
    /// Отображает сводную панель с данными пользователя
    /// </summary>
    private void ShowSummary()
    {
      summaryText.text = userData.ToString();
      ShowPanel(summaryPanel);
    }

    /// <summary>
    /// Обрабатывает нажатие кнопки редактирования
    /// </summary>
    public void OnEditButtonClick()
    {
      FillFormWithSavedData();

      ShowPanel(namePanel);
    }

    /// <summary>
    /// Обрабатывает нажатие кнопки подтверждения
    /// </summary>
    public void OnConfirmButtonClick()
    {
      SaveUserData();
      ShowPanel(successPanel);
    }

    /// <summary>
    /// Обрабатывает нажатие кнопки продолжения
    /// </summary>
    public void OnContinueButtonClick()
    {
      SceneManager.LoadScene("PrologueScene");
    }

    /// <summary>
    /// Активирует указанную панель и деактивирует остальные
    /// </summary>
    /// <param name="panel">Панель для отображения</param>
    private void ShowPanel(GameObject panel)
    {
      namePanel.SetActive(panel == namePanel);
      genderPanel.SetActive(panel == genderPanel);
      datePanel.SetActive(panel == datePanel);
      timePanel.SetActive(panel == timePanel);
      timezonePanel.SetActive(panel == timezonePanel);
      cityPanel.SetActive(panel == cityPanel);
      summaryPanel.SetActive(panel == summaryPanel);
      successPanel.SetActive(panel == successPanel);

      if (panel == successPanel)
      {
        if (!successPanel.GetComponent<ContentPanelClickHandler>())
        {
          successPanel.AddComponent<ContentPanelClickHandler>().OnClick += OnSuccessPanelClicked;
        }
        else
        {
          successPanel.GetComponent<ContentPanelClickHandler>().OnClick += OnSuccessPanelClicked;
          successPanel.GetComponent<ContentPanelClickHandler>().enabled = true;
        }
      }
      else
      {
        if (successPanel.GetComponent<ContentPanelClickHandler>())
        {
          successPanel.GetComponent<ContentPanelClickHandler>().OnClick -= OnSuccessPanelClicked;
          successPanel.GetComponent<ContentPanelClickHandler>().enabled = false;
        }
      }
    }

    /// <summary>
    /// Обрабатывает клик по панели успеха
    /// </summary>
    private void OnSuccessPanelClicked()
    {
      if (SceneTransitionManager.Instance != null)
      {
        SceneTransitionManager.Instance.LoadSceneWithFade("PrologueScene");
      }
      else
      {
        SceneManager.LoadScene("PrologueScene");
      }
    }

    #endregion

    #region Базовый класс (MonoBehaviour)

    /// <summary>
    /// Инициализация компонента при запуске
    /// </summary>
    void Start()
    {
      ShowPanel(namePanel);
      dateInput.onValueChanged.AddListener(FormatDateInput);
      timeInput.onValueChanged.AddListener(FormatTimeInput);
      timezoneInput.onValueChanged.AddListener(FormatTimezoneInput);

      if (SceneTransitionManager.Instance == null)
      {
        GameObject transitionManagerObj = new GameObject("SceneTransitionManager");
        transitionManagerObj.AddComponent<SceneTransitionManager>();
        DontDestroyOnLoad(transitionManagerObj);
      }

      if (cityListContainer.parent.parent.TryGetComponent<ScrollRect>(out var scrollRect))
      {
        scrollRect.vertical = true;
        scrollRect.horizontal = false;
        scrollRect.movementType = ScrollRect.MovementType.Clamped;
        scrollRect.inertia = true;
        scrollRect.decelerationRate = 0.135f;

        scrollRect.content = cityListContainer.GetComponent<RectTransform>();
      }

      if (editButton != null)
        editButton.onClick.AddListener(OnEditButtonClick);

      if (confirmButton != null)
        confirmButton.onClick.AddListener(OnConfirmButtonClick);
    }

    #endregion
  }
}