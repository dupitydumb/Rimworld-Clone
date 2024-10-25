using UnityEngine;

public interface IInteractable
{
    void Interact(Pawns pawns);
    void CancelInteraction();

    Pawns GetWorker();
}