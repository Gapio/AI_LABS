using UnityEngine;

public class Restart : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public static void RestartLevel()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }

}
