using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.EventSystems;
using UnityEngine;

/// <summary>
/// Обработчик кликов по UI элементам
/// </summary>
public class ContentPanelClickHandler : MonoBehaviour, IPointerClickHandler
{
    public event System.Action OnClick;

    /// <summary>
    /// Обработчик события клика UI элемента
    /// </summary>
    public void OnPointerClick(PointerEventData eventData)
    {
        OnClick?.Invoke();
    }
}
