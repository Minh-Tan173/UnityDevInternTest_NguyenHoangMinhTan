using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoccerGoal : MonoBehaviour
{
    public static event EventHandler OnResetCamFollowAtPlayer;

    public event EventHandler OnPlayPartical;

    private Coroutine currentCoroutine;
    private SoccerBall soccerBall;

    private void OnCollisionEnter(Collision collision) {

        if (collision.transform.TryGetComponent<SoccerBall>(out SoccerBall soccerBall)) {

            if (this.soccerBall == soccerBall) {
                // Ensure each ball can only be collided with once before the next shot

                return;
            }

            this.soccerBall = soccerBall;

            if (currentCoroutine != null) {
                StopCoroutine(currentCoroutine);
                currentCoroutine = null;
            }

            currentCoroutine = StartCoroutine(OnGoalCoroutine());
        }
    }

    private IEnumerator OnGoalCoroutine() {

        float waitTimer = 2f;

        OnPlayPartical?.Invoke(this, EventArgs.Empty);

        yield return new WaitForSeconds(waitTimer);

        OnResetCamFollowAtPlayer?.Invoke(this, EventArgs.Empty);

    }

}
