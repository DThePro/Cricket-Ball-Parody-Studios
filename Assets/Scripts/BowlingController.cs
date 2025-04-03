using TMPro;
using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine.UIElements;

public class BowlingController : MonoBehaviour
{
    public enum BowlType { SpinBall, SwingBall }
    public BowlType bowlType;
    public float spinBallSpeed = 6f, swingForce = 0.5f, spinForce = 0.5f;
    public Vector3 dropLocation;
    public GameObject ConfirmCanvas, BowlingCanvas;
    public CinemachineCamera confirmCam, bowlingCam;
    [SerializeField] TextMeshProUGUI bowlTypeText, sideText, swingText;

    private Rigidbody rb;
    private float speed;
    private bool hasHitGround = false;
    private bool left = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void SetSpeedAndBowl(Vector2 trajectory)
    {
        speed = (bowlType == BowlType.SwingBall ? trajectory.x : spinBallSpeed);
        hasHitGround = false;
        BowlBall(trajectory.y);
    }

    void BowlBall(float delta)
    {
        rb.isKinematic = false;
        Vector3 p0 = transform.position;
        Vector3 horizontalDiff = new Vector3(dropLocation.x - p0.x, 0f, dropLocation.z - p0.z);
        float d_h = horizontalDiff.magnitude;
        Vector3 horizontalDir = (horizontalDiff.sqrMagnitude > 0.0001f) ? horizontalDiff.normalized : transform.forward;
        float deltaY = dropLocation.y - p0.y;
        float v0 = speed * 10;
        float g = Mathf.Abs(Physics.gravity.y);
        float Acoef = (g * d_h * d_h) / (2 * v0 * v0);
        float Bcoef = -d_h;
        float Ccoef = deltaY + Acoef;
        float discriminant = Bcoef * Bcoef - 4 * Acoef * Ccoef;
        if (discriminant < 0)
        {
            Debug.LogWarning("No real solution for vertical angle; check speed and dropLocation.");
            discriminant = 0;
        }
        float sqrtDisc = Mathf.Sqrt(discriminant);
        float u = (-Bcoef - sqrtDisc) / (2 * Acoef);
        float alpha = Mathf.Atan(u);
        Vector3 launchDir = horizontalDir * Mathf.Cos(alpha) + Vector3.up * Mathf.Sin(alpha);
        launchDir.Normalize();

        if (bowlType == BowlType.SwingBall)
        {
            float deltaDirection = speed switch
            {
                9 => 3.45f,
                8 => 3.2f,
                7 => 2.85f,
                6 => 2.45f,
                _ => 10f
            };
            Quaternion rotation = Quaternion.AngleAxis(deltaDirection * (left ? -1 : 1), Vector3.up);
            launchDir = rotation * launchDir;
        }
        rb.linearVelocity = launchDir * v0;

        if (bowlType == BowlType.SwingBall)
        {
            StartCoroutine(ApplySwingForce());
        }
        StartCoroutine(AfterBowlActions());
    }

    private IEnumerator ApplySwingForce()
    {
        float force = speed switch
        {
            9 => swingForce * 1.7f,
            8 => swingForce * 1.5f,
            7 => swingForce * 1f,
            6 => swingForce * 0.7f,
            _ => swingForce
        };
        while (!hasHitGround)
        {
            rb.AddForce(Vector3.right * force * Time.deltaTime * (left ? 1 : -1), ForceMode.Force);
            yield return null;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!hasHitGround)
        {
            hasHitGround = true;
            if (bowlType == BowlType.SpinBall)
            {
                float force = speed switch
                {
                    9 => spinForce * 1.6f,
                    8 => spinForce * 1.1f,
                    7 => spinForce * 0.9f,
                    6 => spinForce * 0.7f,
                    _ => 0f
                };
                rb.AddForce(Vector3.right * force * (left ? 1 : -1), ForceMode.Impulse);
            }
        }
    }

    public void SwitchBallType()
    {
        bowlType = (bowlType == BowlType.SpinBall) ? BowlType.SwingBall : BowlType.SpinBall;
        bowlTypeText.text = bowlType == BowlType.SpinBall ? "Spin Ball" : "Swing Ball";
    }

    public void SwitchSide()
    {
        transform.position = new Vector3(-transform.position.x, transform.position.y, transform.position.z);
        sideText.text = transform.position.x > 0 ? "Right Side" : "Left Side";
    }

    public void SwitchSwing()
    {
        left = !left;
        swingText.text = left ? "Right Swing" : "Left Swing";
    }

    private IEnumerator AfterBowlActions()
    {
        yield return new WaitForSeconds(4f);
        ConfirmCanvas.SetActive(true);
        BowlingCanvas.SetActive(false);
        confirmCam.Priority = 1;
        bowlingCam.Priority = 0;

        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}