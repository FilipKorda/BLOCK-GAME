using UnityEditor;
using UnityEngine;

// Narzędzia do testowania zapisu postępu (SaveSystem).
// Uwaga: edytor i build tej samej gry na tym samym komputerze używają tego samego pliku zapisu.
public static class SaveMenu
{
    [MenuItem("Tools/BLOCK GAME/Save/Open Save Folder")]
    private static void OpenSaveFolder()
    {
        EditorUtility.RevealInFinder(SaveSystem.FilePath);
    }

    [MenuItem("Tools/BLOCK GAME/Save/Delete Save")]
    private static void DeleteSave()
    {
        if (EditorUtility.DisplayDialog("Delete Save", "Usunąć zapisany postęp gry?\n\n" + SaveSystem.FilePath, "Usuń", "Anuluj"))
        {
            SaveSystem.DeleteSave();
            Debug.Log("[SaveSystem] Zapis usunięty. Zmiany w trwającej grze (Play Mode) wejdą przy następnym zapisie.");
        }
    }
}
