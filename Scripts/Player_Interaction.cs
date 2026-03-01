using UnityEngine;

public class Player_Interaction : MonoBehaviour
{
    public float interactRange = 3f; // How far you can reach
    public GameObject interactUI;    // Drag your "Press F" text here
    public LayerMask interactableLayer; // Set this to a specific layer for buttons

    public bool pink = false;
    public bool red = false;
    public bool fall = false;
    private bool VOtrig = true;

    void Update()
    {
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;
        

        if (Physics.Raycast(ray, out hit, 20f, interactableLayer))
        {
            
            string hitName = hit.collider.gameObject.name;
            if (hit.collider.CompareTag("Cursed") && VOtrig == true)
            {
                //put voiceline here: "NA KARO NA KARO"
                FindObjectOfType<NarratorManager>().PlayLine(5);
                Debug.Log("warning here");
                VOtrig = false;
            }
        }

        // Check if we are looking at something interactable
        if (Physics.Raycast(ray, out hit, interactRange, interactableLayer))
        {
            interactUI.SetActive(true); // Show the "Press F" UI
            // Check the name of the SPECIFIC object the laser hit


            if (Input.GetKeyDown(KeyCode.F))
            {
                string hitName = hit.collider.gameObject.name;
                Debug.Log("Successfully interacted with: " + hitName);

                if (hitName == "Pink Button")
                {
                    pink = true;
                    Debug.Log("Pink button pressed!");
                }
                else if (hitName == "Red Button")
                {
                    red = true;
                    Debug.Log("Red button pressed!");
                }
                else if (hit.collider.CompareTag("Cursed"))
                {
                    fall = true;

                }
            }
        }
        else
        {
            interactUI.SetActive(false); // Hide the UI if we look away
        }
    }
}