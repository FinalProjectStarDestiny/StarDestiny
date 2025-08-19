using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.EventSystems;
using UnityEngine;

public class SuccessPanelClickHandler : MonoBehaviour, IPointerClickHandler
{
  public event Action OnClick;

  public void OnPointerClick(PointerEventData eventData)
  {
    OnClick?.Invoke();
  }
}

