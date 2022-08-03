using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu(fileName ="GameController", menuName = "Scriptable Objects/GameController")]
public class GameController : SingletonScriptableObject<GameController>
{
    public Camera CameraUI;
    public Camera Camera3D;

    private void Awake()
    {
        Debug.Log("Awoken");
        FindReferences();
    }
    void OnEnable()
    {
        Debug.Log("On Enable");
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        Debug.Log("On Disable");
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        FindReferences();
    }

    void FindReferences()
    {
        GameObject gO;

        gO = GameObject.FindGameObjectWithTag("3DCamera");
        if (gO != null)
        {
            Camera3D = gO.GetComponent<Camera>();
        }
        gO = GameObject.FindGameObjectWithTag("UICamera");
        if (gO != null)
        {
            CameraUI = gO.GetComponent<Camera>();
        }
    }

}
