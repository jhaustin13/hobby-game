using UnityEngine;
using Assets.Scripts.Interactable;

namespace Assets.Scripts.Controllers
{
    public class FirstPersonController :  MonoBehaviour
    {
        private Animator animator;
        private Camera camera;
        private float movementSpeed;
        private float interationDistance = 1000f;

        private float mouseX;
        private float mouseY;

        private void Start()
        {
            mouseX = Input.mousePosition.x;
            mouseY = Input.mousePosition.y;
            animator = GetComponent<Animator>();
            camera = GetComponentInChildren<Camera>();
        }

        private void Update()
        {
            if (Input.GetKey(KeyCode.W))
            {
                if (movementSpeed < 2f)
                {
                    movementSpeed += .1f;
                }
            }
            else
            {
                if (movementSpeed >= 0)
                {
                    movementSpeed -= .5f;
                }
            }

            animator.SetFloat("FwdMovementSpeed", movementSpeed);
            HandleCameraMovement();

            //if(Input.GetKey(KeyCode.D))
            //{
            //    transform.Rotate(new Vector3(0, 1f, 0));
            //}

            //if (Input.GetKey(KeyCode.A))
            //{
            //    transform.Rotate(new Vector3(0, -1f, 0));
            //}

            //Left Mouse Button Down
            if (Input.GetMouseButtonDown(0))
            {
                RaycastHit hit;


                if (Physics.Raycast(camera.transform.position, camera.transform.forward, out hit, interationDistance))
                {
                    //if (hit.collider.tag == "plant")
                    //{
                    //    int hitTriangle = hit.triangleIndex;
                    //    var plantController = hit.collider.GetComponent<PlantController>();
                    //    float distance = float.MaxValue;
                    //    Mesh mesh = hit.collider.GetComponent<MeshFilter>().mesh;
                    //    Vector3 collisionRelativeToPlant = hit.point - hit.transform.position;
                    //    Vector3 closestVertex = mesh.vertices[0];
                    //    foreach (var vertex in mesh.vertices)
                    //    {
                    //        float calculatedDistance = Vector3.Distance(collisionRelativeToPlant, vertex);
                    //        if (calculatedDistance < distance)
                    //        {
                    //            distance = calculatedDistance;
                    //            closestVertex = vertex;
                    //        }
                    //    }

                    //    //int vertexIndex = plantData.MeshData.Triangles[hit.triangleIndex];
                    //    //var point = plantData.MeshData.Vertices[vertexIndex];
                    //    //point = point + new Vector3(0f, 10f, 0f);
                    //    plantController.Split(closestVertex);
                    //}

                    var interactable = hit.collider.GetComponent<Interactable.Interactable>();

                    if (interactable != null)
                    {
                        interactable.HandleLeftClick(GetComponent<PlayerController>(), hit);
                    }
                }
            }

            if (Input.GetMouseButtonDown(1))
            {
                RaycastHit hit;

                if (Physics.Raycast(camera.transform.position, camera.transform.forward, out hit, interationDistance))
                {
                    var interactable = hit.collider.GetComponent<Interactable.Interactable>();

                    if (interactable != null)
                    {
                        interactable.HandleRightClick(GetComponent<PlayerController>(), hit);
                    }
                }
            }

            if (Input.GetMouseButtonDown(2))
            {
                RaycastHit hit;

                if (Physics.Raycast(camera.transform.position, camera.transform.forward, out hit, interationDistance))
                {
                    var interactable = hit.collider.GetComponent<Interactable.Interactable>();

                    if (interactable != null)
                    {
                        interactable.HandleMiddleClick(GetComponent<PlayerController>(), hit);
                    }
                }
            }
        }

        private void OnGUI()
        {
            GUI.Box(new Rect(Screen.width / 2, Screen.height / 2, 2, 2), string.Empty);
        }

        private void HandleCameraMovement()
        {
            var deltaMouseX = mouseX - Input.mousePosition.x;
            var deltaMouseY = mouseY - Input.mousePosition.y;
            var eulerX = camera.transform.rotation.eulerAngles.x;

            //if ((((eulerX + deltaMouseY) % 360) >= 0 && eulerX + deltaMouseY <= 90) || (eulerX + deltaMouseY >= 270 && eulerX + deltaMouseY <= 360))            
            camera.transform.Rotate(new Vector3(deltaMouseY, 0f, 0f));
            
            
            transform.Rotate(new Vector3(0f, deltaMouseX * -1, 0f));

            mouseX = Input.mousePosition.x;
            mouseY = Input.mousePosition.y;
        }
    }
}
