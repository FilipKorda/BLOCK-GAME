using System.Linq;
using UnityEditor;
using UnityEngine;

// Tools > BLOCK GAME > Level 27 > Setup Bridge Buttons
// Podpina w prefabie "Level 27" nowe, unikalne zachowanie mostów:
// - Switch With Two Bridge Button      (kółko) -> chowa tylko lewy most (swój "bridge"),
// - Switch With Two Bridge Button (1)  (kółko) -> chowa tylko prawy most (swój "bridge"),
// - Ground Bridge Button Two           (X, tylko na stojąco) -> chowa oba te mosty.
// Wszystko jednorazowo. Stare skrypty tych przycisków są wyłączane tylko w tym prefabie.
// Można uruchamiać wielokrotnie - istniejące komponenty są aktualizowane, a nie dublowane.
public static class Level27Setup
{
    private const string Level27PrefabPath = "Assets/Gameplay Prefabs/Level 27.prefab";
    private const string LeftSwitchName = "Switch With Two Bridge Button";
    private const string RightSwitchName = "Switch With Two Bridge Button (1)";
    private const string CrossButtonName = "Ground Bridge Button Two";

    [MenuItem("Tools/BLOCK GAME/Level 27/Setup Bridge Buttons")]
    public static void Setup()
    {
        GameObject root = PrefabUtility.LoadPrefabContents(Level27PrefabPath);
        try
        {
            SwitchWithTwoBridgeButton[] switches = root.GetComponentsInChildren<SwitchWithTwoBridgeButton>(true);
            SwitchWithTwoBridgeButton leftSwitch = switches.FirstOrDefault(s => InstanceName(s) == LeftSwitchName);
            SwitchWithTwoBridgeButton rightSwitch = switches.FirstOrDefault(s => InstanceName(s) == RightSwitchName);
            BridgeButtonTwo crossButton = root.GetComponentsInChildren<BridgeButtonTwo>(true).FirstOrDefault(b => InstanceName(b) == CrossButtonName);

            if (leftSwitch == null || rightSwitch == null || crossButton == null)
            {
                EditorUtility.DisplayDialog("Level 27", "Nie znaleziono w prefabie Level 27 obiektów:\n" +
                    $"{LeftSwitchName}: {(leftSwitch != null ? "OK" : "BRAK")}\n" +
                    $"{RightSwitchName}: {(rightSwitch != null ? "OK" : "BRAK")}\n" +
                    $"{CrossButtonName}: {(crossButton != null ? "OK" : "BRAK")}", "OK");
                return;
            }

            Level27Bridge leftBridge = AddBridge(leftSwitch);
            Level27Bridge rightBridge = AddBridge(rightSwitch);
            if (leftBridge == null || rightBridge == null)
            {
                EditorUtility.DisplayDialog("Level 27", "Przycisk kółka nie ma przypiętego mostu (pole Bridge).", "OK");
                return;
            }

            SetupCircleButton(leftSwitch, leftBridge);
            SetupCircleButton(rightSwitch, rightBridge);
            SetupCrossButton(crossButton, leftBridge, rightBridge);

            PrefabUtility.SaveAsPrefabAsset(root, Level27PrefabPath);
            Debug.Log("[Level 27] Podpięto: lewe kółko -> lewy most, prawe kółko -> prawy most, X -> oba mosty.");
        }
        finally
        {
            PrefabUtility.UnloadPrefabContents(root);
        }
    }

    private static string InstanceName(Component component)
    {
        GameObject instanceRoot = PrefabUtility.GetNearestPrefabInstanceRoot(component.gameObject);
        return (instanceRoot != null ? instanceRoot : component.gameObject).name.Trim();
    }

    private static Level27Bridge AddBridge(SwitchWithTwoBridgeButton switchButton)
    {
        GameObject bridge = new SerializedObject(switchButton).FindProperty("bridge").objectReferenceValue as GameObject;
        if (bridge == null) return null;

        return bridge.TryGetComponent(out Level27Bridge existing) ? existing : bridge.AddComponent<Level27Bridge>();
    }

    private static void SetupCircleButton(SwitchWithTwoBridgeButton switchButton, Level27Bridge bridge)
    {
        // Nowy przycisk musi leżeć na obiekcie z triggerem (tam, gdzie stary skrypt).
        GameObject host = switchButton.gameObject;
        if (!host.TryGetComponent(out Level27CircleButton button)) button = host.AddComponent<Level27CircleButton>();
        SetBridges(button, bridge);
        switchButton.enabled = false;
    }

    private static void SetupCrossButton(BridgeButtonTwo crossButton, params Level27Bridge[] bridges)
    {
        GameObject instanceRoot = PrefabUtility.GetNearestPrefabInstanceRoot(crossButton.gameObject);
        if (instanceRoot == null) instanceRoot = crossButton.gameObject;
        if (!instanceRoot.TryGetComponent(out Level27CrossButton button)) button = instanceRoot.AddComponent<Level27CrossButton>();
        SetBridges(button, bridges);

        SerializedObject serialized = new(button);
        serialized.FindProperty("plate").objectReferenceValue = instanceRoot.transform;
        serialized.ApplyModifiedPropertiesWithoutUndo();

        crossButton.enabled = false;
    }

    private static void SetBridges(Component button, params Level27Bridge[] bridges)
    {
        SerializedObject serialized = new(button);
        SerializedProperty list = serialized.FindProperty("bridges");
        list.arraySize = bridges.Length;
        for (int i = 0; i < bridges.Length; i++)
        {
            list.GetArrayElementAtIndex(i).objectReferenceValue = bridges[i];
        }
        serialized.ApplyModifiedPropertiesWithoutUndo();
    }
}
