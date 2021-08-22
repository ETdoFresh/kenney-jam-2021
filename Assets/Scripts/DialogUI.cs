using System;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class DialogUI : MonoBehaviour
{
    [SerializeField] private DialogDB dialogDB;
    [SerializeField] private TextMeshProUGUI textMesh;

    private void OnEnable()
    {
        dialogDB.dialogUI = this;
        Hide();
    }

    private void OnDisable()
    {
        if (dialogDB.dialogUI == this)
            dialogDB.dialogUI = null;
    }

    public void ShowText(string txt)
    {
        Debug.Log(txt);
        if (dialogDB.isShowingText) return;
        textMesh.text = txt;
        ShowUI();
    }

    public void Hide()
    {
        Debug.Log("Hide!");
        HideUI();
        dialogDB.isShowingText = false;
    }

    public void ShowUI()
    {
        dialogDB.isShowingText = true;
        for (var i = 0; i < transform.childCount; i++)
            transform.GetChild(i).gameObject.SetActive(true);
    }

    public void HideUI()
    {
        for (var i = 0; i < transform.childCount; i++)
            transform.GetChild(i).gameObject.SetActive(false);
    }
}