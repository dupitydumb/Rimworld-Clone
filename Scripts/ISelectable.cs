
using UnityEngine;

public interface ISelectable
{
    void Select();
    void Deselect();

    Vector3 GetPosition();
    GameObject GetGameObject();
}
