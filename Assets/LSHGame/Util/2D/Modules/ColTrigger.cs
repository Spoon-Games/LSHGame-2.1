using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider2D))]
public class ColTrigger : MonoBehaviour
{
    [SerializeField]
    protected LayerMask layerMask;

    [SerializeField]
    public UnityEvent OnTriggerEnteredEvent;

    [SerializeField]
    public UnityEvent OnTriggerExitedEvent;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(layerMask == (layerMask | (1 << collision.gameObject.layer)))
        {
            OnTriggerEntered(collision);
        }
    }

    protected virtual void OnTriggerEntered(Collider2D collision)
    {
        OnTriggerEnteredEvent.Invoke();
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (layerMask == (layerMask | (1 << collision.gameObject.layer)))
        {
            OnTriggerExited(collision);
        }
    }

    protected virtual void OnTriggerExited(Collider2D collision)
    {
        OnTriggerExitedEvent.Invoke();
    }
}
