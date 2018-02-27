using UnityEngine;
namespace FlappyBird
{
    public class Column : MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D collision)
        {
            Bird bird = collision.gameObject.GetComponent<Bird>();
            if (bird != null)
            {
                bird.Score += 1;
                bird.BetweenInColums = true;
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            Bird bird = collision.gameObject.GetComponent<Bird>();
            if (bird != null)
            {
                bird.BetweenInColums = false;
            }
        }

        private void OnTriggerStay2D(Collider2D collision)
        {
            Bird bird = collision.gameObject.GetComponent<Bird>();
            if (bird != null)
            {
                bird.BetweenInColums = true;
            }
        }
    }
}