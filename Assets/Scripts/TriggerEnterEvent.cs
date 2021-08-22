using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class TriggerEnterEvent : MonoBehaviour
{
    public UnityEvent<Collider2D> triggerEnter; 
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        var playerCharacter = other.GetComponent<PlayerCharacter>();
        if (playerCharacter)
            triggerEnter.Invoke(other);
    }

    public void LoadLevel(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}
