using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    private CinemachineVirtualCamera cinemachine;

    private void Awake() {

        cinemachine = GetComponent<CinemachineVirtualCamera>();
    }

    private void Start() {

        GameManagerUI.Instance.OnAutoKick += GameManagerUI_OnAutoKick;
        GameManagerUI.Instance.OnKick += KickControlUI_OnKick;

        SoccerGoal.OnResetCamFollowAtPlayer += SoccerGoal_OnResetCamFollowAtPlayer;

        // When start scene 
        SetCameraFollowTo(Player.Instance.transform);
    }

    private void OnDestroy() {

        GameManagerUI.Instance.OnAutoKick -= GameManagerUI_OnAutoKick;
        GameManagerUI.Instance.OnKick -= KickControlUI_OnKick;

        SoccerGoal.OnResetCamFollowAtPlayer -= SoccerGoal_OnResetCamFollowAtPlayer;
    }

    private void SoccerGoal_OnResetCamFollowAtPlayer(object sender, System.EventArgs e) {

        SetCameraFollowTo(Player.Instance.transform);
    }

    private void GameManagerUI_OnAutoKick(object sender, GameManagerUI.OnAutoKickEventArgs e) {

        SoccerBall followBall = e.farthestBall;

        SetCameraFollowTo(followBall.GetCamtargetPoint());
    }

    private void KickControlUI_OnKick(object sender, System.EventArgs e) {

        SoccerBall followBall = Player.Instance.GetSelectedBall();

        SetCameraFollowTo(followBall.GetCamtargetPoint());
    }

    private void SetCameraFollowTo(Transform objectFollow) {

        cinemachine.Follow = objectFollow;
        cinemachine.LookAt = objectFollow;
    }
}
