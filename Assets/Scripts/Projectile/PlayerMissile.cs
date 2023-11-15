using System.Collections;
using UnityEngine;

public class PlayerMissile : PlayerProjectileOverdrive
{
    [SerializeField] AudioData targetAcquiredVoice = null;

    [Header("==== SPEED CHANGE ====")]
    [SerializeField] float lowSpeed = 8f;
    [SerializeField] float highSpeed = 25f;
    [SerializeField] float variableSpeedDelay = 0.5f;

    [Header("==== EXPLOSION ====")]
    [SerializeField] GameObject explosionVFX = null;
    [SerializeField] AudioData explosionSFX = null;
    [SerializeField] LayerMask enemyLayerMask = default;
    [SerializeField] float explosionRadius = 3f;
    [SerializeField] float explosionDamage = 100f;

    WaitForSeconds waitVariableSpeedDelay;

    protected override void Awake()
    {
        base.Awake();
        waitVariableSpeedDelay = new WaitForSeconds(variableSpeedDelay);
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        StartCoroutine(nameof(VariableSpeedCoroutine));
    }

    protected override void OnCollisionEnter2D(Collision2D collision)
    {
        base.OnCollisionEnter2D(collision);
        
        PoolManager.Release(explosionVFX, transform.position);
        
        AudioManager.Instance.PlayRandomSFX(explosionSFX);

        var colliders = Physics2D.OverlapCircleAll(transform.position, explosionRadius, enemyLayerMask);

        foreach (var collider in colliders)
        {
            if(collider.TryGetComponent<Enemy>(out Enemy Enemy))
            {
                Enemy.TakeDamage(explosionDamage);
            }
        }
        
    }

    IEnumerator VariableSpeedCoroutine()
    {
        moveSpeed = lowSpeed;

        yield return waitVariableSpeedDelay;

        moveSpeed = highSpeed;

        if (target != null)
        {
            AudioManager.Instance.PlayRandomSFX(targetAcquiredVoice);
        }
    }

    // * AOE Damage Implementation 2
    // * ��Χ�˺�ʵ�ַ���2
    // !Disadvantages: To detect all enemies in the scene, slightly lower efficiency 
    // !ȱ�㣺��ⳡ�������еĵ��ˣ�Ч���Ե�
    // void DistanceDetection()
    // {
    //     // Loop detection all enemies in current scene
    //     // ������ǰ���������еĵ���
    //     foreach (var enemyInRange in EnemyManager.Instance.Enemies)
    //     {
    //         // If the distance between the enemy and the missile is within the explosion radius (3f)
    //         // ������˺͵����ľ����ڱ�ը�뾶(3f)��
    //         if (Vector2.Distance(transform.position, enemyInRange.transform.position) <= 3f)
    //         {
    //             if (enemyInRange.TryGetComponent<Enemy>(out Enemy enemy))
    //             {
    //                 // enemy take 100 damage
    //                 // ������ܵ�100���˺�
    //                 enemy.TakeDamage(100f);
    //             }
    //         }
    //     }
    // }

    // * AOE Damage Implementation 3
    // * ��Χ�˺�ʵ�ַ���3
    // [SerializeField] LayerMask enemyLayerMask = default;
    // [SerializeField] float explosionRadius = 3f;
    // [SerializeField] float explosionDamage = 100f;

    // void PhysicsOverlapDetection()
    // {
    //     // Enemies within explosion radius take AOE damage
    //     var colliders = Physics2D.OverlapCircleAll(transform.position, explosionRadius, enemyLayerMask);

    //     foreach (var collider in colliders)
    //     {
    //         if (collider.TryGetComponent<Enemy>(out Enemy enemy))
    //         {
    //             enemy.TakeDamage(explosionDamage);
    //         }
    //     }
    // }

    // void OnDrawGizmosSelected()
    // {
    //     Gizmos.color = Color.yellow;
    //     Gizmos.DrawWireSphere(transform.position, explosionRadius);
    // }
}