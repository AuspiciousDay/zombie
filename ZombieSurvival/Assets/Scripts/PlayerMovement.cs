using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public static readonly int HashMove = Animator.StringToHash("Move"); // Hash함수(코드) 매개변수로 넘어오는 값을 인트형으로 넘겨주는 함수, 리턴값으로 매개변수값을 알기 힘들어야함

    public float moveSpeed = 5f;
    public float rotateSpeed = 180f;
    
    private Animator playerAnimator;
    private PlayerInput playerInput;
    private Rigidbody playerRigidbody;

    private void Awake()
    {
        playerAnimator = GetComponent<Animator>();
        playerInput = GetComponent<PlayerInput>();
        playerRigidbody = GetComponent<Rigidbody>();
    }

    private void Update() // 렌더링관련, 가능한 많은 프레임마다 호출
    {
        playerAnimator.SetFloat(HashMove, playerInput.Move);
    }

    private void FixedUpdate() // 물리관련, 고정된 프레임마다 호출
    {
        float angle = playerInput.Rotate * rotateSpeed * Time.deltaTime;
        playerRigidbody.MoveRotation(playerRigidbody.rotation * Quaternion.Euler(0f,  angle, 0f));

        Vector3 delta = playerInput.Move * transform.forward * moveSpeed * Time.deltaTime; //사용자 입력값(입력을 받을때만 움직이도록), 방향, 속도, 시간
        playerRigidbody.MovePosition(playerRigidbody.position + delta);
    }
}
