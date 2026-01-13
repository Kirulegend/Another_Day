using System;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.ShaderGraph.Internal.KeywordDependentCollection;
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
        public static bool isHoldingHand = false;
        public static bool isHolding = false;
        static bool _isInteract = false;
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
            cameraTransform = transform.Find("Camera");
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            grabPos = transform.Find("GrabPos");
            gm = GameObject.Find("GameManager").GetComponent<GameManager>();
            GameInput.GI_Instance.LMB_Down += GameInput_LMB_Down;
            GameInput.GI_Instance.LMB_Hold += GameInput_LMB_Hold;
            GameInput.GI_Instance.RMB_Down += GameInput_RMB_Down;
            GameInput.GI_Instance.LSK_Down += GameInput_LSK_Down;
            GameInput.GI_Instance.ESC_Down += GI_Instance_ESC_Down;
        }

        void OnDisable()
        {
            GameInput.GI_Instance.LMB_Down -= GameInput_LMB_Down;
            GameInput.GI_Instance.LMB_Hold -= GameInput_LMB_Hold;
            GameInput.GI_Instance.RMB_Down -= GameInput_RMB_Down;
            GameInput.GI_Instance.LSK_Down -= GameInput_LSK_Down;
            GameInput.GI_Instance.ESC_Down -= GI_Instance_ESC_Down;
        }

        void GameInput_LMB_Down()
        {
            if (currentHitObj && !isHolding && !isHoldingHand)
            {
                Debug.Log("Interact");
                currentHitObj.GetComponent<Interactable>().OnInteract(0, transform);
            }
            PlaceGrabbedObjHand();  
            PlaceGrabbedObj();
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
            Debug.Log("isHolding : " + isHolding);
            Debug.Log("isHoldingHand : " + isHoldingHand);
            Debug.Log("Current Obj : " + currentHitObj);
            CameraLook();
            MovePlayer();
            if (isHolding) HandlePreview();
            else if (isHoldingHand) HandlePreviewHand();
            else if (!isInteract) RaycastForward();
        }

        void MovePlayer()
        {
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

        public void GrabObj(GameObject box)
        {
            if (isHolding) return;
            GameManager.gM.Off_boxUI();
            KeyEvents._ke.SetUIActive(InteractionType.Place, InteractionType.Rotate);
            grabbedObject = box;
            grabbedObject.layer = LayerMask.NameToLayer("Ignore Raycast");
            grabbedObject.GetComponent<Rigidbody>().isKinematic = true;
            previewObject = Instantiate(box, grabPos.position, Quaternion.identity);
            grabbedObject.GetComponent<Collider>().enabled = false;
            foreach (Renderer r in previewObject.GetComponentsInChildren<Renderer>()) r.material = whiteMaterial;
            foreach (Collider col in previewObject.GetComponentsInChildren<Collider>()) col.isTrigger = true;
            previewObject.AddComponent<PlacementPreview>();
            isHolding = true;
        }
        void PlaceGrabbedObj()
        {
            if (!isPlaceable) return;
            Debug.Log("False");
            grabbedObject.layer = LayerMask.NameToLayer("Interactable");
            grabbedObject.transform.position = previewObject.transform.position;
            grabbedObject.transform.rotation = previewObject.transform.rotation;
            grabbedObject.GetComponent<Collider>().enabled = true;
            Rigidbody rb = grabbedObject.GetComponent<Rigidbody>();
            if (rb) rb.isKinematic = false;
            Destroy(previewObject);
            grabbedObject = null;
            isHolding = false;
            isPlaceable = false;
        }
        void HandlePreview()
        {
            grabbedObject.transform.position = Vector3.Lerp(grabbedObject.transform.position, grabPos.position, Time.deltaTime * lerpSpeed * 2);
            grabbedObject.transform.rotation = Quaternion.Slerp(grabbedObject.transform.rotation, grabPos.rotation, Time.deltaTime * lerpSpeed * 2);
            Ray ray = new Ray(cameraTransform.position, cameraTransform.forward);
            RaycastHit hit;
            Vector3 previewPosition;
            if (Physics.Raycast(ray, out hit, raycastDistance))
            {
                previewPosition = hit.point + new Vector3(0, .01f, 0);
                isPlaceable = !previewObject.GetComponent<PlacementPreview>().HasCollision();
            }
            else
            {
                previewPosition = cameraTransform.position + cameraTransform.forward * raycastDistance;
                isPlaceable = false;
            }
            previewObject.transform.position = previewPosition;
            if (Input.mouseScrollDelta.y != 0) previewObject.transform.Rotate(Vector3.up, Input.mouseScrollDelta.y * 15, Space.Self);
            Renderer[] previewRenderers = previewObject.GetComponentsInChildren<Renderer>();
            if (hit.transform && hit.transform.gameObject.layer == 8) previewObject.SetActive(false);
            else previewObject.SetActive(true);
            foreach (Renderer r in previewRenderers)
            {
                //Debug.Log(isPlaceable);
                if (isPlaceable) r.material = whiteMaterial;
                else r.material = redMaterial;
            }
        }

        public void GrabObjHand(GameObject box)
        {
            if (isHoldingHand) return;
            GameManager.gM.Off_boxUI();
            KeyEvents._ke.SetUIActive(InteractionType.Place, InteractionType.Rotate);
            grabbedObject = box;
            grabbedObject.transform.parent = null;
            grabbedObject.layer = LayerMask.NameToLayer("Ignore Raycast");
            previewObject = Instantiate(box, objGrabPos.position, Quaternion.identity);
            grabbedObject.GetComponent<Collider>().enabled = false;
            previewObject.AddComponent<Rigidbody>().isKinematic = true;
            previewObject.GetComponent<Collider>().enabled = true;
            previewObject.GetComponent<Collider>().isTrigger = true;
            previewObject.AddComponent<PlacementPreview>();
            isHoldingHand = true;
        }

        void PlaceGrabbedObjHand()
        {
            if (!isPlaceable) return;
            grabbedObject.layer = LayerMask.NameToLayer("Interactable");
            grabbedObject.transform.position = previewObject.transform.position;
            grabbedObject.transform.rotation = previewObject.transform.rotation;
            grabbedObject.GetComponent<Collider>().enabled = true;
            Destroy(previewObject);
            grabbedObject = null;
            isHoldingHand = false;
            isPlaceable = false;
            KeyEvents._ke.SetUIActive(InteractionType.None);
        }
        void HandlePreviewHand()
        {
            grabbedObject.transform.position = Vector3.Lerp(grabbedObject.transform.position, objGrabPos.position, Time.deltaTime * lerpSpeed);
            grabbedObject.transform.rotation = Quaternion.Slerp(grabbedObject.transform.rotation, objGrabPos.rotation, Time.deltaTime * lerpSpeed);
            Ray ray = new Ray(cameraTransform.position, cameraTransform.forward);
            Vector3 previewPosition;
            if (Physics.Raycast(ray, out RaycastHit hit, raycastDistance))
            {
                previewPosition = hit.point + new Vector3(0, .01f, 0);
                if (hit.collider.CompareTag("Box") && hit.collider.GetComponent<Box>().ItemCheck(grabbedObject.GetComponent<ItemObj>()._itemName))
                {
                    isPlaceable = true;
                    KeyEvents._ke.SetUIActive(InteractionType.Putin);
                    previewObject.GetComponent<ItemObj>()._material.material = blueMaterial;
                    if (Input.GetMouseButtonDown(0))
                    {
                        if (hit.collider.GetComponent<Box>()?.SetItem(grabbedObject) == true)
                        {
                            grabbedObject.layer = LayerMask.NameToLayer("Interactable");
                            Destroy(previewObject);
                            grabbedObject = null;
                            isHoldingHand = false;
                            KeyEvents._ke.SetUIActive(InteractionType.None);
                        }
                    }
                }
                else if (hit.collider.GetComponent<EggCooker>() && grabbedObject.GetComponent<ItemObj>()._itemName == ItemName.Egg)
                {
                    var EG = hit.collider.GetComponent<EggCooker>();
                    if (!EG.isCooking)
                    {
                        isPlaceable = true;
                        KeyEvents._ke.SetUIActive(InteractionType.Putin);
                        previewObject.GetComponent<ItemObj>()._material.material = blueMaterial;
                        if (Input.GetMouseButtonDown(0))
                        {
                            if (hit.collider.GetComponent<EggCooker>()?.SetItem(grabbedObject) == true)
                            {
                                grabbedObject.layer = LayerMask.NameToLayer("Interactable");
                                Destroy(previewObject);
                                grabbedObject = null;
                                isHoldingHand = false;
                                KeyEvents._ke.SetUIActive(InteractionType.None);
                            }
                        }
                    }
                }
                else
                {
                    isPlaceable = !previewObject.GetComponent<PlacementPreview>().HasCollision();
                    if (hit.transform && hit.transform.gameObject.layer == 8)
                    {
                        previewObject.SetActive(false);
                        isPlaceable = false;
                    }
                    else previewObject.SetActive(true);
                    if (isPlaceable)
                    {
                        KeyEvents._ke.SetUIActive(InteractionType.Place, InteractionType.Rotate);
                        previewObject.GetComponent<ItemObj>()._material.material = whiteMaterial;
                    }
                    else
                    {
                        KeyEvents._ke.SetUIActive(InteractionType.None);
                        previewObject.GetComponent<ItemObj>()._material.material = redMaterial;
                    }
                }
            }
            else
            {
                previewPosition = cameraTransform.position + cameraTransform.forward * raycastDistance;
                isPlaceable = false;
                KeyEvents._ke.SetUIActive(InteractionType.None);
                previewObject.GetComponent<ItemObj>()._material.material = redMaterial;
            }
            previewObject.transform.position = previewPosition;
            if (Input.mouseScrollDelta.y != 0) previewObject.transform.Rotate(Vector3.up, Input.mouseScrollDelta.y * 15, Space.Self);
        }
    }
}