using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCController : MonoBehaviour
{
    //private PlayerController player;
    private GameObject player;

    private DialougeManagerV2 dialougeManager;
    [SerializeField] private Dialouge dialouge;

    [SerializeField] private float xDistanceBeforeDialogeCanBeActive;
    private float yDistanceBeforeDialogeCanBeActive = 0.1f;

    // Start is called before the first frame update
    void Start()
    {
        dialougeManager = DialougeManagerV2.instance;
        player = GameObject.FindGameObjectWithTag("Player");
    }

    private float xDistance;
    private float yDistance;
    public void StartDialouge()
    {
        xDistance = transform.position.x - player.transform.position.x;
        yDistance = transform.position.y - player.transform.position.y;
        if(xDistance < xDistanceBeforeDialogeCanBeActive && yDistance < yDistanceBeforeDialogeCanBeActive)
            dialougeManager.StartDialouge(dialouge);
    }
}
