using UnityEngine;

public class FragmentsScript : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        GamePlayScript gamePlay = other.GetComponent<GamePlayScript>();

        if (gamePlay != null) 
        {
            gamePlay.FragmentCollected();
            Destroy(gameObject);
        }
    }
}
