using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public static Player Instance { get; private set; }

    [SerializeField] private PlayerSO playerSO;

    public event EventHandler ShowKickButton;
    public event EventHandler HideKickButton;

    private Rigidbody rbPlayer;
    private BoxCollider colliderPlayer;

    private bool canMove;
    private bool isWalking;
    private bool isRunning;
    private float currentSpeed;

    private float playerHeight;
    private Vector3 moveDir;

    private SoccerBall selectedBall;

    private Collider[] ballDetectedArray;

    private void Awake() {

        Instance = this;

        colliderPlayer = GetComponent<BoxCollider>();
        rbPlayer = GetComponent<Rigidbody>();

        playerHeight = colliderPlayer.size.y;

        ballDetectedArray = new Collider[10];
    }

    private void Start() {

        GameInput.Instance.OnRunning += GameInput_OnRunning;
        GameInput.Instance.UnRunning += GameInput_UnRunning;

        // When start scene
        currentSpeed = playerSO.walkSpeed;
    }

    private void OnDestroy() {

        GameInput.Instance.OnRunning -= GameInput_OnRunning;
        GameInput.Instance.UnRunning -= GameInput_UnRunning;
    }

    private void GameInput_UnRunning(object sender, System.EventArgs e) {
        
        if (isRunning) {

            currentSpeed = playerSO.walkSpeed;
            isRunning = false;
        }
    }

    private void GameInput_OnRunning(object sender, System.EventArgs e) {
        
        if (!isRunning) {

            currentSpeed = playerSO.runSpeed;
            isRunning = true;
        }
    }

    private void Update() {

        // Move
        Vector2 inputVector = GameInput.Instance.GetInputVectorNormalized();
        this.moveDir = new Vector3(inputVector.x, 0f, inputVector.y);

        isWalking = moveDir != Vector3.zero;

        float radius = 0.4f;
        float maxDistance = 0.8f;
        canMove = !Physics.CapsuleCast(this.transform.position, this.transform.position + Vector3.up * playerHeight, radius, moveDir, maxDistance, playerSO.boundaryLayer);

        if (!canMove) {

            // Try move at X-asis
            Vector3 moveDirX = new Vector3(moveDir.x, 0f, 0f).normalized;
            canMove = moveDirX.x != 0f && !Physics.CapsuleCast(this.transform.position, this.transform.position + Vector3.up * playerHeight, radius, moveDirX, maxDistance);

            if (canMove) {

                this.moveDir = moveDirX;
            }
            else {
                // Try move at Z-asis

                Vector3 moveDirZ = new Vector3(0f, 0f, moveDir.z).normalized;
                canMove = moveDirZ.z != 0f && !Physics.CapsuleCast(this.transform.position, this.transform.position + Vector3.up * playerHeight, radius, moveDirZ, maxDistance);

                if (canMove) {

                    this.moveDir = moveDirZ;
                }
            }
        }


        //// Rotate
        //transform.forward = Vector3.Slerp(transform.forward, moveDir, playerSO.rotateSpeed * Time.deltaTime);


        HandleInteractions();
    }

    private void FixedUpdate() {

        if (!canMove || moveDir == Vector3.zero) {

            rbPlayer.velocity = new Vector3(0, rbPlayer.velocity.y, 0);
        }
        else {

            rbPlayer.velocity = new Vector3(moveDir.x * currentSpeed, rbPlayer.velocity.y, moveDir.z * currentSpeed);

            Quaternion targetRotation = Quaternion.LookRotation(moveDir);
            Quaternion nextRotation = Quaternion.Slerp(rbPlayer.rotation, targetRotation, playerSO.rotateSpeed * Time.fixedDeltaTime);
            rbPlayer.MoveRotation(nextRotation);
        }
    }

    private void HandleInteractions() {


        float offset = 0.1f;
        Vector3 halfExtents = new Vector3 (colliderPlayer.size.x / 2f + offset, colliderPlayer.size.y / 2f, colliderPlayer.size.z + offset);
        Quaternion orientation = transform.rotation;
        
        ballDetectedArray = Physics.OverlapBox(colliderPlayer.bounds.center, halfExtents, orientation, playerSO.ballLayer);

        if (ballDetectedArray.Length > 0) {

            float minDistanceToBall = Mathf.Infinity;
            Collider nearestBall = null;

            foreach (Collider ball in ballDetectedArray) {

                float sqrDistance = (this.transform.position - ball.transform.position).sqrMagnitude;

                if (sqrDistance <= minDistanceToBall) {

                    minDistanceToBall = sqrDistance;
                    nearestBall = ball;
                }
            }

            if (nearestBall.transform.TryGetComponent<SoccerBall>(out SoccerBall selectedBall)) {

                if (this.selectedBall != selectedBall) {

                    SetSelectedBall(selectedBall);
                }
            }
            else {

                SetSelectedBall(null);
            }

        }
        else {
            SetSelectedBall(null);
        }
    }

    private void SetSelectedBall(SoccerBall soccerBall) {

        this.selectedBall = soccerBall;

        if (soccerBall != null) {

            ShowKickButton?.Invoke(this, EventArgs.Empty);
        }
        else {

            HideKickButton?.Invoke(this, EventArgs.Empty);
        }
    }

    public bool IsWalking() {
        return this.isWalking;
    }

    public Vector3 GetMoveDir() {
        return this.moveDir;
    }

    public float GetCurrentSpeed() {
        return this.currentSpeed;
    }

    public PlayerSO GetPlayerSO() {
        return this.playerSO;
    }

    public SoccerBall GetSelectedBall() {
        return this.selectedBall;
    }
}
