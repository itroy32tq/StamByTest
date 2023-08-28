using Net;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class JampComponent : MonoBehaviour, IPointerClickHandler
{
    public SpineBoyModel Model;
    public void OnPointerClick(PointerEventData eventData)
    {
        Model.TryJump();    
    }

}
