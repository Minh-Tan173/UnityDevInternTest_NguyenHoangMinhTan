using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticalManager : MonoBehaviour
{
    [SerializeField] private SoccerGoal soccerGoalParent;

    private ParticleSystem partical;
    private Coroutine currentCoroutine;

    private void Awake() {

        partical = GetComponent<ParticleSystem>();
    }


    private void Start() {

        soccerGoalParent.OnPlayPartical += SoccerGoalParent_OnPlayPartical;

        // When start scene
        Hide();
    }

    private void OnDestroy() {

        soccerGoalParent.OnPlayPartical -= SoccerGoalParent_OnPlayPartical;
    }

    private void SoccerGoalParent_OnPlayPartical(object sender, System.EventArgs e) {

        Show();
        
        if (currentCoroutine != null) {
            StopCoroutine(currentCoroutine);
            currentCoroutine = null;
        }

        currentCoroutine = StartCoroutine(PlayParticalCoroutine());
    }

    private IEnumerator PlayParticalCoroutine() {

        partical.Stop();
        partical.Clear();
        partical.Play();

        while (partical.IsAlive()) {
            yield return null;  
        }

        yield return null;

        Hide();
    }

    private void Show() {
        this.gameObject.SetActive(true);
    }

    private void Hide() {
        this.gameObject.SetActive(false);
    }
}
