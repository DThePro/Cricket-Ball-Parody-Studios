using Unity.Cinemachine;
using UnityEngine;

public class BallParameters : MonoBehaviour
{
    [Tooltip("Add 4 elements, the first one being the fastest speed and the 4th one being the slowest (no-ball) speed for a swing ball specifically.")]
    public float[] speeds = {9, 8, 7, 6};
    [Tooltip("Spin balls can't have variable speed. The slider determines it's angle of spin.")]
    [Range(4, 10)]
    public float spinBallSpeed = 4;
    [Tooltip("This is the base swing force. A multiplier will act on it based on what speed the ball travels at.")]
    [Range(300, 1000)]
    public float swingForce = 1000;
    [Tooltip("This is the base spin force. Based on the color you choose, a multiplier will act on it, with blue being the highest spin force and red being the lowest.")]
    [Range(1.5f, 3)]
    public float spinForce = 2;

    public static BallParameters Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else Destroy(gameObject);
    }
}
