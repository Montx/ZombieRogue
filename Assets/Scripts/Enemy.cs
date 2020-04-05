using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MovingObject
{
    public int playerDamage;

    public AudioClip enemyChop1;
    public AudioClip enemyChop2;

    private Animator animator;
    private Transform target;
    private bool skipMove;

    protected override void Start() {
        GameManager.instance.AddEnemyToList(this);

        animator = GetComponent<Animator>();
        target = GameObject.FindGameObjectWithTag("Player").transform;

        base.Start();
    }

    protected override bool AttemptMove<T>(int xDir, int yDir) {
        if (skipMove) {
            skipMove = false;
            return false;
        }

        base.AttemptMove<T>(xDir, yDir);

        skipMove = true;

        return true;
    }

    protected override void OnCantMove<T>(T component) {
        Player hitPlayer = component as Player;

        hitPlayer.LoseFood(playerDamage);

        animator.SetTrigger("enemyChop");

        SoundManager.instance.RandomizeSfx(enemyChop1, enemyChop2);
    }

    public void MoveEnemy() {
        int xDir = 0;
        int yDir = 0;

        if (Mathf.Abs(target.position.x - transform.position.x) < float.Epsilon) {
            yDir = target.position.y > transform.position.y ? 1 : -1;
        } else {
            xDir = target.position.x > transform.position.x ? 1 : -1;
        }

        AttemptMove<Player>(xDir, yDir);
    }


}
