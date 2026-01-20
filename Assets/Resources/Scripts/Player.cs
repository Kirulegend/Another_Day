using System;
using UnityEngine;
namespace ITISKIRU
{
    public class Player : MonoBehaviour
    {
        [SerializeField] float moveSpeed = 5f;
        [SerializeField] float mouseSensitivity = 2f;
        [SerializeField] Transform cameraTransform;
        [SerializeField] float maxLookX = 60f;
        [SerializeField] float minLookX = -60f;
        [SerializeField] float rotationX;
        [SerializeField] float raycastDistance = 3f;
        [SerializeField] LayerMask raycastLayerMask;
        [SerializeField] LayerMask ignoreLayerMask;
        [SerializeField] Transform grabPos;
        [SerializeField] Transform objGrabPos;
        [SerializeField] GameObject grabbedObject;
        [SerializeField] GameObject previewObject;
        [SerializeField] float lerpSpeed = 10f;
        [SerializeField] Material blueMaterial;
        [SerializeField] Material whiteMaterial;
        [SerializeField] Material redMaterial;
        [SerializeField] Transform uiCanvas;
        [SerializeField] GameManager gm;
        [SerializeField] Animator anim;
        [SerializeField] GameObject currentHitObj;
        [SerializeField] bool isPlaceable = false;
        [SerializeField] bool isStorable = false;
        [SerializeField] bool isFillable = false;
        [SerializeField] bool isHoldingHand = false;
        [SerializeField] bool isHolding = false;
        static bool _isInteract = false;
        public static bool _isHold = false;
        public static event Action<bool> OnInteraction;
        

        public static bool isInteract
        {
            get => _isInteract;
            set
            {
                _isInteract = value;
                OnInteraction?.Invoke(_isInteract);
            }
        }

        void Start()
        {
            isHolding = false;
            isHoldingHand = false;
            _isHold = false;
            cameraTransform = transform.Find("Camera");
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            grabPos = transform.Find("GrabPos");
            gm = GameObject.Find("GameManager").GetComponent<GameManager>();
            GameInput.GI_Instance.LMB_Down += GameInput_LMB_Down;
            //GameInput.GI_Instance.LMB_Hold += GameInput_LMB_Hold;
            GameInput.GI_Instance.RMB_Down += GameInput_RMB_Down;
            GameInput.GI_Instance.LSK_Down += GameInput_LSK_Down;
            GameInput.GI_Instance.ESC_Down += GI_Instance_ESC_Down;
        }

        void OnDisable()
        {
            GameInput.GI_Instance.LMB_Down -= GameInput_LMB_Down;
            //GameInput.GI_Instance.LMB_Hold -= GameInput_LMB_Hold;
            GameInput.GI_Instance.RMB_Down -= GameInput_RMB_Down;
            GameInput.GI_Instance.LSK_Down -= GameInput_LSK_Down;
            GameInput.GI_Instance.ESC_Down -= GI_Instance_ESC_Down;
        }

        void GameInput_LMB_Down()
        {
            if (currentHitObj && currentHitObj.GetComponent<Interactable>() != null)
            {
                if (!isHolding && !isHoldingHand)
                {
                    Interactable[] interactables = currentHitObj.GetComponents<Interactable>();
                    foreach (Interactable target in interactables) target.OnInteract(0, transform);
                }
                else if(isFillable) currentHitObj.GetComponent<Fillable>().Fill(grabbedObject);
                else if (isStorable) currentHitObj.GetComponent<Interactable>().OnInteractHand(grabbedObject.transform);
            }
            else if(grabbedObject && grabbedObject.GetComponent<Interactable>() != null && isPlaceable)
            {
                grabbedObject.GetComponent<Interactable>().OnInteract(0, transform);
            }

            if (isHoldingHand && !isFillable) PlaceGrabbedObjHand();
            else if(isHolding) PlaceGrabbedObj();

            if (Cursor.visible && !isInteract)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }
        void GameInput_LMB_Hold()
        {
            
        }

        void GameInput_RMB_Down()
        {
            if (currentHitObj && !grabbedObject) currentHitObj.GetComponent<Interactable>().OnInteract(1, transform);
        }

        void GameInput_LSK_Down()
        {
            if (moveSpeed == 10)
            {
                moveSpeed = 5;
                anim.SetBool("Sprint", false);
            }
            else
            {
                moveSpeed = 10;
                anim.SetBool("Sprint", true);
            }
        }

        void GI_Instance_ESC_Down()
        {
            if (!Cursor.visible)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
        }

        void Update()
        {
            CameraLook();
            MovePlayer();
            if (isHolding) HandlePreview();
            else if (isHoldingHand) HandlePreviewHand();
            else if (!isInteract) RaycastForward();
        }

