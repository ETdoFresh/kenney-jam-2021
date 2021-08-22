using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Create DialogDB", fileName = "DialogDB", order = 0)]
public class DialogDB : ScriptableObject
{
    [Multiline]
    public List<string> values;

    public DialogUI dialogUI;
    public bool isShowingText;

    public void ShowDialog(int i) => dialogUI?.ShowText(values[i]);
    public void HideDialog() => dialogUI?.Hide();
}