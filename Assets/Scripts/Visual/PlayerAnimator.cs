using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    [SerializeField] private Player player;

    private const string BLEND_TREE_PARA = "Blend";
    private const float IDLE_THRESHOLD = 0f;
    private const float WALKING_THRESHOLD = 0.25f;
    private const float RUNNING_THRESHOLD = 0.6f;

    private Animator animator;

    private void Awake() {

        animator = GetComponent<Animator>();
    }

    private void Update() {

        if (player.IsWalking()) {

            if (player.GetCurrentSpeed() == player.GetPlayerSO().walkSpeed) {

                animator.SetFloat(BLEND_TREE_PARA, WALKING_THRESHOLD);
            }
            else if (player.GetCurrentSpeed() == player.GetPlayerSO().runSpeed) {

                animator.SetFloat(BLEND_TREE_PARA, RUNNING_THRESHOLD);
            }
        }
        else {

            animator.SetFloat(BLEND_TREE_PARA, IDLE_THRESHOLD);
        }
    }
}
