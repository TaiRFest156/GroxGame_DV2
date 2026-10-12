using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    public GameObject PausePanel;
    public GameObject joystickObject; // MoveJoystick
    public Toggle movementToggle;     // El Toggle llamado "movement"

    void Start()
    {
        // Sincroniza el estado inicial del joystick con el toggle
        if (movementToggle != null)
            ToggleJoystick(movementToggle.isOn);

        // Escucha cambios en tiempo real
        movementToggle.onValueChanged.AddListener(ToggleJoystick);
    }

    public void Pause()
    {
        PausePanel.SetActive(true);
        Time.timeScale = 0;
    }

    public void Continue()
    {
        PausePanel.SetActive(false);
        Time.timeScale = 1;
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void RestartLevel()
    {
        PausePanel.SetActive(false);
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void ToggleJoystick(bool isOn)
    {
        if (joystickObject != null)
            joystickObject.SetActive(isOn);
    }
}
