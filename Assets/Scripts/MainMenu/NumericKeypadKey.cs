using UnityEngine;
using UnityEngine.UI;

// Klawisz klawiatury numerycznej. Akcję podpina NumericKeypad (rodzic) w trakcie gry.
[RequireComponent(typeof(Button))]
public class NumericKeypadKey : MonoBehaviour
{
    public enum KeyAction
    {
        Digit,
        Backspace,
        Enter
    }

    public KeyAction action;
    [Tooltip("Cyfra dopisywana do kodu (tylko dla Digit).")]
    public string digit;
}
