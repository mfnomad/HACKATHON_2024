using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DoorFunctionality : MonoBehaviour
{
    public GameObject[] doors;
    public GameObject player;
    public TMP_Text interactText;
    public KeyCode interactionKey = KeyCode.E;
    public GameObject showImageWindow;
    public GameObject openDoorWindow;
    public GameObject openedDoorSprite;

    private bool isWindowShown = false;

    


    void CheckPlayerPositionCloseToDoors()
    {
        Vector3 playerPosition = player.transform.position;
        bool isPlayerNearDoor = false;



        foreach (var door in doors)
        {
            Vector3 doorPosition = door.transform.position;
            if (playerPosition.x >= doorPosition.x - 1f && playerPosition.x <= doorPosition.x + 1f &&
            playerPosition.y >= doorPosition.y - 1.5f && playerPosition.y <= doorPosition.y + 1.5f)
            {
                if (Input.GetKeyDown(interactionKey))
                {
                    //Player interacts with door by pressing E
                    Debug.Log("Player interacted with door:" + door.name);
                    isWindowShown = !isWindowShown;


                    showImageWindow.transform.position = new Vector3(doorPosition.x - 1.5f, doorPosition.y, doorPosition.z);
                    openDoorWindow.transform.position = new Vector3(doorPosition.x + 1.5f, doorPosition.y, doorPosition.z);
                    showImageWindow.SetActive(true);
                    openDoorWindow.SetActive(true);


                }
                //Check if openDoorWindow has been clicked on by player
                if (isWindowShown && Input.GetMouseButtonDown(0))
                {
                    bool isDoorOpenDoorClicked = false;
                    Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    Collider2D hitCollider = Physics2D.OverlapPoint(mousePosition);
                    
                    if (hitCollider != null && hitCollider.gameObject == openDoorWindow)
                    {
                        isDoorOpenDoorClicked = true;
                        //
                        changeDoorSpriteToOpen(door);
                    }
                }



                isPlayerNearDoor = true;
                break;

            }

            else
            {
                interactText.gameObject.SetActive(false);

            }

        }

        interactText.gameObject.SetActive(isPlayerNearDoor);
        showImageWindow.SetActive(isWindowShown);
        openDoorWindow.SetActive(isWindowShown);
    }

    // Start is called before the first frame update
    void Start()
    {
        interactText.gameObject.SetActive(false);
        showImageWindow.SetActive(false);
        openDoorWindow.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        CheckPlayerPositionCloseToDoors();

        
        

       
    }

    void changeDoorSpriteToOpen(GameObject door_)
    {
        
        GameObject openedDoor = GameObject.Find(door_.name);

        openedDoor = openedDoorSprite;
    }
}
