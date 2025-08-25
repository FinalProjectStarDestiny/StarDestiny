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
  /// �������� ����� ��� ����� � ��������� ���������������� ������
  /// </summary>
  public class DataCollector : MonoBehaviour
  {
    #region ���������

    /// <summary>��� ��� ������� � GeoNames API</summary>
    private const string GEONAMES_USERNAME = "StarDestiny";

    #endregion

    #region ��������� ����

    /// <summary>����� ��� �������������� ������ �� GeoNames API</summary>
    [System.Serializable]
    private class GeoNamesResponse
    {
      /// <summary>������ ��������� �������</summary>
      public List<CityData> geonames;
    }

    /// <summary>����� ��� �������� ������ � ������</summary>
    [System.Serializable]
    private class CityData
    {
      /// <summary>�������� ������</summary>
      public string name;

      /// <summary>�������� ������</summary>
      public string countryName;

      /// <summary>��� ������</summary>
      public string countryCode;

      /// <summary>�������������� ������</summary>
      public float lat;

      /// <summary>�������������� �������</summary>
      public float lng;
    }

    #endregion

    #region ���� � ��������

    [Header("UI Panels")]

    /// <summary>������ ��� ����� �����</summary>
    public GameObject namePanel;

    /// <summary>������ ��� ������ ����</summary>
    public GameObject genderPanel;

    /// <summary>������ ��� ����� ���� ��������</summary>
    public GameObject datePanel;

    /// <summary>������ ��� ����� ������� ��������</summary>
    public GameObject timePanel;

    /// <summary>������ ��� ����� �������� �����</summary>
    public GameObject timezonePanel;

    /// <summary>������ ��� ������ ������</summary>
    public GameObject cityPanel;

    /// <summary>������ � ��������� �������</summary>
    public GameObject summaryPanel;

    /// <summary>������ ��������� ����������</summary>
    public GameObject successPanel;

    [Header("Name Input")]

    /// <summary>���� ����� �����</summary>
    public InputField nameInput;

    /// <summary>��������� ������� ��� ����������� ������ �����</summary>
    public Text nameError;

    [Header("Gender Selection")]

    /// <summary> ������ �������������� ��� ������ ���� ������������  </summary>
    public ToggleGroup genderToggleGroup;

    /// <summary> ������������� ��� ������ �������� ���� </summary>
    public Toggle maleToggle;

    /// <summary> ������������� ��� ������ �������� ���� </summary>
    public Toggle femaleToggle;

    /// <summary>��������� ������� ��� ����������� ������ ������ ����</summary>
    public Text genderError;

    [Header("Date Input")]

    /// <summary>���� ����� ���� ��������</summary>
    public InputField dateInput;

    /// <summary>��������� ������� ��� ����������� ������ ����</summary>
    public Text dateError;

    [Header("Time Input")]

    /// <summary>���� ����� ������� ��������</summary>
    public InputField timeInput;

    /// <summary>��������� ������� ��� ����������� ������ �������</summary>
    public Text timeError;

    [Header("Tine Zone Input")]

    /// <summary>���� ����� ��������� ����</summary>
    public InputField timezoneInput;

    /// <summary>��������� ������� ��� ����������� ������ ��������� ����</summary>
    public Text timezoneError;

    [Header("City Input")]

    /// <summary>���� ����� ������</summary>
    public InputField cityInput;

    /// <summary>��������� ��� ������ �������</summary>
    public Transform cityListContainer;

    /// <summary>������ �������� ������ �������</summary>
    public GameObject cityItemPrefab;

    /// <summary>��������� ������� ��� ����������� ������ ������</summary>
    public Text cityError;

    [Header("Summary")]

    /// <summary>��������� ������� ��� ����������� �������� ������</summary>
    public Text summaryText;

    /// <summary>������ �������������� ������</summary>
    public Button editButton;

    /// <summary>������ ������������� ������</summary>
    public Button confirmButton;

    [Header("Success Panel")]

    /// <summary>��������� ������� ��������� �� ������</summary>
    public Text successText;

    /// <summary>��������� ������ ������������</summary>
    private UserData userData = new UserData();

    /// <summary>���� ��� �������������� �������� ��� ����� ���� � �������</summary>
    private bool isFormatting = false;

    /// <summary>������������ ��������� ���/summary>
    private float minTimezone = -12f;

    /// <summary>������������ ��������� ���/summary>
    private float maxTimezone = 14f;

    #endregion

    #region ������

    /// <summary>
    /// ��������� ��������� ��� � ��������� � ������ ����
    /// </summary>
    public void ValidateName()
    {
      if (string.IsNullOrWhiteSpace(nameInput.text))
      {
        nameError.text = "��� �� ����� ���� ������!";
        return;
      }

      userData.Name = nameInput.text.Trim();
      nameError.text = "";
      ShowPanel(genderPanel);
    }

    /// <summary>
    /// ��������� ����� ���� � ��������� � ������ ����
    /// </summary>
    public void ValidateGender()
    {
      Toggle selectedToggle = genderToggleGroup.GetFirstActiveToggle();

      if (selectedToggle == null)
      {
        genderError.text = "����������, �������� ���!";
        return;
      }

      userData.Gender = selectedToggle == maleToggle ? "�������" : "�������";
      genderError.text = "";
      ShowPanel(datePanel);
    }

    /// <summary>
    /// ��������� ��������� ���� � ��������� � ������ �������
    /// </summary>
    public void ValidateDate()
    {
      dateError.text = "";

      if (dateInput.text.Length < 8)
      {
        dateError.text = "������� ������ ���� (8 ����)";
        return;
      }

      if (!DateTime.TryParseExact(
          dateInput.text,
          "dd.MM.yyyy",
          CultureInfo.InvariantCulture,
          DateTimeStyles.None,
          out DateTime result))
      {
        dateError.text = "�������� ������ ����! ����������� ��:��:����";
        return;
      }

      userData.BirthDate = result;
      ShowPanel(timePanel);
    }

    /// <summary>
    /// ��������� ��������� ����� � ��������� � ������ ��������� ����
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
        timeError.text = "�������� ������ �������! ����������� ��:��";
        return;
      }

      userData.BirthTime = result;
      timeError.text = "";
      ShowPanel(timezonePanel);
    }

    /// <summary>
    /// ����������� ���� ���� � �������������� ������������ �����
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
    /// ����������� ���� ������� � �������������� ������������ ���������
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
    /// ��������� ��������� ��������� ���� � ��������� � ������ ������
    /// </summary>
    public void ValidateTimezone()
    {

      timezoneError.text = "";

      if (string.IsNullOrWhiteSpace(timezoneInput.text))
      {
        timezoneError.text = "��������� ���� �� ����� ���� ������!";
        return;
      }

      if (!float.TryParse(timezoneInput.text.Replace(',', '.'), NumberStyles.Any, CultureInfo.InvariantCulture, out float timezoneValue))
      {
        timezoneError.text = "�������� ������! ������� ����� (��������, 4 ��� -5.5)";
        return;
      }

      if (timezoneValue < minTimezone || timezoneValue > maxTimezone)
      {
        timezoneError.text = $"��������� ���� ������ ���� � ��������� �� {minTimezone} �� {maxTimezone}";
        return;
      }

      float fractionalPart = Math.Abs(timezoneValue) % 1;
      if (fractionalPart != 0 && fractionalPart != 0.5f)
      {
        timezoneError.text = "�������� ��������! ���������� ��������: ����� ����� � ������� (��������, 4, -5.5)";
        return;
      }

      userData.TimeZone = timezoneValue;
      timezoneError.text = "";

      ShowPanel(cityPanel);

      Debug.Log($"��������� ���� ���������: {timezoneValue}");
    }

    /// <summary>
    /// ����������� ���� ��������� ���� � �������������� ������� ������� �� �����
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
    /// ������������ ��������� ����� ������ (���������� ��� ��������� ������)
    /// </summary>
    public void OnCityInputChanged()
    {
      if (cityInput.text.Length > 2)
      {
        StartCoroutine(SearchCities(cityInput.text));
        cityError.text = "���� �����...";
      }
      else
      {
        ClearCityList();
        cityError.text = cityInput.text.Length == 0 ? "" : "������� ���� �� 3 �������";
      }
    }

    /// <summary>
    /// ��������� ����� ������ � ��������� � ������
    /// </summary>
    public void ValidateCity()
    {
      if (string.IsNullOrEmpty(userData.BirthCity) || !string.Equals(cityInput.text, userData.BirthCity))
      {
        cityError.text = "����������, �������� ����� �� ������!";
        return;
      }

      ShowSummary();
    }

    /// <summary>
    /// ��������� ����� ������� ����� GeoNames API
    /// </summary>
    /// <param name="query">��������� ������ (�������� ������)</param>
    /// <returns>�������� ��� ������������ ����������</returns>
    private IEnumerator SearchCities(string query)
    {
      string url = $"http://api.geonames.org/searchJSON?name_startsWith=" +
        $"{UnityWebRequest.EscapeURL(query)}&maxRows=10&username={GEONAMES_USERNAME}&lang=ru&featureClass=P";

      using (UnityWebRequest request = UnityWebRequest.Get(url))
      {
        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
          Debug.LogError($"������: {request.error}");
          cityError.text = "������ ����";
          yield break;
        }

        ProcessCityData(request.downloadHandler.text);
      }
    }

    /// <summary>
    /// ������������ JSON-������ � �������
    /// </summary>
    /// <param name="json">JSON-������ � ������� � �������</param>
    private void ProcessCityData(string json)
    {
      try
      {
        GeoNamesResponse response = JsonUtility.FromJson<GeoNamesResponse>(json);

        if (string.Equals(cityInput.text, userData.BirthCity))
          return;

        if (response == null || response.geonames == null || response.geonames.Count == 0)
        {
          cityError.text = "������ �� �������. ���������� ������ ��������";
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
        Debug.LogError($"������ ��������� ������: {e.Message}");
        cityError.text = "������ �������� ������";
      }
    }

    /// <summary>
    /// �������� ��� ���������� ���������� ��������� � ���������� ������ �������.
    /// ��������� ���� �������� ���������� ���������� � �������� �������������� ��������������� ����������.
    /// </summary>
    /// <returns>IEnumerator ��� ���������� ���������.</returns>
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
    /// ������� ������� ������ �������
    /// </summary>
    private void ClearCityList()
    {
      foreach (Transform child in cityListContainer)
      {
        Destroy(child.gameObject);
      }
    }

    /// <summary>
    /// ������������ ����� ������ �� ������
    /// </summary>
    /// <param name="city">��������� �����</param>
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
    /// �������� ������� �������� ������ �� ���� ������.
    /// </summary>
    /// <param name="countryCode">��� ������ (��������, "RU", "US").</param>
    /// <returns>������� �������� ������, ���� ��� ������, ����� ���������� ��� ��� ������.</returns>
    private string GetRussianCountryName(string countryCode)
    {
      var countryNames = new Dictionary<string, string>
          {
              {"RU", "������"}, {"US", "���"}, {"DE", "��������"},
              {"FR", "�������"}, {"CN", "�����"}, {"JP", "������"},
              {"KZ", "���������"}, {"UA", "�������"}, {"BY", "��������"},
              {"GB", "��������������"}, {"IT", "������"}, {"ES", "�������"},
              {"CA", "������"}, {"AU", "���������"}, {"TR", "������"}
          };

      return countryNames.TryGetValue(countryCode, out string name) ?
          name :
          countryCode;
    }

    /// <summary>
    /// ��������� ������ ������������ � PlayerPrefs
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
    /// ��������� ���� ����� ������������ �������
    /// </summary>
    private void FillFormWithSavedData()
    {
      nameInput.text = userData.Name;

      if (!string.IsNullOrEmpty(userData.Gender))
      {
        if (userData.Gender == "�������")
          maleToggle.isOn = true;
        else if (userData.Gender == "�������")
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
    /// ���������� ������� ������ � ������� ������������
    /// </summary>
    private void ShowSummary()
    {
      summaryText.text = userData.ToString();
      ShowPanel(summaryPanel);
    }

    /// <summary>
    /// ������������ ������� ������ ��������������
    /// </summary>
    public void OnEditButtonClick()
    {
      FillFormWithSavedData();

      ShowPanel(namePanel);
    }

    /// <summary>
    /// ������������ ������� ������ �������������
    /// </summary>
    public void OnConfirmButtonClick()
    {
      SaveUserData();
      ShowPanel(successPanel);
    }

    /// <summary>
    /// ������������ ������� ������ �����������
    /// </summary>
    public void OnContinueButtonClick()
    {
      SceneManager.LoadScene("PrologueScene");
    }

    /// <summary>
    /// ���������� ��������� ������ � ������������ ���������
    /// </summary>
    /// <param name="panel">������ ��� �����������</param>
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
    /// ������������ ���� �� ������ ������
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

    #region ������� ����� (MonoBehaviour)

    /// <summary>
    /// ������������� ���������� ��� �������
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