        void MovePlayer()
        {
            Debug.Log(isInteract);
            if (isInteract) return;
            int moveX = (int)Input.GetAxisRaw("Horizontal");
            int moveZ = (int)Input.GetAxisRaw("Vertical");
            Vector3 move = (transform.right * moveX + transform.forward * moveZ).normalized * moveSpeed * Time.deltaTime;
            anim.SetFloat("MoveX", Input.GetAxis("Horizontal"));
            anim.SetFloat("MoveZ", Input.GetAxis("Vertical"));
            transform.position += move;
        }

        void CameraLook()
        {
            if (isInteract && Cursor.visible) return;
            float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
            float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;
            rotationX -= mouseY;
            rotationX = Mathf.Clamp(rotationX, minLookX, maxLookX);
            cameraTransform.localRotation = Quaternion.Euler(rotationX, 0f, 0f);
            transform.Rotate(Vector3.up * mouseX);
        }
        void RaycastForward()
        {
            Ray ray = new Ray(cameraTransform.position, cameraTransform.forward);
            if (Physics.Raycast(ray, out RaycastHit hit, raycastDistance))
            {
                if ((raycastLayerMask.value & (1 << hit.collider.gameObject.layer)) != 0)
                {
                    currentHitObj = hit.collider.gameObject;
                    Debug.DrawRay(ray.origin, ray.direction * hit.distance, Color.red); 
                }
                else
                {
                    currentHitObj = null;
                    KeyEvents._ke.SetUIActive();
                }
            }
            else
            {
                currentHitObj = null;
                KeyEvents._ke.SetUIActive();
            }
        }

        public void GrabObj(GameObject Obj)
        {
            if (isHolding) return;
            GameManager.gM.Off_boxUI();
            KeyEvents._ke.SetUIActive(InteractionType.Place, InteractionType.Rotate);
            grabbedObject = Obj;
            grabbedObject.layer = LayerMask.NameToLayer("Ignore Raycast");
            Rigidbody rb = grabbedObject.GetComponent<Rigidbody>();
            if(rb) rb.isKinematic = true;
            MonoBehaviour[] scripts = grabbedObject.GetComponents<MonoBehaviour>();
            foreach (var script in scripts) script.enabled = false;
            foreach (Collider col in grabbedObject.GetComponentsInChildren<Collider>()) col.enabled = false;
            previewObject = Instantiate(Obj, grabPos.position, Quaternion.identity);
            foreach (var script in scripts) script.enabled = true;
            foreach (Renderer r in previewObject.GetComponentsInChildren<Renderer>()) r.material = whiteMaterial;
            previewObject.GetComponent<Collider>().enabled = previewObject.GetComponent<Collider>().isTrigger = true;
            previewObject.AddComponent<PlacementPreview>();
            isHolding = _isHold = true;
        }

        void HandlePreview()
        {
            grabbedObject.transform.position = Vector3.Lerp(grabbedObject.transform.position, grabPos.position, Time.deltaTime * lerpSpeed * 2);
            grabbedObject.transform.rotation = Quaternion.Slerp(grabbedObject.transform.rotation, grabPos.rotation, Time.deltaTime * lerpSpeed * 2);
            Ray ray = new Ray(cameraTransform.position, cameraTransform.forward);
            RaycastHit hit;
            Vector3 previewPosition;
            if (Physics.Raycast(ray, out hit, raycastDistance, ~ignoreLayerMask))
            {
                if (hit.transform && hit.transform.gameObject.layer == 8)
                {
                    previewObject.SetActive(false);
                    isPlaceable = false;
                }
                else
                {
                    previewObject.SetActive(true);
                    isPlaceable = !previewObject.GetComponent<PlacementPreview>().HasCollision();
                }
                currentHitObj = hit.collider.gameObject;
                previewPosition = hit.point + new Vector3(0, .01f, 0);
            }
            else
            {
                currentHitObj = null;
                previewPosition = cameraTransform.position + cameraTransform.forward * raycastDistance;
                previewObject.SetActive(false);
                isPlaceable = false;
            }
            Renderer[] previewRenderers = previewObject.GetComponentsInChildren<Renderer>();
            foreach (Renderer r in previewRenderers)
            {
                if (isPlaceable)
                {
                    r.material = whiteMaterial;
                    KeyEvents._ke.SetUIActive(InteractionType.Place);
                }
                else
                {
                    r.material = redMaterial;
                    KeyEvents._ke.SetUIActive(InteractionType.None);
                }
            }
            previewObject.transform.position = previewPosition;
            if (Input.mouseScrollDelta.y != 0) previewObject.transform.Rotate(Vector3.up, Input.mouseScrollDelta.y * 15, Space.Self);
        }

