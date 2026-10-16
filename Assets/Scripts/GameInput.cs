using UnityEngine.InputSystem;

// Wszystkie akcje gry zdefiniowane w kodzie (New Input System).
// Akcje są tworzone raz i pozostają włączone przez cały czas działania gry,
// więc nie trzeba nic podpinać w inspektorze w żadnej ze scen.
public static class GameInput
{
    public static readonly InputAction MoveUp;
    public static readonly InputAction MoveDown;
    public static readonly InputAction MoveLeft;
    public static readonly InputAction MoveRight;
    public static readonly InputAction SwitchCube;
    public static readonly InputAction Pause;
    public static readonly InputAction Restart;

    public static readonly InputAction PreviousVideo;
    public static readonly InputAction NextVideo;
    public static readonly InputAction Confirm;
    public static readonly InputAction Click;

    static GameInput()
    {
        MoveUp = CreateButton("MoveUp", "<Keyboard>/w", "<Keyboard>/upArrow", "<Gamepad>/dpad/up");
        MoveDown = CreateButton("MoveDown", "<Keyboard>/s", "<Keyboard>/downArrow", "<Gamepad>/dpad/down");
        MoveLeft = CreateButton("MoveLeft", "<Keyboard>/a", "<Keyboard>/leftArrow", "<Gamepad>/dpad/left");
        MoveRight = CreateButton("MoveRight", "<Keyboard>/d", "<Keyboard>/rightArrow", "<Gamepad>/dpad/right");
        SwitchCube = CreateButton("SwitchCube", "<Keyboard>/tab", "<Gamepad>/buttonNorth");
        Pause = CreateButton("Pause", "<Keyboard>/escape", "<Gamepad>/start");
        Restart = CreateButton("Restart", "<Keyboard>/r", "<Gamepad>/select");

        PreviousVideo = CreateButton("PreviousVideo", "<Keyboard>/q", "<Gamepad>/leftShoulder");
        NextVideo = CreateButton("NextVideo", "<Keyboard>/e", "<Gamepad>/rightShoulder");
        Confirm = CreateButton("Confirm", "<Keyboard>/enter", "<Keyboard>/numpadEnter", "<Gamepad>/buttonSouth");
        Click = CreateButton("Click", "<Mouse>/leftButton");
    }

    private static InputAction CreateButton(string name, params string[] bindings)
    {
        var action = new InputAction(name, InputActionType.Button);
        foreach (var binding in bindings)
        {
            action.AddBinding(binding);
        }
        action.Enable();
        return action;
    }
}
