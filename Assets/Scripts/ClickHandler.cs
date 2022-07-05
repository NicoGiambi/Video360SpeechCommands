using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
public class ClickHandler : MonoBehaviour
{

    public UnityEvent downEvent;
    public UnityEvent upEvent;

    void OnMouseDown() {
        Debug.Log("Down");
        downEvent?.Invoke();
    }

    void OnMouseUp() {
        Debug.Log("Up");
        upEvent?.Invoke();
    }
}