        void PlaceGrabbedObj()
        {
            if (!isPlaceable) return;
            grabbedObject.layer = LayerMask.NameToLayer("Interactable");
            grabbedObject.transform.position = previewObject.transform.position;
            grabbedObject.transform.rotation = previewObject.transform.rotation;
            foreach (Collider col in grabbedObject.GetComponentsInChildren<Collider>()) col.enabled = true;
            Rigidbody rb = grabbedObject.GetComponent<Rigidbody>();
            if (rb) rb.isKinematic = false;
            Destroy(previewObject);
            grabbedObject = null;
            isHolding = _isHold  = isPlaceable = false;
            KeyEvents._ke.SetUIActive(InteractionType.None);
        }

        public void GrabObjHand(GameObject Obj)
        {
            if (isHoldingHand) return;
            GameManager.gM.Off_boxUI();
            KeyEvents._ke.SetUIActive(InteractionType.Place, InteractionType.Rotate);
            grabbedObject = Obj;
            grabbedObject.transform.parent = null;
            grabbedObject.layer = LayerMask.NameToLayer("Ignore Raycast");
            previewObject = Instantiate(Obj, objGrabPos.position, Quaternion.identity);
            grabbedObject.GetComponent<Collider>().enabled = false;
            previewObject.AddComponent<Rigidbody>().isKinematic = true;
            previewObject.GetComponent<Collider>().enabled = previewObject.GetComponent<Collider>().isTrigger = true;
            previewObject.AddComponent<PlacementPreview>();
            isHoldingHand = _isHold = true;
        }

        void HandlePreviewHand()
        {
            grabbedObject.transform.position = Vector3.Lerp(grabbedObject.transform.position, objGrabPos.position, Time.deltaTime * lerpSpeed);
            grabbedObject.transform.rotation = Quaternion.Slerp(grabbedObject.transform.rotation, objGrabPos.rotation, Time.deltaTime * lerpSpeed);
            Ray ray = new Ray(cameraTransform.position, cameraTransform.forward);
            Vector3 previewPosition;
            if (Physics.Raycast(ray, out RaycastHit hit, raycastDistance, ~ignoreLayerMask))
            {
                previewPosition = hit.point + new Vector3(0, .01f, 0);
                currentHitObj = hit.collider.gameObject;
                ItemObj IO = grabbedObject.GetComponent<ItemObj>();
                if (IO && IO._itemName != ItemName.Other)
                {
                    if (hit.transform.GetComponent<Containable>() != null)
                    {
                        isStorable = hit.transform.GetComponent<Containable>().Check(grabbedObject.GetComponent<ItemObj>()._itemName);
                        if (hit.transform.GetComponent<Fillable>() != null) isFillable = hit.collider.GetComponent<Containable>().Check(grabbedObject.GetComponent<ItemObj>()._itemName);
                    }
                    else isStorable = isFillable = false;
                }

                if (hit.transform && hit.transform.gameObject.layer == 8)
                {
                    previewObject.SetActive(false);
                    isPlaceable = false;
                }
                else
                {
                    previewObject.SetActive(true);
                    isPlaceable = !previewObject.GetComponent<PlacementPreview>().HasCollision();
                }
            }
            else
            {
                currentHitObj = null;
                previewPosition = cameraTransform.position + cameraTransform.forward * raycastDistance;
                previewObject.SetActive(false);
                isPlaceable = false;
                KeyEvents._ke.SetUIActive(InteractionType.None);
            }
            if (isStorable)
            {
                previewObject.GetComponent<ItemObj>()._material.material = blueMaterial;
                KeyEvents._ke.SetUIActive(InteractionType.Putin);
            }
            else if (isPlaceable)
            {
                previewObject.GetComponent<ItemObj>()._material.material = whiteMaterial;
                KeyEvents._ke.SetUIActive(InteractionType.Place);
            }
            else
            {
                previewObject.GetComponent<ItemObj>()._material.material = redMaterial;
                KeyEvents._ke.SetUIActive(InteractionType.None);
            }
            previewObject.transform.position = previewPosition;
            if (Input.mouseScrollDelta.y != 0) previewObject.transform.Rotate(Vector3.up, Input.mouseScrollDelta.y * 15, Space.Self);
        }

        void PlaceGrabbedObjHand()
        {
            if (!isPlaceable && !isStorable) return;
            grabbedObject.layer = LayerMask.NameToLayer("Interactable");
            Destroy(previewObject);
            isHoldingHand = _isHold = isPlaceable = false;
            if (isStorable) isStorable = isFillable = false;
            else
            {
                grabbedObject.GetComponent<Collider>().enabled = true;
                grabbedObject.transform.position = previewObject.transform.position;
                grabbedObject.transform.rotation = previewObject.transform.rotation;
            }
            grabbedObject = null;
            KeyEvents._ke.SetUIActive(InteractionType.None);
        }
    }
}