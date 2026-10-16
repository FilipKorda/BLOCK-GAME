using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

// Automatyczne zaznaczanie przycisków UI, gdy podłączony jest pad (bez zaznaczenia pad nie ma
// od czego zacząć nawigacji). Działa tylko w menu: w widokach menu głównego (MenuView) i w pauzie.
// - Wejście w widok / otwarcie pauzy zaznacza jego "First Selected" albo, gdy pole jest puste,
//   najwyżej położony aktywny przycisk.
// - Gdy zaznaczenie zniknie (np. pad podłączony w trakcie, przycisk ukryty), wraca do bieżącego widoku.
// W trakcie rozgrywki nic nie jest zaznaczane, żeby przycisk A nie klikał czegoś w HUD.
public class GamepadMenuSelection : MonoBehaviour
{
    private struct SelectionContext
    {
        public Transform root;
        public Selectable preferred;
    }

    private static GamepadMenuSelection instance;
    private static readonly List<SelectionContext> contexts = new();
    private static bool selectionRequested;

    public static bool GamepadConnected => Gamepad.all.Count > 0;

    // Czy gracz ostatnio używał pada (a nie klawiatury/myszy). Na starcie: czy pad jest podłączony.
    public static bool UsingGamepad { get; private set; }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void CreateInstance()
    {
        if (instance != null) return;

        GameObject host = new("[Gamepad Menu Selection]");
        DontDestroyOnLoad(host);
        instance = host.AddComponent<GamepadMenuSelection>();
        UsingGamepad = GamepadConnected;
    }

    // Widok menu / panel pauzy staje się aktywny - zaznacz w nim przycisk w następnej klatce
    // (po tym, jak widok włączy i wyłączy swoje elementy).
    public static void Push(Transform root, Selectable preferred)
    {
        if (root == null) return;

        contexts.RemoveAll(context => context.root == root || context.root == null);
        contexts.Add(new SelectionContext { root = root, preferred = preferred });
        selectionRequested = true;
    }

    public static void Pop(Transform root)
    {
        contexts.RemoveAll(context => context.root == root || context.root == null);
        selectionRequested = true;
    }

    private void OnEnable() => InputSystem.onDeviceChange += OnDeviceChange;
    private void OnDisable() => InputSystem.onDeviceChange -= OnDeviceChange;

    private void OnDeviceChange(InputDevice device, InputDeviceChange change)
    {
        if (device is not Gamepad) return;

        if (change == InputDeviceChange.Added)
        {
            selectionRequested = true;
            UsingGamepad = true;
        }
        else if (change == InputDeviceChange.Removed && !GamepadConnected)
        {
            UsingGamepad = false;
        }
    }

    private static void UpdateActiveDevice()
    {
        Gamepad gamepad = Gamepad.current;
        if (gamepad != null && (gamepad.wasUpdatedThisFrame && IsGamepadActuated(gamepad)))
        {
            UsingGamepad = true;
            return;
        }

        Keyboard keyboard = Keyboard.current;
        Mouse mouse = Mouse.current;
        if ((keyboard != null && keyboard.anyKey.wasPressedThisFrame)
            || (mouse != null && (mouse.leftButton.wasPressedThisFrame || mouse.delta.ReadValue().sqrMagnitude > 4f)))
        {
            UsingGamepad = false;
        }
    }

    private static bool IsGamepadActuated(Gamepad gamepad)
    {
        return gamepad.buttonSouth.isPressed || gamepad.buttonEast.isPressed || gamepad.buttonWest.isPressed || gamepad.buttonNorth.isPressed
               || gamepad.startButton.isPressed || gamepad.selectButton.isPressed
               || gamepad.leftShoulder.isPressed || gamepad.rightShoulder.isPressed
               || gamepad.dpad.ReadValue().sqrMagnitude > 0.25f
               || gamepad.leftStick.ReadValue().sqrMagnitude > 0.25f || gamepad.rightStick.ReadValue().sqrMagnitude > 0.25f;
    }

