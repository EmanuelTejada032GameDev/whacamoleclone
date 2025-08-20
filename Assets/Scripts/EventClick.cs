using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class EventClick : MonoBehaviour,
    IPointerDownHandler,
    IPointerUpHandler,
    IPointerClickHandler,
    IPointerEnterHandler,
    IPointerExitHandler
{

    [SerializeField] private float hitInterval = 0.5f; // Time between hits in seconds
    private Coroutine hitCoroutine;

    public void OnPointerDown(PointerEventData eventData)
    {
        //Debug.Log("OnPointerDown");
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        //Debug.Log("OnPointerUp");
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        //Debug.Log("OnPointerClick");
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        hitCoroutine = StartCoroutine(RepeatMoleHit());
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (hitCoroutine != null)
        {
            StopCoroutine(hitCoroutine);
            hitCoroutine = null;
        }
    }

    private IEnumerator RepeatMoleHit()
    {
        while (true)
        {
            Debug.Log("mole hit");
            yield return new WaitForSeconds(hitInterval);
        }
    }
}
