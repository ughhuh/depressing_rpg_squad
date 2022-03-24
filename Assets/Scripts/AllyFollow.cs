using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AllyFollow : MonoBehaviour
{
    [Tooltip("How fast the object moves")]
    [SerializeField] private float followSpeed = 1f;
    [Tooltip("Acceptable distance between target and object")]
    [SerializeField] private float followDistance = 1f;
    [Tooltip("How fast the object moves towards the target")]
    [SerializeField] Transform target;

    void Update()
    {
        if (Vector2.Distance(transform.position, target.position) > followDistance)
        {
            FollowPlayer();
        }
    }

    private void FollowPlayer()
    {
        transform.position = Vector2.MoveTowards(transform.position, target.position, followSpeed * Time.deltaTime);
    }
}
