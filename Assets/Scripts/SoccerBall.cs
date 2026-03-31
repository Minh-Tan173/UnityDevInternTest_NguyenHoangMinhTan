using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoccerBall : MonoBehaviour
{
    [SerializeField] private Transform camTargetPoint;

    private Rigidbody rbBall;

    private void Awake() {

        rbBall = GetComponent<Rigidbody>();
    }

    private void Start() {

        rbBall.isKinematic = true;

        GameManagerUI.Instance.OnAutoKick += KickControlUI_OnAutoKick;
        GameManagerUI.Instance.OnKick += KickControlUI_OnKick;
    }

    private void OnDestroy() {

        GameManagerUI.Instance.OnAutoKick -= KickControlUI_OnAutoKick;
        GameManagerUI.Instance.OnKick -= KickControlUI_OnKick;
    }

    private void KickControlUI_OnKick(object sender, System.EventArgs e) {
        
        if (this == Player.Instance.GetSelectedBall()) {
            // If Player is selected this ball

            Kicked(FindNearestGoal().transform.position);

        }
    }

    private void KickControlUI_OnAutoKick(object sender, GameManagerUI.OnAutoKickEventArgs e) {
        
        if (e.farthestBall == this) {
            // If this is the farthest ball from the player 

            Kicked(FindNearestGoal().transform.position);
        }
    }

    private void Update() {

        camTargetPoint.rotation = Quaternion.identity;
    }

    private SoccerGoal FindNearestGoal() {

        float minDistanceToGoal = Mathf.Infinity;
        SoccerGoal goalTarget = null;   

        List<SoccerGoal> soccerGoalList = GameManagerUI.Instance.GetSoccerGoalList();

        foreach(SoccerGoal soccerGoal in soccerGoalList) {

            float sqrDistanceToGoal = (this.transform.position - soccerGoal.transform.position).sqrMagnitude;

            if (sqrDistanceToGoal <= minDistanceToGoal) {

                minDistanceToGoal = sqrDistanceToGoal;
                goalTarget = soccerGoal;
            }

        }


        return goalTarget;
    }

    public void Kicked(Vector3 targetPos) {

        rbBall.isKinematic = false;

        rbBall.velocity = Vector3.zero;
        rbBall.angularVelocity = Vector3.zero;

        Vector3 direction = targetPos - transform.position;
        float distance = direction.magnitude;

        float horizontalPower = 20f;
        float upPower = 5f; 

        Vector3 forceVector = direction.normalized * horizontalPower;
        forceVector.y = upPower;

        rbBall.AddForce(forceVector, ForceMode.VelocityChange);

        //rb.AddTorque(transform.right * 20f, ForceMode.Impulse);
    }

    public Transform GetCamtargetPoint() {
        return this.camTargetPoint;
    }
}
