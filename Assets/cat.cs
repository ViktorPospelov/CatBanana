using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class cat : MonoBehaviour
{
    [SerializeField] private GameObject cry;
    [SerializeField] private Collider2D collider2D;
    [SerializeField] private Collider2D collider2DOsnova;

    private void Start()
    {
        transform.localPosition += new Vector3(Random.Range(-20, 40) * 1f, 0f);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.otherCollider == collider2D)
        {
            cry.SetActive(true);
            GameControler.EndGame?.Invoke();
        }

        if (collision.otherCollider == collider2DOsnova && collision.collider.tag == "Respawn")
        {
            cry.SetActive(true);
            GameControler.EndGame?.Invoke();
        }
    }
}