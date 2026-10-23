using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    public static readonly string MoveAixs = "Vertical";
    public static readonly string RotateAixs = "Horizontal";
    public static readonly string FireAixs = "Fire1";
    public static readonly string ReloadAixs = "Reload";

    public float Move {get; private set;}
    public float Rotate {get; private set;}
    public bool Fire {get; private set;}
    public bool Reload {get; private set;}
    
    private void Update()
    {
        Move = Input.GetAxis(MoveAixs);
        Rotate = Input.GetAxis(RotateAixs);
        Fire = Input.GetButton(FireAixs);
        Reload = Input.GetButtonDown(ReloadAixs);

        // Debug.Log($"{Move} / {Rotate} / {Fire} / {Reload}");
    }
}
