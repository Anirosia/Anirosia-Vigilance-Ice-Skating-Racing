using UnityEngine;
namespace LevelScripts.Coins
{
    public class CoinPickUp : MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D other){
            if(other.CompareTag("Mouse")){
                GameManager.Instance.OnCoinCollected();
                Destroy(gameObject);
            }
        
        }
    }
}
