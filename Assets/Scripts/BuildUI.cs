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
    // Start is called before the first frame update
    void Start()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(Build);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Build()
    {
        GameManager.instance.isPlacing = true;
        Debug.Log("Setting wall name to " + wallName);
        GameManager.instance.SetWallName = "/" + wallName + "/";
        GameManager.instance.buildType = buildType;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        GameManager.instance.isHoverUI = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        GameManager.instance.isHoverUI = false;
    }
}


public enum BuildType
{
    Wall,
    Zone,

    Floor
}
