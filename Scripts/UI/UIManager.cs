using UnityEngine;

public class UIManager : MonoBehaviour
{

    public GameObject[] tabs;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            CloseAllTabs();
            GameManager.instance.building.isPlacing = false;
        }
    }


    public void OpenTab(int tab)
    {
        for (int i = 0; i < tabs.Length; i++)
        {
            if (i == tab)
            {
                tabs[i].SetActive(true);
                if (i == 1)
                {
                    GameManager.instance.building.isPlacing = true;
                }
            }
            else
            {
                tabs[i].SetActive(false);
            }
        }
    }

    public void CloseAllTabs()
    {
        for (int i = 0; i < tabs.Length; i++)
        {
            tabs[i].SetActive(false);
        }
    }
}
