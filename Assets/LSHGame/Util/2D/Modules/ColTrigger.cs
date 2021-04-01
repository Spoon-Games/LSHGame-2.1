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

    private Collider2D col;

    private bool _isTriggered = false;
    public bool IsTriggered { get => _isTriggered; private set
        {
            if (value && !_isTriggered)
                OnTriggerEntered();
            else if (!value && _isTriggered)
                OnTriggerExited();

            _isTriggered = value;
        }
    }

    private void Awake()
    {
        col = GetComponent<Collider2D>();
    }

    private void FixedUpdate()
    {
        IsTriggered = Physics2D.IsTouchingLayers(col, layerMask);
    }


    //private void OnTriggerEnter2D(Collider2D collision)
    //{
    //    if(layerMask == (layerMask | (1 << collision.gameObject.layer)))
    //    {
    //        OnTriggerEntered(collision);
    //    }
    //}

    protected virtual void OnTriggerEntered()
    {
        OnTriggerEnteredEvent.Invoke();
    }

    //private void OnTriggerExit2D(Collider2D collision)
    //{
    //    if (layerMask == (layerMask | (1 << collision.gameObject.layer)))
    //    {
    //        OnTriggerExited(collision);
    //    }
    //}

    protected virtual void OnTriggerExited()
    {
        OnTriggerExitedEvent.Invoke();
    }
}
