using TMPro;
using UnityEngine;

public class Paddle : MonoBehaviour {
    [SerializeField]
    TextMeshPro scoreText;

    int score;

    [SerializeField]
    bool isAI;

    [SerializeField, Min(0f)]
    float minExtents = 4f, maxExtents = 4f, speed = 10f, maxTargetingBias = 0.75f;

    float extents, targetingBias;

    void Awake() => SetScore(0);


    public bool ScorePoint(int pointsToWin) {
        SetScore(score + 1, pointsToWin);
        return score >= pointsToWin;
    }


    void SetScore(int newScore, float pointsToWin = 1000f) {
        this.score = newScore;
        scoreText.SetText("{0}", score);
        SetExtents(Mathf.Lerp(maxExtents, minExtents, newScore / (pointsToWin - 1f)));
    }



    public void StartNewGame() {
        SetScore(0);
        ChangeTargetingBias();
    }

    public void Move(float target, float arenaExtents) {
        Vector3 p = transform.localPosition;
        p.x = isAI ? AdjustByAI(p.x, target) : AdjustByPlayer(p.x);
        float limit = arenaExtents - extents;
        p.x = Mathf.Clamp(p.x, -limit, limit);
        transform.localPosition = p;
    }

    void SetExtents(float newExtents) {
        extents = newExtents;
        Vector3 s = transform.localScale;
        s.x = 2f * newExtents;
        transform.localScale = s;
    }

    float AdjustByAI(float x, float target) {
        target += targetingBias * extents;
        if (x < target) {
            return Mathf.Min(x + speed * Time.deltaTime, target);
        }
        return Mathf.Max(x - speed * Time.deltaTime, target);
    }

    void ChangeTargetingBias() => targetingBias = Random.Range(-maxTargetingBias, maxTargetingBias);

    float AdjustByPlayer(float x) {
        bool goRight = Input.GetKey(KeyCode.RightArrow);
        bool goLeft = Input.GetKey(KeyCode.LeftArrow);
        if (goRight && !goLeft) {
            return x + speed * Time.deltaTime;
        } else if (goLeft && !goRight) {
            return x - speed * Time.deltaTime;
        }
        return x;
    }

    public bool HitBall(float ballX, float ballExtents, out float hitFactor) {
        ChangeTargetingBias();
        hitFactor = (ballX - transform.localPosition.x) / (ballExtents + extents);
        return -1f <= hitFactor && hitFactor <= 1f;
    }
}
