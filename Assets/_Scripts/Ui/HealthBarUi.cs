using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Unity.Netcode;
using System;

public class HealthBarUi : MonoBehaviour
{
    [SerializeField] private Image healthBar;
    [SerializeField] private Image healthFiller;

    public Transform playerPos;

    private float target = 1f;
    [SerializeField]private float ReduceSpeed = 2f;
    private Vector3 startSize;

    private void Start()
    {
        startSize = transform.localScale;
        playerPos = Camera.main.transform;
    }



    public void updateHealthBar(float currentHealth, float maxHealth)
    {
        if (transform.localScale == startSize)
            transform.DOPunchScale(transform.localScale , 0.15f);
        target = currentHealth / maxHealth;
    }

    private void LateUpdate()
    {
        if (playerPos == null) return;
        //Debug.Log();
        transform.rotation = Quaternion.LookRotation(transform.position - playerPos.transform.position);
        healthBar.fillAmount = target;
        healthFiller.fillAmount = Mathf.MoveTowards(healthFiller.fillAmount, target, ReduceSpeed * Time.deltaTime);
    }
}
