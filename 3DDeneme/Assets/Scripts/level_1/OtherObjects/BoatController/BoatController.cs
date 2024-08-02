using UnityEngine;

public class BoatController : MonoBehaviour
{
    private bool isNearBoat = false;
    private GameObject player;
    private bool isDriving = false;
    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (isNearBoat && Input.GetKeyDown(KeyCode.E))
        {
            if (!isDriving)
            {
                // Karakteri bota bindir
                player.transform.SetParent(transform);
                player.SetActive(false);
                isDriving = true;
            }
            else
            {
                // Karakteri botdan indir
                player.transform.SetParent(null);
                player.SetActive(true);
                isDriving = false;
            }
        }

        if (isDriving)
        {
            // Bot hareketi
            float move = Input.GetAxis("Vertical") * 10f * Time.deltaTime;
            float turn = Input.GetAxis("Horizontal") * 50f * Time.deltaTime;

            rb.MovePosition(rb.position + transform.forward * move);
            rb.MoveRotation(rb.rotation * Quaternion.Euler(0, turn, 0));
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            isNearBoat = true;
            player = collision.gameObject;
        }
    }

    private void OnCollisionExit(Collision other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            isNearBoat = false;
            player = null;
        }
    }
}
