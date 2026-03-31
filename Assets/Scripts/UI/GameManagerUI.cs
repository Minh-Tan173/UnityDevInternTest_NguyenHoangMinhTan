using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManagerUI : MonoBehaviour
{
    public static GameManagerUI Instance { get; private set; }

    public event EventHandler<OnAutoKickEventArgs> OnAutoKick;
    public class OnAutoKickEventArgs : EventArgs {

        public SoccerBall farthestBall;
        public Vector2 targetPos;
    }

    public event EventHandler OnKick;

    [Header("Radar")]
    [SerializeField] private BoxCollider radarBox;
    [SerializeField] private LayerMask ballLayer;
    [SerializeField] private LayerMask goalLayer;

    [Header("Button")]
    [SerializeField] private Button autoKickButton;
    [SerializeField] private Button kickButton;
    [SerializeField] private Button resetButton;

    private List<SoccerGoal> soccerGoalList;
    private List<SoccerBall> soccerBallList;

    private void Awake() {

        Instance = this;

        soccerGoalList = new List<SoccerGoal>();
        soccerBallList = new List<SoccerBall>();

        // PREPARED DATA

        Collider[] goalArray = GetScanResultByLayer(goalLayer);
        foreach (Collider goal in goalArray) {

            if (goal.TryGetComponent<SoccerGoal>(out SoccerGoal soccerGoal)) {

                soccerGoalList.Add(soccerGoal);
            }
        }

        Collider[] ballArray = GetScanResultByLayer(ballLayer);
        foreach (Collider ball in ballArray) {

            if (ball.TryGetComponent<SoccerBall>(out SoccerBall soccerBall)) {

                soccerBallList.Add(soccerBall);
            }
        }

        // BUTTON
        autoKickButton.onClick.AddListener(CommandMoveFarestBall);

        kickButton.onClick.AddListener(() => {

            OnKick?.Invoke(this, EventArgs.Empty);

        });

        resetButton.onClick.AddListener(() => {

            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        });

    }

    private void Start() {

        Player.Instance.ShowKickButton += Player_ShowKickButton;
        Player.Instance.HideKickButton += Player_HideKickButton;

        // When start scene
        HideKickButton();
    }

    private void OnDestroy() {

        Player.Instance.ShowKickButton -= Player_ShowKickButton;
        Player.Instance.HideKickButton -= Player_HideKickButton;
    }

    private void CommandMoveFarestBall() {

        float farestPlayerDis = -Mathf.Infinity;
        SoccerBall farthestBall = null; 

        // Find farthest ball
        Vector3 playerPos = Player.Instance.transform.position;

        // Start scan full field
        foreach (SoccerBall currentBall in soccerBallList) {

            float sqrDistanceToPlayer = (currentBall.transform.position - playerPos).sqrMagnitude;

            if (sqrDistanceToPlayer > farestPlayerDis) {

                farestPlayerDis = sqrDistanceToPlayer;
                farthestBall = currentBall;
            }

        }

        // After having farthest ball
        OnAutoKick?.Invoke(this, new OnAutoKickEventArgs { farthestBall = farthestBall });

    }

    private Collider[] GetScanResultByLayer(LayerMask scanLayer) {

        Vector3 halfExtends = radarBox.size / 2f;
        Quaternion orientation = radarBox.transform.rotation;

        return Physics.OverlapBox(radarBox.transform.position, halfExtends, orientation, scanLayer);
    }

    private void Player_HideKickButton(object sender, EventArgs e) {

        HideKickButton();
    }

    private void Player_ShowKickButton(object sender, EventArgs e) {

        ShowKickButton();
    }

    private void ShowKickButton() {
        kickButton.gameObject.SetActive(true);
    }

    private void HideKickButton() {
        kickButton.gameObject.SetActive(false); 
    }

    public List<SoccerGoal> GetSoccerGoalList() {
        return this.soccerGoalList;
    }
}
