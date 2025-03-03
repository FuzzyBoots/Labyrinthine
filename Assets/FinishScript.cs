using UnityEngine;

public class FinishScript : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) {
            GameManager.Instance.WinCondition();
        }
    }
}
