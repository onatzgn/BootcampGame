using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SurpriseBox : MonoBehaviour
{
    [SerializeField] private GameObject award;
    [SerializeField] private GameObject empty_prefab;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            HandleCollision();
        }
    }

    private void HandleCollision()
    {
        // %10 ihtimalle fonksiyonu çalýþtýr
        SurprizeBoxCheck();

        
        Vector3 currentPosition = transform.position;

        
        GameObject empty = Instantiate(empty_prefab, currentPosition, Quaternion.identity);

        
        gameObject.SetActive(false);

        
        Destroy(empty, 6.0f);

        
        Destroy(gameObject, 5.0f);
    }

    private void SurprizeBoxCheck()
    {
        float randomValue = Random.Range(0f, 1f);

        if (randomValue < 0.1f)
        {
            SurprizeBoxFunc();
        }
    }

    private void SurprizeBoxFunc()
    {
        Instantiate(award, transform.position, Quaternion.identity);
    }
}
