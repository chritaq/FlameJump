using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingBridge : MonoBehaviour
{
    private Vector3[] bridgePiecesPlacedPosition;
    private Vector3[] bridgePiecesFloatPosition;
    private Quaternion[] bridgePiecesPlacedRotation;
    private Quaternion[] bridgePiecesFloatRotation;
    private Color placedColor;
    [SerializeField] private Color floatColor;

    [SerializeField] private GameObject[] bridgePieces;
    private SpriteRenderer[] spriteRenderers;

    private float distanceToPlayer;
    private float distanceToPlayerBeforeMove = 24;

    PlayerController player;

    private bool stop = false;
    private bool start = true;


    // Start is called before the first frame update
    void Start()
    {
        placedColor = bridgePieces[0].GetComponent<SpriteRenderer>().color;
        bridgePiecesPlacedPosition = new Vector3[bridgePieces.Length];
        bridgePiecesFloatPosition = new Vector3[bridgePieces.Length];
        bridgePiecesPlacedRotation = new Quaternion[bridgePieces.Length];
        bridgePiecesFloatRotation = new Quaternion[bridgePieces.Length];
        spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
        for (int i = 0; i < bridgePieces.Length; i++)
        {
            bridgePiecesPlacedPosition[i] = bridgePieces[i].transform.position;
            bridgePiecesFloatPosition[i] = RandomizeBridgePiecePositions(bridgePieces[i]);
            bridgePiecesPlacedRotation[i] = bridgePieces[i].transform.rotation;
            bridgePiecesFloatRotation[i] = Random.rotation;
        }

        player = PlayerController.playerInstance;
    }


    private Vector3 RandomizeBridgePiecePositions(GameObject bridgePiece)
    {
        Vector3 randomAddition = Random.insideUnitCircle * 7.5f;
        Vector3 pos = bridgePiece.transform.position;
        bridgePiece.transform.position = randomAddition + pos;
        return bridgePiece.transform.position;
    }

    [SerializeField] private bool debugLerp;
    // Update is called once per frame
    void Update()
    {
        //if (stop) return;

        distanceToPlayer = Mathf.Abs(player.transform.position.x - transform.position.x) - 3;

        if(distanceToPlayer < distanceToPlayerBeforeMove)
        {
            start = true;
        }
        else
        {
            start = false;
        }

        if(start)
        {
            //Convert 0 and 200 distance range to 0f and 1f range
            float lerp = mapValue(distanceToPlayer, 2, distanceToPlayerBeforeMove, 0f, 1f);

            if (debugLerp)
            {
                Debug.Log(lerp);
            }

            for (int i = 0; i < bridgePieces.Length; i++)
            {
                bridgePieces[i].transform.position = Vector3.Lerp(bridgePiecesPlacedPosition[i], bridgePiecesFloatPosition[i], lerp);
                bridgePieces[i].transform.rotation = Quaternion.Lerp(bridgePiecesPlacedRotation[i], bridgePiecesFloatRotation[i], lerp);
                spriteRenderers[i].color = Color.Lerp(placedColor, floatColor, (lerp + 0.4f));
            }

            if (lerp <= 0)
            {
                stop = true;
            }

        }

        

        //transform.position = Vector3.Lerp(transform.position, bridgePiecesPlacedPosition, lerp);
    }

    float mapValue(float mainValue, float inValueMin, float inValueMax, float outValueMin, float outValueMax)
    {
        return (mainValue - inValueMin) * (outValueMax - outValueMin) / (inValueMax - inValueMin) + outValueMin;
    }

}
