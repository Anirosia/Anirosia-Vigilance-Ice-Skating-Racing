using System.Collections;
using UnityEngine;
namespace LevelScripts
{
    public class Parallax : MonoBehaviour
    {
        private float _startPosX;
        private float _length;
        private Transform _camera;

        [SerializeField] private float effect;
        // Start is called before the first frame update
        void Start(){
            _camera = transform.root;
            _startPosX = transform.position.x;
            _length = GetComponent<SpriteRenderer>().bounds.size.x;
            StartCoroutine(ParallaxEffect());
        }

        // Update is called once per frame
        IEnumerator ParallaxEffect(){
            while (true){
                var position = transform.position;
                var temp = (_camera.position.x * (1 - effect));
                var dist = (_camera.position.x * effect);
                position = new Vector3(_startPosX + dist, position.y, position.z);
                transform.position = position;

                if(temp > _startPosX + _length) _startPosX += _length;
                else if(temp < _startPosX - _length) _startPosX -= _length;
                yield return null;
            }
        }
    }
}
