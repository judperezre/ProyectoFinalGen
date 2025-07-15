using UnityEngine;

public class GamePlayScript : MonoBehaviour
{
    public int numberOfFragments { get; private set; }
    public GameObject flower;

    public void FragmentCollected() 
    {
        numberOfFragments++;

        if (numberOfFragments == 3) 
        {
            flower.SetActive(true);
        }
    }

}
