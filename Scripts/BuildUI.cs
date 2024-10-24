using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BuildUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject previewPrefab;
    public BuildType buildType;
    public string wallName;
    private Button button;

    GameManager gameManager;
    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameManager.instance;
        button = GetComponent<Button>();
        button.onClick.AddListener(Build);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Build()
    {
        gameManager.building.isPlacing = true;
        Debug.Log("Setting wall name to " + wallName);
        gameManager.building.SetWallName = "/" + wallName + "/";
        gameManager.building.buildType = buildType;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        gameManager.building.isHoverUI = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        gameManager.building.isHoverUI = false;
    }
}


public enum BuildType
{
    Wall,
    Zone,

    Floor
}