    private void Update()
    {
        UpdateActiveDevice();

        EventSystem eventSystem = EventSystem.current;
        if (eventSystem == null || !GamepadConnected) return;

        contexts.RemoveAll(context => context.root == null || !context.root.gameObject.activeInHierarchy);
        if (contexts.Count == 0) return;

        SelectionContext current = contexts[^1];
        PrepareInputFields(current.root);
        GameObject selected = eventSystem.currentSelectedGameObject;
        if (selected != null && selected.TryGetComponent(out TMP_InputField inputField) && LeaveInputField(inputField, eventSystem)) return;
        bool selectionValid = selected != null && selected.activeInHierarchy
                              && selected.TryGetComponent(out Selectable selectable) && selectable.IsInteractable()
                              && selected.transform.IsChildOf(current.root);

        if (!selectionRequested && selectionValid) return;
        selectionRequested = false;

        Selectable target = IsUsable(current.preferred) ? current.preferred : FindTopmostSelectable(current.root);
        if (target != null) eventSystem.SetSelectedGameObject(target.gameObject);
    }

    // Z padem pole tekstowe po zaznaczeniu ma być tylko podświetlone - inaczej od razu przechodzi
    // w tryb pisania, a wtedy krzyżak przesuwa kursor w tekście i nie da się przejść dalej.
    // Edycję włącza przycisk A (Submit) albo kliknięcie myszą.
    private static void PrepareInputFields(Transform root)
    {
        foreach (TMP_InputField field in root.GetComponentsInChildren<TMP_InputField>())
        {
            field.shouldActivateOnSelect = false;
        }
    }

    // Pole w trybie pisania: krzyżak / lewa gałka przechodzi do sąsiedniego przycisku, B kończy edycję.
    private static bool LeaveInputField(TMP_InputField field, EventSystem eventSystem)
    {
        if (!field.isFocused) return false;

        Gamepad gamepad = Gamepad.current;
        if (gamepad == null) return false;

        if (gamepad.buttonEast.wasPressedThisFrame)
        {
            field.DeactivateInputField();
            return true;
        }

        Selectable next = null;
        if (gamepad.dpad.up.wasPressedThisFrame || gamepad.leftStick.up.wasPressedThisFrame) next = field.FindSelectableOnUp();
        else if (gamepad.dpad.down.wasPressedThisFrame || gamepad.leftStick.down.wasPressedThisFrame) next = field.FindSelectableOnDown();
        else if (gamepad.dpad.left.wasPressedThisFrame || gamepad.leftStick.left.wasPressedThisFrame) next = field.FindSelectableOnLeft();
        else if (gamepad.dpad.right.wasPressedThisFrame || gamepad.leftStick.right.wasPressedThisFrame) next = field.FindSelectableOnRight();
        else return false;

        field.DeactivateInputField();
        if (next != null) eventSystem.SetSelectedGameObject(next.gameObject);
        return true;
    }

    private static bool IsUsable(Selectable selectable)
    {
        return selectable != null && selectable.gameObject.activeInHierarchy && selectable.IsInteractable();
    }

    // Najwyżej położony (a przy równej wysokości - najbardziej z lewej) aktywny przycisk w widoku.
    private static Selectable FindTopmostSelectable(Transform root)
    {
        return root.GetComponentsInChildren<Selectable>()
            .Where(IsUsable)
            .Select(selectable => (selectable, screen: ScreenPosition(selectable.transform)))
            .OrderByDescending(entry => Mathf.Round(entry.screen.y))
            .ThenBy(entry => entry.screen.x)
            .Select(entry => entry.selectable)
            .FirstOrDefault();
    }

    private static Vector2 ScreenPosition(Transform target)
    {
        Canvas canvas = target.GetComponentInParent<Canvas>();
        Camera camera = canvas != null && canvas.rootCanvas.renderMode != RenderMode.ScreenSpaceOverlay ? canvas.rootCanvas.worldCamera : null;
        return RectTransformUtility.WorldToScreenPoint(camera, target.position);
    }
}
