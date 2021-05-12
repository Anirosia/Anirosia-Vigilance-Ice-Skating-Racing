using System;
using System.Collections;
using DefaultNamespace;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

namespace Gameplay
{
    public class CameraFollow : MonoBehaviour
    {
        #region Fields
        [Header("Camera Settings")]
        [ReadOnlyInspector] [SerializeReference]
        private float cameraSmoothing = 0.1f;
        [SerializeField] private Vector3 offset;
        [SerializeField] private Transform target;
        [SerializeField] [Range(0, 1)] private float zoomSmoothSpeed;
        [ReadOnlyInspector] [SerializeField] private int viewDist;
        [ReadOnlyInspector] [SerializeField] private float smooth;
        private int _desiredZoomInDistance = 3;
        private int _desiredZoomOutDistance = 8;
        private bool _isTargetNotNull;
        private Camera _viewCamera;
        private Vector3 _savedOffset;
        private float _cameraLock = 1;
        [Header("Zoom Settings")]
        [Header("Zoom Values")]
        [ReadOnlyInspector] [SerializeField] private float setOffsetSpeed = 17f;
        [ReadOnlyInspector] [SerializeField] private float setZoomSpeed = 10f;
        [ReadOnlyInspector] [SerializeField] private float setLockSpeed = 4.5f;

        [Header("Zoom Calculations")]
        //execution after calculation
        [ReadOnlyInspector] [SerializeField] private float offsetSpeed;
        [ReadOnlyInspector] [SerializeField] private float zoomSpeed;
        [ReadOnlyInspector] [SerializeField] private float lockSpeed;
        private bool _actionCalled;
  #endregion

        #region Properties
        public Camera ViewCamera=>_viewCamera;
  #endregion

        #region Start Methods
        private void Awake(){
            _viewCamera = GetComponent<Camera>();
        }
        private void OnEnable(){
            _isTargetNotNull = target!=null;
            viewDist = Mathf.RoundToInt(_viewCamera.orthographicSize);
            _savedOffset = offset;
            smooth = cameraSmoothing;
            UnitNotionConversion();
        }

        private void OnValidate(){
            UnitNotionConversion();
        }
  #endregion

        #region Updates
        private void FixedUpdate(){
            if(_isTargetNotNull){
                var desiredPos = target.position + offset;
                Vector3 smoothPos = Vector3.Lerp(transform.position, desiredPos, smooth);
                transform.position = smoothPos;
            }
        }
        public void CameraWork(bool grounded, bool sliding){
            if(!sliding){
                if(!grounded && !_actionCalled){
                    StopAllCoroutines();
                    StartCoroutine(CameraZoomOut(enableY: true));
                    _actionCalled = true;
                }
                else if(grounded && _actionCalled){
                    StopAllCoroutines();
                    StartCoroutine(CameraZoomReset());
                    _actionCalled = false;
                }
            }
        }
  #endregion

        #region Camera Actions
        public IEnumerator CameraZoomIn(float xOffset = default, bool camLock = false,
                                        bool reset = false){
            // Debug.Log("Camera Zoom In");
            int zoomDistance;
            zoomDistance = reset ? viewDist : _desiredZoomInDistance;

            while (_viewCamera.orthographicSize >= zoomDistance){
                _viewCamera.orthographicSize -= zoomSpeed;

                if(offset.y <= 0) offset.y += offsetSpeed * 1.5f;

                if(offset.x >= xOffset) offset.x -= offsetSpeed;
                yield return new WaitForSeconds(Time.deltaTime);
                if(camLock){
                    if(smooth <= _cameraLock) smooth += lockSpeed;
                }
            }

            if(camLock) smooth = _cameraLock;
            offset.x = xOffset;
            offset.y = _savedOffset.y;
            _viewCamera.orthographicSize = zoomDistance;
        }
        private IEnumerator CameraZoomOut(float xOffset = default, bool reset = false, bool enableY = false){
            // Debug.Log("Camera Zoom Out");
            int zoomDistance;
            zoomDistance = reset ? viewDist : _desiredZoomOutDistance;
            while (_viewCamera.orthographicSize <= zoomDistance){
                _viewCamera.orthographicSize += zoomSpeed;
                if(offset.x <= xOffset) offset.x += offsetSpeed;
                if(offset.y <= 0 && enableY) offset.y -= offsetSpeed;
                if(_viewCamera.orthographicSize >= cameraSmoothing){
                    if(smooth <= cameraSmoothing) smooth = cameraSmoothing;
                    else smooth -= lockSpeed;
                }
                yield return new WaitForSeconds(Time.deltaTime);
            }
            smooth = cameraSmoothing;
            offset.x = _savedOffset.x;
            offset.y = _savedOffset.y;
            _viewCamera.orthographicSize = zoomDistance;
        }
        public IEnumerator CameraZoomReset(){
            StopAllCoroutines();
            // Debug.Log("Camera Reset");
            if(_viewCamera.orthographicSize < viewDist)
                return CameraZoomOut(_savedOffset.x, true);
            else
                return CameraZoomIn(_savedOffset.x, reset: true);
        }

        private float _shakeTime = 1f;
        private float _duration;
        public IEnumerator CameraShake(){
            _duration = 0;
            var originalPos = offset;
            var xMin = offset.x - 1;
            var xMax = offset.x + 1;
            while (_shakeTime > _duration){
                var xOffset = offset.x + Random.Range(-1f, 1f) * .1f;
                var yOffset = Random.Range(-1f, 1f) * .1f;
                offset = new Vector3(Mathf.Clamp(xOffset, xMin, xMax), offset.y, offset.z);
                _duration += Time.deltaTime;
                yield return null;
            }
            offset = originalPos;
        }
  #endregion

        private void UnitNotionConversion(){
            var power = _desiredZoomInDistance + zoomSmoothSpeed;
            offsetSpeed = (float)Math.Round(setOffsetSpeed * Mathf.Pow(10, -power), 5);
            zoomSpeed = (float)Math.Round(setZoomSpeed * Mathf.Pow(10, -power), 5);
            lockSpeed = (float)Math.Round(setLockSpeed * Mathf.Pow(10, -power), 5);
        }

        // public void SelfReference(Transform objectTransform) {
        // 	if (!_isTargetNotNull) {
        // 		target = objectTransform;
        // 		_isTargetNotNull = true;
        // 	}
        // }
    }
}
