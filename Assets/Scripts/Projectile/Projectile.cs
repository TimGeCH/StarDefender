using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour

{
    [SerializeField] protected GameObject hitVFX;

    [SerializeField] protected AudioData[] hitSFX;

    [SerializeField] public float damage;

    [SerializeField] protected float moveSpeed = 10f;

    [SerializeField] protected Vector2 moveDirection;

    protected GameObject target;

    protected float damagePerShot;

    public static float originalDamage;

 

    protected virtual void OnEnable()
    {
 
        StartCoroutine(MoveDirctly());
    }

    protected virtual IEnumerator MoveDirctly()
    {
        while (gameObject.activeSelf)
        {
            Move();

            yield return null;
        }
    }

    protected void SetTarget(GameObject target)
    {
        this.target = target;
    }

    public void Move() => transform.Translate(moveDirection * moveSpeed * Time.deltaTime);

    protected virtual void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.TryGetComponent<Character>(out Character character))
        {
            character.TakeDamage(damage);
            PoolManager.Release(hitVFX, collision.GetContact(0).point, Quaternion.LookRotation(collision.GetContact(0).normal));
            AudioManager.Instance.PlayRandomSFX(hitSFX);
            gameObject.SetActive(false);
        }
    }

    
}
