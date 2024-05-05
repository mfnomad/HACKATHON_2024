using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static IAController;
using static Gameplay;

public class DoorFunctionality : MonoBehaviour
{
    public GameObject[] doors;
    public GameObject player;
    public TMP_Text interactText;
    public KeyCode interactionKey = KeyCode.E;
    public GameObject showImageWindow;
    public GameObject openDoorWindow;
    public GameObject openedDoorSprite;

    // Vars for IAController
    private IAController iaController;
    public TMP_Text hint;
    public RawImage[] images;

    //Vars fro gameplay
    private Gameplay gameplay;

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
                    isWindowShown = !isWindowShown;


                    showImageWindow.transform.position = new Vector3(doorPosition.x - 1.5f, doorPosition.y, doorPosition.z);
                    openDoorWindow.transform.position = new Vector3(doorPosition.x + 1.5f, doorPosition.y, doorPosition.z);
                    showImageWindow.SetActive(isWindowShown);
                    openDoorWindow.SetActive(isWindowShown);


                }
                //Check if openDoorWindow has been clicked on by player
                if (isWindowShown && Input.GetMouseButtonDown(0))
                {
                    Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    Collider2D hitCollider = Physics2D.OverlapPoint(mousePosition);
                    
                    if (hitCollider != null && hitCollider.gameObject == openDoorWindow)
                    {
                        int selectedDoorIndex = 0;

                        if (door.name == "door1") {
                            selectedDoorIndex = 1;
                        } else if (door.name == "door2") {
                            selectedDoorIndex = 2;
                        } else if (door.name == "door3") {
                            selectedDoorIndex = 3;
                        }

                        gameplay.NextTrial(iaController.getNumCorrecta() == selectedDoorIndex);
                        
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
        iaController = GetComponent<IAController>();
        iaController.getText(hint, images[0], images[1], images[2]);
        gameplay = GetComponent<Gameplay>();
        gameplay.HideAssets();
    }

    // Update is called once per frame
    void Update()
    {
        CheckPlayerPositionCloseToDoors();

        
        

       
    }

    void changeDoorSpriteToOpen(GameObject door_)
    {
        // Get the SpriteRenderer component of the door
        SpriteRenderer doorRenderer = door_.GetComponent<SpriteRenderer>();

        // Assign the opened door sprite to the SpriteRenderer
        doorRenderer.sprite = openedDoorSprite.GetComponent<SpriteRenderer>().sprite;
    }
}
