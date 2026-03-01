using Unity.VisualScripting;
using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
public class PlayerController : MonoBehaviour
{
    public float speed = 6f;
    public float jumpForce = 8f;
    public float gravity = -20f;
    public float mouseSensitivity = 200f;

    public Material originalSkybox;
    public Light directionalLight;
    public Light spotLight;
    public Player_Interaction interactScript;

    public GameObject normalTerrain;
    public GameObject glitchTerrain;

    private CharacterController controller;
    private Vector3 velocity;
    private float xRotation = 0f;
    private Camera cam;
    private Rigidbody rb;


    void Start()
    {
        //add sound effect (starting)

        controller = GetComponent<CharacterController>();
        cam = GetComponentInChildren<Camera>();
        Cursor.lockState = CursorLockMode.Locked;

        rb = GetComponent<Rigidbody>();

        FindObjectOfType<NarratorManager>().PlayLine(0);
    }

    void Update()
    {
        // -------- MOUSE LOOK --------
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -80f, 80f);

        cam.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);

        // -------- MOVEMENT --------
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = transform.right * x + transform.forward * z;

        if (controller.isGrounded)
        {
            if (velocity.y < 0)
                velocity.y = -2f;

            if (Input.GetKeyDown(KeyCode.Space))
            {
                velocity.y = jumpForce;
            }
        }

        velocity.y += gravity * Time.deltaTime;

        Vector3 finalMove = move * speed + new Vector3(0, velocity.y, 0);
        controller.Move(finalMove * Time.deltaTime);


        //logic for interaction of buttons
       if (interactScript.pink)
        {
            normalTerrain.SetActive(false);
            glitchTerrain.SetActive(true);

            //add voice lines
            FindObjectOfType<NarratorManager>().PlayLine(4);

            interactScript.pink = false;
            
        }
        if (interactScript.red)
        {
            FindObjectOfType<NarratorManager>().PlayLine(7);

            //skybox
            RenderSettings.skybox = originalSkybox;
            RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Skybox; // Switch back from Flat

            //Fix the camera
            Camera.main.clearFlags = CameraClearFlags.Skybox;

            //fog
            RenderSettings.fog = false;

            //light
            directionalLight.enabled = true;
            spotLight.GetComponent<Light>().enabled = false;


            interactScript.red = false;
        }



        if (interactScript.fall)
        {
            // Turn on the rigidbody component
            //since character controller is also on, this makes the player have "2 brains" so he falls down
            Debug.Log("cube pressed");
            controller.enabled = false;
            rb.isKinematic = false;
            rb.useGravity = true;


            // Add a "glitchy" random shove to make the fall funnier
            Vector3 randomPush = new Vector3(Random.Range(-5, 5), 2, Random.Range(-5, 5));
            rb.AddForce(randomPush, ForceMode.Impulse);

            // Add some spin!
            rb.AddTorque(Random.insideUnitSphere * 10f, ForceMode.Impulse);

       
            StartCoroutine(funnyfall());


            interactScript.fall = false;
        }

        //to reset player rotation //put a voiceline telling the player to press R
        //fix this to make the quaternion line only for the x and y rotations
        if (Input.GetKeyDown(KeyCode.R) && Quaternion.Angle(transform.rotation, Quaternion.identity) > 1f)
        {
            if (transform.position.y < -50f)
            {
                controller.enabled = false;
                transform.position = new Vector3(10f, -34.39f, 0f);
            }

            controller.enabled = false;
            transform.rotation = Quaternion.identity;
            controller.enabled = true;
            rb.isKinematic = true;
            rb.useGravity = false;
           
        }

    }

    IEnumerator funnyfall()
    {
        yield return new WaitForSeconds(4);
        //voice line here
        //"wow look what you have done"
        FindObjectOfType<NarratorManager>().PlayLine(9);
        //press r if you wanna stand up
        
    }

    //interaction: falling down the world
    void OnTriggerEnter(Collider other)
    {
        //respawn
        if (other.gameObject.CompareTag("Bounds"))
        {
            controller.enabled = false;
            transform.position = new Vector3(10, -34.39f, 0);
            controller.enabled = true;
            Debug.Log("hit");
            int index = Random.Range(2, 4);
            FindObjectOfType<NarratorManager>().PlayLine(index);
            //A LOT OF DIFFERENT VOICELINES HERE
            //VOICELINES SHOULD CHANGE EVERYTIME
        }

        //clip through
        if (other.gameObject.CompareTag("Special"))
        {
            FindObjectOfType<NarratorManager>().PlayLine(1);
            StartCoroutine(clipThrough());
        }

        if(other.gameObject.CompareTag("trigger"))
        {
            FindObjectOfType<NarratorManager>().PlayLine(8);
        }
        if(other.gameObject.CompareTag("triggger2"))
        {
            FindObjectOfType<NarratorManager>().PlayLine(10);
            StartCoroutine(endScreen());
        }
        
    }

    IEnumerator clipThrough()
    {
        controller.enabled = false;
        transform.position = new Vector3(transform.position.x, -1, transform.position.y);
        controller.enabled = true;
        yield return new WaitForSeconds(0.4f);
        controller.enabled = false;
        transform.position = new Vector3(transform.position.x, -1, transform.position.y);
        controller.enabled = true;

        yield return new WaitForSeconds(0.4f);
        controller.enabled = false;
        transform.position = new Vector3(transform.position.x, -1, transform.position.y);
        controller.enabled = true;
        yield return new WaitForSeconds(0.4f);
        controller.enabled = false;
        transform.position = new Vector3(transform.position.x, -1, transform.position.y);
        controller.enabled = true;

         FindObjectOfType<NarratorManager>().PlayLine(1);

        yield return new WaitForSeconds(0.4f);
        controller.enabled = false;
        transform.position = new Vector3(transform.position.x, -1, transform.position.y);
        controller.enabled = true;
        yield return new WaitForSeconds(0.4f);
        controller.enabled = true;


        //Turn everything black
        yield return new WaitForSeconds(1f);
        //skybox
        RenderSettings.skybox = null;
        RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;
        RenderSettings.ambientLight = Color.black;
        //directional lighting
        if (directionalLight != null)
        {
            directionalLight.GetComponent<Light>().enabled = false;
        }
        //camera
        //add a delay for 0.5 seconds? and a add soundeffect when he clips through the floor...glitch sound effect? 
        Camera.main.clearFlags = CameraClearFlags.SolidColor;
        Camera.main.backgroundColor = Color.black;

        // Turn on Fog to hide the edges of your "box"
        RenderSettings.fog = true;
        RenderSettings.fogColor = Color.black;
        RenderSettings.fogDensity = 0.08f;

        //mehmood sahab face when screen glitching

        //turn spotlight back on in the dark
        //put a voiceline here to make it seem like the narrator turned the spotlight on

        yield return new WaitForSeconds(4);
        if (spotLight != null)
        {
            spotLight.GetComponent<Light>().enabled = true;
        }
         FindObjectOfType<NarratorManager>().PlayLine(8);
    }

    IEnumerator endScreen()
    {
        yield return new WaitForSeconds(4);
        SceneManager.LoadScene("Level2");
        
    }

}