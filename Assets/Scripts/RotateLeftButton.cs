using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class RotateLeftButton : MonoBehaviour
{
    [SerializeField] private Button button;

    public void AddListener(UnityAction call)
    {
#if UNITY_EDITOR
        UnityEditor.Events.UnityEventTools.AddPersistentListener(button.onClick, call);
#else
        button.onClick.AddListener(call);
#endif
    }

    public void RemoveListener(UnityAction call)
    {
        if (!button) return;
#if UNITY_EDITOR
        UnityEditor.Events.UnityEventTools.RemovePersistentListener(button.onClick, call);
#else
        button.onClick.RemoveListener(call);
#endif
    }
}