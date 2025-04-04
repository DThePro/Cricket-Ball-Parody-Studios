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
    [HideInInspector]
    public Vector3 dropLocation;
    public GameObject ConfirmCanvas, BowlingCanvas;
    public CinemachineCamera confirmCam, bowlingCam;
    [SerializeField] TextMeshProUGUI bowlTypeText, sideText, swingText;
    [SerializeField] CanvasGroup outPanel;

    private Rigidbody rb;
    private float speed;
    private bool hasHitGround = false;
    private bool left = false, onlyOnce = false;
    private int level;
    private float spinBallSpeed = 6f, swingForce = 0.5f, spinForce = 0.5f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        spinBallSpeed = BallParameters.Instance.spinBallSpeed;
        swingForce = BallParameters.Instance.swingForce;
        spinForce = BallParameters.Instance.spinForce;
    }

    public void SetSpeedAndBowl(Vector3 trajectory)
    {
        speed = (bowlType == BowlType.SwingBall ? trajectory.x : spinBallSpeed);
        hasHitGround = false;
        level = (int)trajectory.z;
        BowlBall(trajectory.y);
    }

    void BowlBall(float delta)
    {
        rb.isKinematic = false;
        Vector3 p0 = transform.position;
        Vector3 horizontalDiff = new(dropLocation.x - p0.x, 0f, dropLocation.z - p0.z);
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

        if (bowlType == BowlType.SwingBall)
        {
            float deltaDirection;
            /*
            float deltaDirection = speed switch
            {
                9 => 3.45f,
                8 => 3.2f,
                7 => 2.85f,
                6 => 2.45f,
                _ => 10f
            };
            */
            deltaDirection = ((-Mathf.Pow(speed, 3) + 18 * speed * speed - 59 * speed + 215) / 120f) * swingForce / 300;
            Quaternion rotation = Quaternion.AngleAxis(deltaDirection * (left ? -1 : 1), Vector3.up);
            launchDir = rotation * launchDir;
        }
        launchDir.Normalize();
        rb.linearVelocity = launchDir * v0;

        if (bowlType == BowlType.SwingBall)
        {
            StartCoroutine(ApplySwingForce());
        }
        StartCoroutine(AfterBowlActions());
    }

    private IEnumerator ApplySwingForce()
    {
        float force;
        /*
        float force = speed switch
        {
            9 => swingForce * 1.7f,
            8 => swingForce * 1.5f,
            7 => swingForce * 1f,
            6 => swingForce * 0.7f,
            _ => swingForce
        };
        */
        float a;
        if (speed <= 7) a = 0.7f + 0.3f * (speed - 6);
        else if (speed >= 7 && speed <= 8) a = 1 + 0.5f * (speed - 7);
        else a = 1.5f + 0.2f * (speed - 8);
        force = swingForce * a;
        // force = swingForce * ((-Mathf.Pow(speed, 3) * 5 + 111 * speed * speed - 790 * speed + 1866) / 60f);
        while (!hasHitGround)
        {
            rb.AddForce((left ? 1 : -1) * force * Time.deltaTime * Vector3.right, ForceMode.Force);
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
                float force = level switch
                {
                    0 => spinForce * 1.6f,
                    1 => spinForce * 1.1f,
                    2 => spinForce * 0.9f,
                    3 => spinForce * 0.7f, 
                    _ => 10f
                };
                rb.AddForce((left ? 1 : -1) * force * Vector3.right, ForceMode.Impulse);
            }
        }

        if (collision.gameObject.CompareTag("Wicket") && !onlyOnce)
        {
            StartCoroutine(Out());
            onlyOnce = true;
        }
    }

    IEnumerator Out()
    {
        float duration = 0.5f;

        // Fade in
        for (float t = 0; t < duration; t += Time.deltaTime)
        {
            outPanel.alpha = t / duration;
            yield return null;
        }
        outPanel.alpha = 1f;

        // Stay visible for 2 seconds
        yield return new WaitForSeconds(2f);

        // Fade out
        for (float t = 0; t < duration; t += Time.deltaTime)
        {
            outPanel.alpha = 1 - (t / duration);
            yield return null;
        }
        outPanel.alpha = 0f;
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