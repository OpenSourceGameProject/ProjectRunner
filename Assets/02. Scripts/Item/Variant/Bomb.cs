using UnityEngine;

public class Bomb : MonoBehaviour {
    private void Start() {
        Destroy(gameObject, 1f);
    }
}