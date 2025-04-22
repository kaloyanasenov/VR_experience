using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.SceneManagement;


public class GameManager : MonoBehaviour
{
   
   public UnityEngine.XR.Interaction.Toolkit.Interactors.XRSocketInteractor socket;
   public Light targetLight; // Assign your in-game Light here
   public TMPro.TextMeshProUGUI debugText;
   public GameObject[] uiToDisable; // Assign other UI elements (not the completion menu)
   public GameObject completionMenu; // Assign the CompletionMenu panel here
   public ParticleSystem confettiFX;

    private bool panelPlaced = false;
    private bool hasLogged = false;
    private bool completionTriggered = false;
    private bool hasWrongObjectLogged = false;

 void Update()
    {
        // Light check
        bool lightIsOff = targetLight != null && !targetLight.enabled;

        if (lightIsOff && !hasLogged)
        {
            debugText.text += "✅ Light is OFF\n";
            hasLogged = true;
        }
        else if (!lightIsOff && hasLogged)
        {
            debugText.text += "💡 Light turned back ON\n";
            hasLogged = false;
        }

        // Panel placement check
        if (socket == null || debugText == null) return;

if (!panelPlaced && socket.hasSelection)
{
    var interactable = socket.firstInteractableSelected;
    if (interactable != null)
    {
        if (interactable.transform.name == "SolarPanel")
        {
            debugText.text += "🔌 Solar panel placed in socket!\n";
            panelPlaced = true;
            hasWrongObjectLogged = false; // Reset in case something wrong was there before
        }
        else if (!hasWrongObjectLogged)
        {
            debugText.text += "⚠️ Wrong object placed in socket: " + interactable.transform.name + "\n";
            hasWrongObjectLogged = true;
        }
    }
}
else if (!socket.hasSelection)
{
    if (panelPlaced)
    {
        debugText.text += "⚠️ Solar panel removed from socket.\n";
        panelPlaced = false;
    }

    hasWrongObjectLogged = false; // Reset so we can log again next time
}


        // ✅ Check if both are satisfied and trigger completion
        if (panelPlaced && lightIsOff && !completionTriggered)
        {
            debugText.text += "🎉 Challenge complete! Disabling UI...\n";
            TriggerCompletion();
            completionTriggered = true;
        }
    }

    void TriggerCompletion()
    {
        foreach (GameObject ui in uiToDisable)
        {
            if (ui != null)
                ui.SetActive(false);
        }

        // Show completion menu
    if (completionMenu != null)
        completionMenu.SetActive(true);
        
         if (confettiFX != null)
    {
        confettiFX.Play();
    }
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void QuitGame()
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
    #else
        Application.Quit();
    #endif
    }

    public void DismissMenu()
    {
        if (completionMenu != null)
            completionMenu.SetActive(false);

        foreach (GameObject ui in uiToDisable)
        {
            if (ui != null)
                ui.SetActive(true); // Re-enable the original UI elements
        }
    }

}
