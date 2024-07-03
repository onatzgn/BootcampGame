using UnityEngine;

public class Projectile : MonoBehaviour
{
    public int damage = 1;
    public ParticleSystem hitEffect;
    private void OnCollisionEnter(Collision collision)
    {
        GameObject player = collision.gameObject;
        if (player.CompareTag("Player"))
        {
            Actor playerActor = player.GetComponent<Actor>();
            if (playerActor != null)
            {
                playerActor.TakeDamage(damage);

                if (hitEffect != null)
                {
                    ParticleSystem effect = Instantiate(hitEffect, player.transform.position, Quaternion.identity);
                    effect.transform.SetParent(null);
                    effect.Play();

                    Destroy(effect.gameObject, effect.main.duration + effect.main.startLifetime.constant);
                }
            }

            Destroy(gameObject); 
        }

    }
}