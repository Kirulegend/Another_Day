using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;

namespace ITISKIRU
{
    public class Plates : MonoBehaviour, Interactable
    {
        [SerializeField] GameObject plate;
        [SerializeField] int defaultCount = 10;
        [SerializeField] List<GameObject> plates;
        [SerializeField] BoxCollider BC;
        [SerializeField] Transform _canvasPoint;

        public void OnInteract(int Mouse, Transform Script)
        {
            if (Mouse == 0 && plates.Count > 0)
            {
                GameObject lastPlate = plates[plates.Count - 1];
                plates.RemoveAt(plates.Count - 1);
                Script.GetComponent<Player>().GrabObjHand(lastPlate);
                BC.size = new Vector3(BC.size.x, 0.03f * defaultCount, BC.size.z);
                BC.center = new Vector3(BC.center.x, 0.015f * defaultCount, BC.center.z);
                _canvasPoint.transform.localPosition = new Vector3(0, Mathf.Clamp(0.1f * plates.Count, .5f, 2.5f), 0);
            }
        }

        public void OnInteractHand(Transform Item)
        {
            
        }

        void Start()
        {
            for (int i = 0; i < defaultCount; i++)
            {
                GameObject Plate = Instantiate(plate, transform);
                Plate.transform.localPosition = new Vector3(0, 0.025f * i, 0);
                plates.Add(Plate);
            }
            BC.size = new Vector3(BC.size.x, 0.03f * defaultCount, BC.size.z);
            BC.center = new Vector3(BC.center.x, 0.015f * defaultCount, BC.center.z);
            _canvasPoint.transform.localPosition = new Vector3(0, .75f + Mathf.Clamp(0.03f * plates.Count, .5f, 2.5f), 0);
        }
        void OnMouseEnter()
        {
            GameManager.gM.Set_boxUI("Plate Stack", "Plates : " + plates.Count, _canvasPoint.position);
            if (plates.Count > 0) KeyEvents._ke.SetUIActive(InteractionType.Take);
        }
        void OnMouseExit() => GameManager.gM.Off_boxUI();
    }
}
