using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class PlayerManager : MonoBehaviour
{
    public float movespeed;
    public float sensitivity;
    public float gravity;
    private float mouseX;
    private float mouseY;
    private float vertical;
    private float horizontal;

    public Vector2 clampangle;
    private Vector3 Velocity;
    private Vector2 angle;
    public Transform cameraTransform;
    private Joystick joystick;
    private FixedScreen fixedScreen;
    public PhotonView photonView; 

    public CharacterController charactercontroller;

    private void Awake()
    {
        joystick = GameObject.Find("FloatingJoystick").GetComponent<FloatingJoystick>();
        fixedScreen = GameObject.Find("SensorPanel").GetComponent<FixedScreen>();
        photonView = gameObject.GetComponent<PhotonView>();
        gameObject.name = photonView.Owner.NickName;
        if (!photonView.IsMine)
        {
            GameObject.Find(gameObject.name + "/Camera").SetActive(false);
        }
    }

    private void LateUpdate()
    {
        if (photonView.IsMine)
        {
            vertical = joystick.Vertical;
            horizontal = joystick.Horizontal;

            Vector3 playerMovementInput = new Vector3(horizontal, 0.0f, vertical);
            Vector3 moveVector = transform.TransformDirection(playerMovementInput);

            if (charactercontroller.isGrounded)
            {
                Velocity.y = -1f;
            }
            else
            {
                Velocity.y -= gravity * Time.deltaTime;
            }

            charactercontroller.Move(moveVector * movespeed * Time.deltaTime);
            charactercontroller.Move(Velocity * Time.deltaTime);

            mouseX = fixedScreen.TouchDist.x / 20;
            mouseY = fixedScreen.TouchDist.y / 20;

            angle.x -= mouseY * sensitivity;
            angle.y -= mouseX * -sensitivity;

            angle.x = Mathf.Clamp(angle.x, -clampangle.x, clampangle.y);

            Quaternion rotation = Quaternion.Euler(angle.x, angle.y, 0.0f);
            Quaternion rotationTwo = Quaternion.Euler(0.0f, angle.y, 0.0f);
            transform.rotation = rotationTwo;
            cameraTransform.rotation = rotation;

            //if (gameObject.transform.position.y < -4.0f)
            //{
            //    gameObject.transform.position = new Vector3(10.0f, 10.2f, 1.6f);
            //}
        }
    }
}