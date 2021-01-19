using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D.IK;
using RedBlueGames.LiteFSM;
using System;

public class PlayerKatanaHandler : MonoBehaviour
{
//    [SerializeField] private Animator anim;
//    [Space]
//    [SerializeField] private Transform katana;
//    [SerializeField] private Collider2D hitCollider;
//    [SerializeField] private SpriteRenderer sprite;
//    [SerializeField] private LimbSolver2D handSolver;
//    [Space]
//    [SerializeField] private float solverAttachedWeight;
//    [Space]
//    [SerializeField] private Transform spine;
//    [SerializeField] private Vector3 localSpinePosition;
//    [SerializeField] private Vector3 localSpineRotation;
//    [Space]
//    [SerializeField] private Transform hand;
//    [SerializeField] private Vector3 localHandPosition;
//    [SerializeField] private Vector3 localHandRotation;
//    [Space]
//    [SerializeField] private float disconnectFromHandTime;
//    [Header("Attack")]
//    [SerializeField] private float attackInterval;
//    [SerializeField] private Vector3 checkSphereOffset;
//    [SerializeField] private float checkSphereRadius;

//    public float SolverAttachedWeight => solverAttachedWeight;

//    private Player player;
//    private Transform playerTransform;
//    private bool connectedToHand = false;
//    private bool attackUpdated = false;
//    private bool readyToAttack;
//    private float targetSolverWeight;
//    private float currentSolverWeight;
//    public void Initialize(Player player)
//	{
//        this.player = player;
//        playerTransform = player.transform;
//        player.OnAttackPressed += Attack;
//        AttachToSpine();
//	}

//    public void KatanaUpdate()
//    {
//        SolverWeightUpdate();
//    }

//    private void SolverWeightUpdate()
//	{
//        currentSolverWeight = Mathf.Lerp(currentSolverWeight, targetSolverWeight, Time.deltaTime * 10);

//        handSolver.weight = currentSolverWeight;
//	}

//    public void SetTargetSolverWeight(float value)
//	{
//        targetSolverWeight = Mathf.Clamp01(value);
//	}

//    public void Attack()
//	{
//        CustomEvent.Trigger(gameObject, "Attack Performed");
//        Variables.Scene(gameObject).Set("current in hand timer", 0.0);
//	}

//    public void OnAttackAnimationEnd()
//	{
//        hand.localEulerAngles = Vector3.zero;
//        CustomEvent.Trigger(gameObject, "Attack Animation End");
//    }

//	public void AttachToSpine()
//	{
//        sprite.sortingOrder = -1;
//        katana.parent = spine;
//        katana.localPosition = localSpinePosition;
//        katana.localEulerAngles = localSpineRotation;
//    }

//    public void AttachToHand()
//	{
//        sprite.sortingOrder = 1;
//        katana.parent = hand;
//        katana.localPosition = localHandPosition;
//        katana.localEulerAngles = localHandRotation;
//    }

//    public void SetHandAttachedWeight()
//	{
//        CustomEvent.Trigger(gameObject, "Attached To Hand");
//        handSolver.weight = solverAttachedWeight;
//    }

//    public void SetSpineAttachedWeight()
//	{
//        CustomEvent.Trigger(gameObject, "Attached To Spine");
//        handSolver.weight = 1;
//    }

//    public void CheckEnemies()
//	{
        
//	}

//	private void OnDisable()
//	{
//        player.OnAttackPressed -= Attack;
//    }

//#if UNITY_EDITOR
//	private void OnDrawGizmos()
//	{
//        Gizmos.color = Color.red;
//        Gizmos.DrawWireSphere(transform.position + checkSphereOffset, checkSphereRadius);
//	}
//#endif
}
