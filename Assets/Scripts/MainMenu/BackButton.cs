using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(ISubmitHandler))]
public class BackButton : BackButtonInvoker
{
    [ReadOnly]
    private ISubmitHandler handler;

    [SerializeField] private GameObject[] mustBeInactives;

    private void Awake()
    {
        if (handler == null)
        {
            handler = GetComponent<ISubmitHandler>();
        }
    }

    protected override void OnCancel(InputAction.CallbackContext obj)
    {
        if (mustBeInactives.Any(go => go.activeInHierarchy)) return;
        handler.OnSubmit(new BaseEventData(EventSystem.current));
    }
}
