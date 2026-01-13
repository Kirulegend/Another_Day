using TMPro;
using UnityEngine;
using System.Collections;
namespace ITISKIRU
{
    public class PC : MonoBehaviour, Interactable
    {
        [SerializeField] Transform _cameraTrans;
        [SerializeField] Transform _cameraPreviewPos;
        [SerializeField] Transform _originalCamParent;
        [SerializeField] Vector3 _originalCamLocalPos;
        [SerializeField] Quaternion _originalCamLocalRot;
        [SerializeField] bool _isCameraTransitioning = false;
        [SerializeField] float _moveDuration = 1f;
        [SerializeField] bool _isInteracting = false;

        void Start()
        {
            _cameraTrans = Camera.main.transform;
            GameInput.GI_Instance.RMB_Down += GameInput_RMB_Down;
        }
        void OnDisable()
        {
            GameInput.GI_Instance.RMB_Down -= GameInput_RMB_Down;
        }
        void OnMouseOver()
        {
            if (!Player.isInteract) KeyEvents._ke.SetUIActive(InteractionType.Use);
        }
        void GameInput_RMB_Down()
        {
            if(!_isCameraTransitioning && _originalCamParent) StartCoroutine(ReturnCameraToOriginal());
        }
        public void OnInteract(int Num, Transform T)
        {
            if (_isInteracting) return;
            else
            {
                KeyEvents._ke.SetUIActive();
                _isInteracting = true;
                transform.parent.Find("Mart").GetComponent<Canvas>().worldCamera = Camera.main;
                if (!_isCameraTransitioning)
                {
                    _originalCamParent = _cameraTrans.parent;
                    _originalCamLocalPos = _cameraTrans.localPosition;
                    _originalCamLocalRot = _cameraTrans.localRotation;
                    Player.isInteract = true;
                    _cameraTrans.SetParent(null);
                    StartCoroutine(MoveCameraToLock(_cameraPreviewPos.position, _cameraPreviewPos.rotation));
                    GetComponent<Collider>().enabled = false;
                }
            }
        }
        public void OnInteractHand(Transform T) { }
        IEnumerator MoveCameraToLock(Vector3 targetPos, Quaternion targetRot)
        {
            _isCameraTransitioning = true;
            float elapsedTime = 0;
            Vector3 startPos = _cameraTrans.position;
            Quaternion startRot = _cameraTrans.rotation;
            while (elapsedTime < _moveDuration)
            {
                _cameraTrans.position = Vector3.Lerp(startPos, targetPos, elapsedTime / _moveDuration);
                _cameraTrans.rotation = Quaternion.Slerp(startRot, targetRot, elapsedTime / _moveDuration);
                Debug.Log("Moving");
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            KeyEvents._ke.SetUIActive(InteractionType.Back);
            _cameraTrans.position = targetPos;
            _cameraTrans.rotation = targetRot;
            _isCameraTransitioning = false;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        IEnumerator ReturnCameraToOriginal()
        {
            KeyEvents._ke.SetUIActive();
            GetComponent<Collider>().enabled = true;
            _isCameraTransitioning = true;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            float elapsedTime = 0;
            Vector3 startPos = _cameraTrans.position;
            Quaternion startRot = _cameraTrans.rotation;
            Vector3 targetPos = _originalCamParent.TransformPoint(_originalCamLocalPos);
            Quaternion targetRot = _originalCamParent.rotation * _originalCamLocalRot;
            while (elapsedTime < _moveDuration)
            {
                _cameraTrans.position = Vector3.Lerp(startPos, targetPos, elapsedTime / _moveDuration);
                _cameraTrans.rotation = Quaternion.Slerp(startRot, targetRot, elapsedTime / _moveDuration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            transform.parent.Find("Mart").GetComponent<Canvas>().worldCamera = null;
            _cameraTrans.SetParent(_originalCamParent);
            _cameraTrans.localPosition = _originalCamLocalPos;
            _cameraTrans.localRotation = _originalCamLocalRot;
            _originalCamParent = null;
            _isCameraTransitioning = false;
            Player.isInteract = false;
            _isInteracting = false;
        }
    }
}