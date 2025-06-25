using UnityEngine;

public class Gates : MonoBehaviour
{
    [field: SerializeField]
    public float Score { get; private set; }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Ball>() != null)
        {
            Destroy(other.gameObject);
            Score += 10;
            Debug.Log("Goal! Score: " + Score.ToString());
        }
    }
}