using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private bool _canRestart = false;

    [SerializeField] Canvas _loseText;
    [SerializeField] Canvas _winText;

    private void Awake()
    {
        // If there is an instance, and it's not me, delete myself.

        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    private void Update()
    {
        if (_canRestart && Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(0);
        }
    }

    public void WinCondition()
    {
        _winText.enabled = true;

        _canRestart = true;
    }

    public void LoseCondition()
    {
        _loseText.enabled = true;

        Debug.Log("Loss");

        _canRestart = true;
    }
}
