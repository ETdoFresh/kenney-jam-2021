using UnityEngine;
using UnityEngine.InputSystem;

public class ReadDialog : MonoBehaviour
{
    [SerializeField] private DialogDB dialogDB;
    [SerializeField] private int dialogIndex;
    [SerializeField] private bool isPlayerInRage;
    private Controls _controls;

    private void OnEnable()
    {
        if (_controls == null) _controls = new Controls();
        _controls.Enable();
        _controls.Gameplay.Interact.performed += OnInteract;
    }

    private void OnDisable()
    {
        _controls.Disable();
        _controls.Gameplay.Interact.performed -= OnInteract;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        var playerCharacter = other.GetComponent<PlayerCharacter>();
        if (playerCharacter)
            isPlayerInRage = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        var playerCharacter = other.GetComponent<PlayerCharacter>();
        if (playerCharacter)
            isPlayerInRage = false;
    }

    private void OnInteract(InputAction.CallbackContext obj)
    {
        if (isPlayerInRage && !dialogDB.isShowingText)
        {
            dialogDB.ShowDialog(dialogIndex);
        }
        else if (isPlayerInRage)
        {
            dialogDB.HideDialog();
        }
    }
}
