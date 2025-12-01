using UnityEngine;
using UnityEngine.SceneManagement;  

public class ButtonSceneTransition : MonoBehaviour
{
    
    public void OnHomeButtonClick()
    {
        SceneManager.LoadScene("Start_Menu");  
    }
}
