using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SpeedController : MonoBehaviour
{
    public RectTransform arrow;
    public BowlingController bowlingScript;
    public CanvasGroup noBallCanvasGroup;

    public float moveSpeed = 2f;
    public Vector2 posA, posB;
    private bool movingUp = true;

    // Speed Ranges
    public float pos1a, pos1b, pos2a, pos2b, pos2c, pos2d;
    public float pos3a, pos3b, pos3c, pos3d, pos4a, pos4b, pos4c, pos4d;

    public bool ballBowled = false;

    void Update()
    {
        if (!ballBowled)
            MoveArrow();
    }

    void MoveArrow()
    {
        float t = Mathf.PingPong(Time.time * moveSpeed, 1f);
        arrow.anchoredPosition = Vector2.Lerp(posA, posB, t);
    }

    public void BowlBall()
    {
        float arrowY = arrow.anchoredPosition.y;
        Vector2 trajectory = new Vector2(3.5f, -0.01f);
        // float speed = 3.5f; // Default to lowest speed

        if (arrowY <= pos1a && arrowY >= pos1b) trajectory = new Vector2(9f, -0.04f);
        else if ((arrowY <= pos2a && arrowY >= pos2b) || (arrowY <= pos2c && arrowY >= pos2d)) trajectory = new Vector2(8f, -0.01f);
        else if ((arrowY <= pos3a && arrowY >= pos3b) || (arrowY <= pos3c && arrowY >= pos3d)) trajectory = new Vector2(7f, 0.01f);
        else if ((arrowY <= pos4a && arrowY >= pos4b) || (arrowY <= pos4c && arrowY >= pos4d))
        {
            trajectory = new Vector2(6f, 0.03f);
            StartCoroutine(NoBall(noBallCanvasGroup));
        }

        bowlingScript.SetSpeedAndBowl(trajectory);
        ballBowled = true;
    }

    IEnumerator NoBall(CanvasGroup canvasGroup)
    {
        float duration = 0.5f;

        // Fade in
        for (float t = 0; t < duration; t += Time.deltaTime)
        {
            canvasGroup.alpha = t / duration;
            yield return null;
        }
        canvasGroup.alpha = 1f;

        // Stay visible for 2 seconds
        yield return new WaitForSeconds(2f);

        // Fade out
        for (float t = 0; t < duration; t += Time.deltaTime)
        {
            canvasGroup.alpha = 1 - (t / duration);
            yield return null;
        }
        canvasGroup.alpha = 0f;
    }

}
