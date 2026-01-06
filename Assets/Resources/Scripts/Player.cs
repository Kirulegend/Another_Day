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
        public static bool isHoldingSmall = false;
        public static bool isHolding = false;
        static bool _isInteract = false;
        public static event Action<bool> OnInteraction;

        public static bool isInteract
        {
            get => _isInteract;
            set
            {
                if (_isInteract == value) return;
                _isInteract = value;
                OnInteraction?.Invoke(_isInteract);
            }
        }

        void Start()
        {
            cameraTransform = transform.Find("Camera");
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            grabPos = transform.Find("GrabPos");
            gm = GameObject.Find("GameManager").GetComponent<GameManager>();
            GameInput.GI_Instance.LMB_Down += GameInput_LMB_Down;
            GameInput.GI_Instance.RMB_Down += GameInput_RMB_Down;
            GameInput.GI_Instance.LSK_Down += GameInput_LSK_Down;
            GameInput.GI_Instance.ESC_Down += GI_Instance_ESC_Down;
        }

        void OnDisable()
        {
            GameInput.GI_Instance.LMB_Down -= GameInput_LMB_Down;
            GameInput.GI_Instance.RMB_Down -= GameInput_RMB_Down;
            GameInput.GI_Instance.LSK_Down -= GameInput_LSK_Down;
            GameInput.GI_Instance.ESC_Down -= GI_Instance_ESC_Down;
        }

        void GameInput_LMB_Down()
        {
            Debug.Log("GameInput_LMB_Down event received in Player class.");
            if(currentHitObj) currentHitObj.GetComponent<Interactable>().OnInteract("Suli");
            if (Cursor.visible && !isInteract)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }

        void GameInput_RMB_Down()
        {
            //if (currentHitObj) currentHitObj.GetComponent<Interactable>().OnInteract("Suli");
            Debug.Log("GameInput_RMB_Down event received in Player class.");

        }

        void GameInput_LSK_Down()
        {
            Debug.Log("GameInput_LSK_Down event received in Player class.");
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
            else if (isHoldingSmall) HandlePreviewHand();
            else if (!isInteract && !isHolding) RaycastForward();
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
                    if (hit.collider.CompareTag("Box"))
                    {
                        if ((uiCanvas && uiCanvas.transform.position != hit.collider.GetComponent<Box>()._canvasPoint.position) || !uiCanvas)
                        {
                            uiCanvas = gm.Set_boxUI(hit.collider.GetComponent<Box>().defaultItem.ToString(), hit.collider.GetComponent<Box>().GetQuantity(), hit.collider.GetComponent<Box>()._canvasPoint.position);
                        }
                        bool temp = hit.collider.GetComponent<Box>()._opened;
                        Animator animator = hit.collider.GetComponent<Animator>();
                        if (Input.GetMouseButtonDown(1) && !temp)
                        {
                            GrabObj(hit.collider.gameObject);
                            hit.collider.GetComponent<Box>()._grabbed = true;
                        }
                        else if (Input.GetMouseButtonDown(1) && temp)
                        {
                            animator.SetTrigger("Move");
                            animator.SetBool("Open", false);
                            hit.collider.GetComponent<Box>()._opened = false;
                        }
                        else if (Input.GetMouseButtonDown(0) && !temp)
                        {
                            animator.SetTrigger("Move");
                            animator.SetBool("Open", true);
                            hit.collider.GetComponent<Box>()._opened = true;
                        }
                        else if (Input.GetMouseButtonDown(0) && temp)
                        {
                            GameObject Temp = hit.collider.GetComponent<Box>().GetItem();
                            if (Temp) GrabObjHand(Temp);
                        }
                    }
                    else if (hit.collider.name == "EggCooker")
                    {
                        currentHitObj = null;
                        if ((uiCanvas && uiCanvas.transform.position != hit.collider.GetComponent<EggCooker>()._canvasPoint.position) || !uiCanvas)
                        {
                            uiCanvas = gm.Set_boxUI("EggCooker", hit.collider.GetComponent<EggCooker>().GetData(), hit.collider.GetComponent<EggCooker>()._canvasPoint.position);
                        }
                        if (Input.GetMouseButtonDown(0))
                        {
                            if (hit.collider.GetComponent<EggCooker>().isOpen)
                            {
                                GameObject Temp = hit.collider.GetComponent<EggCooker>().GetItem();
                                if (Temp) GrabObjHand(Temp);
                            }
                            else
                            {
                                //hit.collider.GetComponent<EggCooker>().Boil();
                                hit.collider.GetComponent<EggCooker>().OC(false);
                            }
                        }
                        if (Input.GetMouseButtonDown(1))
                        {
                            if (hit.collider.GetComponent<EggCooker>().isOpen) hit.collider.GetComponent<EggCooker>().OC(true);
                            else hit.collider.GetComponent<EggCooker>().Boil();
                        }
                    }
                    else if (hit.collider.CompareTag("Item"))
                    {
                        currentHitObj = null;
                        if ((uiCanvas && uiCanvas.transform.position != hit.collider.GetComponent<ItemObj>()._canvasPoint.position) || !uiCanvas)
                        {
                            uiCanvas = gm.Set_boxUI(hit.collider.GetComponent<ItemObj>()._itemName.ToString(), hit.collider.GetComponent<ItemObj>().GetData(), hit.collider.GetComponent<ItemObj>()._canvasPoint.position);
                        }
                        if (Input.GetMouseButtonDown(0)) GrabObjHand(hit.collider.gameObject);
                    }
                    else
                    {
                        if (uiCanvas)
                        {
                            uiCanvas.gameObject.SetActive(false);
                            uiCanvas = null;
                        }
                    }
                }
                else
                {
                    if (uiCanvas)
                    {
                        uiCanvas.gameObject.SetActive(false);
                        uiCanvas = null;
                    }
                    KeyEvents._ke.SetUIActive();
                }
            }
            else
            {
                currentHitObj = null;
                KeyEvents._ke.SetUIActive();
                if (uiCanvas)
                {
                    uiCanvas.gameObject.SetActive(false);
                    uiCanvas = null;
                }
            }
        }

        void LateUpdate()
        {
            if (uiCanvas)
            {
                Vector3 direction = cameraTransform.position - uiCanvas.transform.position;
                if (direction.magnitude > 0.01f)
                {
                    Quaternion targetRotation = Quaternion.LookRotation(-direction);
                    uiCanvas.transform.rotation = Quaternion.Slerp(uiCanvas.transform.rotation, targetRotation, lerpSpeed * Time.deltaTime);
                }
            }
        }

        void GrabObj(GameObject box)
        {
            if (isHolding) return;
            if (uiCanvas)
            {
                uiCanvas.gameObject.SetActive(false);
                uiCanvas = null;
            }
            KeyEvents._ke.SetUIActive(InteractionType.Place, InteractionType.Rotate);
            isHolding = true;
            grabbedObject = box;
            grabbedObject.layer = LayerMask.NameToLayer("Ignore Raycast");
            grabbedObject.GetComponent<Collider>().enabled = false;
            grabbedObject.GetComponent<Rigidbody>().isKinematic = true;
            previewObject = Instantiate(box, grabPos.position, Quaternion.identity);
            Renderer[] previewRenderers = previewObject.GetComponentsInChildren<Renderer>();
            foreach (Renderer r in previewRenderers) r.material = whiteMaterial;
            Collider[] previewColliders = previewObject.GetComponentsInChildren<Collider>();
            foreach (Collider col in previewColliders) col.isTrigger = true;
            previewObject.AddComponent<PlacementPreview>();
        }

        void HandlePreview()
        {
            grabbedObject.transform.position = Vector3.Lerp(grabbedObject.transform.position, grabPos.position, Time.deltaTime * lerpSpeed * 2);
            grabbedObject.transform.rotation = Quaternion.Slerp(grabbedObject.transform.rotation, grabPos.rotation, Time.deltaTime * lerpSpeed * 2);
            Ray ray = new Ray(cameraTransform.position, cameraTransform.forward);
            RaycastHit hit;
            bool isPlaceable = false;
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
            float scrollInput = Input.mouseScrollDelta.y;
            if (scrollInput != 0) previewObject.transform.Rotate(Vector3.up, scrollInput * 15, Space.Self);
            Renderer[] previewRenderers = previewObject.GetComponentsInChildren<Renderer>();
            foreach (Renderer r in previewRenderers)
            {
                if (isPlaceable) r.material = whiteMaterial;
                else r.material = redMaterial;
            }
            if (Input.GetMouseButtonDown(0) && isPlaceable)
            {
                grabbedObject.layer = LayerMask.NameToLayer("Water");
                grabbedObject.transform.position = previewPosition;
                grabbedObject.transform.rotation = previewObject.transform.rotation;
                Rigidbody rb = grabbedObject.GetComponent<Rigidbody>();
                if (rb) rb.isKinematic = false;
                grabbedObject.GetComponent<Collider>().enabled = true;
                Renderer[] originalRenderers = grabbedObject.GetComponentsInChildren<Renderer>();
                foreach (Renderer r in originalRenderers) r.enabled = true;
                Destroy(previewObject);

                if (grabbedObject.GetComponent<Box>()) grabbedObject.GetComponent<Box>()._grabbed = false;

                grabbedObject = null;
                isHolding = false;

            }
        }

        void GrabObjHand(GameObject box)
        {
            if (isHoldingSmall) return;
            if (uiCanvas)
            {
                uiCanvas.gameObject.SetActive(false);
                uiCanvas = null;
            }
            KeyEvents._ke.SetUIActive(InteractionType.Place, InteractionType.Rotate);
            isHoldingSmall = true;
            grabbedObject = box;
            grabbedObject.transform.parent = null;
            grabbedObject.layer = LayerMask.NameToLayer("Ignore Raycast");
            previewObject = Instantiate(box, objGrabPos.position, Quaternion.identity);
            grabbedObject.GetComponent<Collider>().enabled = false;
            previewObject.AddComponent<Rigidbody>().isKinematic = true;
            previewObject.GetComponent<Collider>().enabled = true;
            previewObject.GetComponent<Collider>().isTrigger = true;
            previewObject.GetComponent<Renderer>().material = whiteMaterial;
            previewObject.AddComponent<PlacementPreview>();
        }

        void HandlePreviewHand()
        {
            grabbedObject.transform.position = Vector3.Lerp(grabbedObject.transform.position, objGrabPos.position, Time.deltaTime * lerpSpeed);
            grabbedObject.transform.rotation = Quaternion.Slerp(grabbedObject.transform.rotation, objGrabPos.rotation, Time.deltaTime * lerpSpeed);
            Ray ray = new Ray(cameraTransform.position, cameraTransform.forward);
            bool isPlaceable = false;
            Vector3 previewPosition;
            if (Physics.Raycast(ray, out RaycastHit hit, raycastDistance))
            {
                previewPosition = hit.point + new Vector3(0, .01f, 0);
                if (hit.collider.CompareTag("Box") && hit.collider.GetComponent<Box>().ItemCheck(grabbedObject.GetComponent<ItemObj>()._itemName))
                {
                    isPlaceable = true;
                    KeyEvents._ke.SetUIActive(InteractionType.Putin);
                    previewObject.GetComponent<Renderer>().material = blueMaterial;
                    if (Input.GetMouseButtonDown(0))
                    {
                        if (hit.collider.GetComponent<Box>()?.SetItem(grabbedObject) == true)
                        {
                            grabbedObject.layer = LayerMask.NameToLayer("Water");
                            Destroy(previewObject);
                            grabbedObject = null;
                            isHoldingSmall = false;
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
                        previewObject.GetComponent<Renderer>().material = blueMaterial;
                        if (Input.GetMouseButtonDown(0))
                        {
                            if (hit.collider.GetComponent<EggCooker>()?.SetItem(grabbedObject) == true)
                            {
                                grabbedObject.layer = LayerMask.NameToLayer("Water");
                                Destroy(previewObject);
                                grabbedObject = null;
                                isHoldingSmall = false;
                                KeyEvents._ke.SetUIActive(InteractionType.None);
                            }
                            else isPlaceable = false;
                        }
                    }
                }
                else
                {
                    isPlaceable = !previewObject.GetComponent<PlacementPreview>().HasCollision();
                    if (isPlaceable)
                    {
                        KeyEvents._ke.SetUIActive(InteractionType.Place, InteractionType.Rotate);
                        previewObject.GetComponent<Renderer>().material = whiteMaterial;
                    }
                    else
                    {
                        KeyEvents._ke.SetUIActive(InteractionType.None);
                        previewObject.GetComponent<Renderer>().material = redMaterial;
                    }
                }
                if (Input.GetMouseButtonDown(0) && isPlaceable && grabbedObject)
                {
                    grabbedObject.layer = LayerMask.NameToLayer("Water");
                    grabbedObject.transform.position = previewPosition;
                    grabbedObject.transform.rotation = previewObject.transform.rotation;
                    grabbedObject.GetComponent<Collider>().enabled = true;
                    Destroy(previewObject);
                    grabbedObject = null;
                    isHoldingSmall = false;
                    KeyEvents._ke.SetUIActive(InteractionType.None);
                }
            }
            else
            {
                previewPosition = cameraTransform.position + cameraTransform.forward * raycastDistance;
                isPlaceable = false;
                KeyEvents._ke.SetUIActive(InteractionType.None);
                previewObject.GetComponent<Renderer>().material = redMaterial;
            }
            previewObject.transform.position = previewPosition;
            float scrollInput = Input.mouseScrollDelta.y;
            if (scrollInput != 0) previewObject.transform.Rotate(Vector3.up, scrollInput * 15, Space.Self);
        }
    }
}