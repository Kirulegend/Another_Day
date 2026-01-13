using System;
using System.Collections;
using System.Collections.Generic;
using System.Security;
using UnityEngine;
namespace ITISKIRU
{
    public class EggCooker : MonoBehaviour, Interactable
    {
        [SerializeField] Transform cap;
        [SerializeField] float openAngle = -127f;
        [SerializeField] float speed = 60f;
        [SerializeField] bool isMoving = false;
        public bool isOpen = false;
        public bool isCooking = false;
        [SerializeField] List<Spot> spots = new List<Spot>();
        [SerializeField] int _quantity = 0;
        [SerializeField] Coroutine cookingCoroutine;
        [SerializeField] float cookDuration = 10f;
        [SerializeField] ParticleSystem smoke;
        public Transform _canvasPoint;
        [SerializeField] Material defaultMat;
        [SerializeField] Material boiledMat;

        void Start()
        {
            foreach (Transform child in transform)
            {
                if (child.name == "Spot")
                {
                    Spot newSpot = new Spot
                    {
                        _spot = child,
                        _isOccupied = false
                    };
                    spots.Add(newSpot);
                }
            }
            spots.Reverse();
        }
        void OnMouseEnter()
        {
            GameManager.gM.Set_boxUI("Egg Boiler", GetData(), _canvasPoint.position);
            if (!Player.isHolding && !Player.isHoldingHand)
            {
                if (!isMoving && isOpen && !isCooking) KeyEvents._ke.SetUIActive(InteractionType.Take, InteractionType.Close);
                else if (!isMoving && !isOpen && !isCooking) KeyEvents._ke.SetUIActive(InteractionType.Boil, InteractionType.Open);
                else if (isCooking) KeyEvents._ke.SetUIActive(InteractionType.End);
            }
        }
        void OnMouseExit() => GameManager.gM.Off_boxUI();
        public void OC(bool isOpenC)
        {
            if (!Player.isHolding && !Player.isHoldingHand)
            {
                if (!isCooking)
                {
                    if (isOpenC == isOpen)
                    {
                        float targetAngle = isOpen ? 0f : openAngle;
                        StartCoroutine(RotateCap(targetAngle));
                        isOpen = !isOpen;
                    }
                }
                else Boil();
                if (!isMoving && isOpen && !isCooking) KeyEvents._ke.SetUIActive(InteractionType.Take, InteractionType.Close);
                else if (!isMoving && !isOpen && !isCooking) KeyEvents._ke.SetUIActive(InteractionType.Boil, InteractionType.Open);
                else if (isCooking) KeyEvents._ke.SetUIActive(InteractionType.Open);
            }
        }

        IEnumerator RotateCap(float targetAngle)
        {
            isMoving = true;
            Quaternion startRot = cap.localRotation;
            Quaternion endRot = Quaternion.Euler(targetAngle, 0f, 0f);
            float duration = Quaternion.Angle(startRot, endRot) / speed;
            float elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / duration);
                cap.localRotation = Quaternion.Slerp(startRot, endRot, t);
                yield return null;
            }
            cap.localRotation = endRot;
            isMoving = false;
        }
        public void Boil()
        {
            if (cookingCoroutine != null)
            {
                StopCoroutine(cookingCoroutine);
                isCooking = false;
                cookingCoroutine = null;
                smoke.Stop();
            }
            else if (_quantity > 0) cookingCoroutine = StartCoroutine(CookingTimer());
        }

        IEnumerator CookingTimer()
        {
            float timer = 0f;
            KeyEvents._ke.SetUIActive(InteractionType.End);
            smoke.Play();
            isCooking = true;
            Debug.Log("Boiling");
            while (timer < cookDuration)
            {
                timer += Time.deltaTime;
                foreach (Spot spot in spots) if (spot._isOccupied && spot._spot.childCount > 0) spot._spot.GetChild(0).GetComponent<Renderer>().sharedMaterial.Lerp(defaultMat, boiledMat, timer / cookDuration);
                yield return null;
            }
            isCooking = false;
            OC(false);
            foreach (Spot spot in spots) if (spot._isOccupied && spot._spot.childCount > 0) spot._spot.GetChild(0).GetComponent<ItemObj>()._status = "Boiled";
            cookingCoroutine = null;
            smoke.Stop();
        }
        public GameObject GetItem()
        {
            if (isOpen)
            {
                foreach (Spot spot in spots) if (spot._isOccupied && spot._spot.childCount > 0)
                    {
                        spot._isOccupied = false;
                        _quantity--;
                        return spot._spot.GetChild(0).gameObject;
                    }
            }
            return null;
        }
        public bool SetItem(GameObject item)
        {
            for (int i = spots.Count - 1; i >= 0; i--)
            {
                Spot spot = spots[i];
                if (!spot._isOccupied)
                {
                    spot._isOccupied = true;
                    _quantity++;
                    item.transform.SetParent(spot._spot);
                    item.transform.localPosition = Vector3.zero;
                    item.transform.localRotation = Quaternion.identity;
                    KeyEvents._ke.SetUIActive(InteractionType.Take, InteractionType.Close);
                    return true;
                }
            }
            return false;
        }
        public string GetData()
        {
            if (isCooking) return "Cooking";
            else return "Idle";
        }

        public void OnInteract(int Num, Transform Player)
        {
            Debug.Log("OnInteract");
            if (Num == 0)
            {
                if (isOpen)
                {
                    GameObject Temp = GetItem();
                    if (Temp) Player.GetComponent<Player>().GrabObjHand(Temp);
                }
                else OC(false);
            }
            if (Num == 1)
            {
                if (isOpen) OC(true);
                else Boil();
            }
        }
        public void OnInteractHand(Transform T)
        {

        }
    }
}