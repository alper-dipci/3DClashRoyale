using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Netcode;

public class LevelManager : MonoBehaviour {
    public static LevelManager instance;
    private void Start()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(instance);
            instance = this;
        }
        //if(SceneManager.GetActiveScene() == SceneManager.GetSceneByName("MainMenuScene"))
        //{
        //    AudioSystem.Instance.PlayMusic(AudioSystem.Instance.mainMenuMusic);
        //}
    }
    // Function to load a scene by name
    public  void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
    public static void LoadScene(SceneNames sceneName)
    {
        SceneManager.LoadScene(sceneName.ToString());
    }
    public static void LoadNetworkScene(SceneNames sceneName)
    {
        NetworkManager.Singleton.SceneManager.LoadScene(sceneName.ToString(), LoadSceneMode.Single);
    }

    // Function to quit the application
    public static void QuitGame()
    {
        Application.Quit();
    }
    
}
public enum SceneNames {
    Lobby,
    MainMenuScene,
    Main
}
