using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class RaycastLaserPointer : MonoBehaviour
{
    public float rayWidth = 0.01f;
    public float rayLengthMax = 50f;
    public float maxObjRotateSpeed = 100f;
    public LineRenderer lineRndr;
    public LayerMask rayMask;
    
    private OvrAvatar ovrAvatar;
    private OVRPlayerController ovrPlayerController;
    private RaycastHit hit;
    private Transform currentTransform;
    private float length;
    
    // Start
    public void Start()
    {
        Vector3[] initLaserPos = new Vector3[2] { Vector3.zero, Vector3.zero };
        lineRndr.SetPositions(initLaserPos);
        lineRndr.startWidth = rayWidth;
        lineRndr.endWidth = rayWidth/2;
        lineRndr.material.color = Color.white;
        ovrAvatar = FindObjectOfType<OvrAvatar>();
        ovrPlayerController = FindObjectOfType<OVRPlayerController>();
    }
    
    // Update
    public void Update()
    {
        if (OVRInput.Get(OVRInput.RawButton.RHandTrigger))
        {
            Debug.Log("Pointer button pressed!");
            
            lineRndr.material.color = Color.white;
            
            PointRaycast(ovrAvatar.HandRight.transform.position, ovrAvatar.HandRight.transform.forward, rayLengthMax, rayMask);
            //text.text = "Laser initialized!";
            //Debug.Log("Laser initialized!");
            lineRndr.enabled = true;
            
            if (currentTransform)
            {
                MoveObjectAround();
            }
        }
        
        else
        {
            //text.text = "";
            lineRndr.enabled = false;
        }
    }

    // Pointer laser
    void PointRaycast(Vector3 _targetPos, Vector3 _dir, float _length, LayerMask _layerMask)
    {
        Ray ray =  new Ray(_targetPos, _dir);
        Vector3 endPos = _targetPos + (length * _dir);
        if (Physics.Raycast(ray, out RaycastHit raycastHit, _length, _layerMask))
        {
            endPos = raycastHit.point;
            
            if (raycastHit.transform.CompareTag("MovableObj"))
            {
                if (OVRInput.GetDown(OVRInput.RawButton.RIndexTrigger))
                {
                    //Pick up obj
                    PickUpObject(raycastHit.transform);

                    //Disable player movement
                    ovrPlayerController.EnableRotation = false;
                }
                else if (OVRInput.GetUp(OVRInput.RawButton.RIndexTrigger))
                {
                    //Release obj
                    ReleaseObject();
                    
                    //Enable player movement
                    ovrPlayerController.EnableRotation = true;
                    //ovrPlayerController.EnableLinearMovement = true;
                }
            }
            else if (raycastHit.transform.CompareTag("RayButton"))
            {
                lineRndr.material.color = Color.red;
                
                if (raycastHit.transform.name.Equals("RayButtonMoveFurniture"))
                {
                    if (OVRInput.GetDown(OVRInput.RawButton.RIndexTrigger))
                    {
                        //Jump to Move furniture scene
                        SceneManager.LoadScene (sceneName:"Scenes/MoveFurnitureScene");
                    }
                }
                else if (raycastHit.transform.name.Equals("RayButtonAssembleTable"))
                {
                    if (OVRInput.GetDown(OVRInput.RawButton.RIndexTrigger))
                    {
                        //Jump to Assemble table scene
                        SceneManager.LoadScene(sceneName: "Scenes/AssembleTableScene");
                    }
                }
                else if (raycastHit.transform.name.Equals("RayButtonExitRoom"))
                {
                    if (OVRInput.GetDown(OVRInput.RawButton.RIndexTrigger))
                    {
                        //Exit to main menu
                        SceneManager.LoadScene(sceneName: "Scenes/MenuScene");
                    }
                }
            }
        }
        
        lineRndr.SetPosition(0, _targetPos);
        lineRndr.SetPosition(1, endPos);
    }

    public void PickUpObject(Transform _transform)
    {
        if (currentTransform)
            return;

        currentTransform = _transform;
        length = Vector3.Distance(transform.position, _transform.position);
        currentTransform.GetComponent<Rigidbody>().isKinematic = true;
        lineRndr.material.color = Color.yellow;
    }

    void MoveObjectAround()
    {
        currentTransform.position = transform.position + transform.forward * length;
        //currentTransform.rotation = transform.rotation;
        
        //Get right hand thumbstick X axis pos, range[-1,1]
        Vector2 vec2 = OVRInput.Get(OVRInput.Axis2D.SecondaryThumbstick);

        if (vec2.x < 0)
        {
            // Rotate obj left
            currentTransform.transform.RotateAround(currentTransform.transform.position, Vector3.up, (vec2.x * (-maxObjRotateSpeed)) * Time.deltaTime);
        }
        
        else if (vec2.x > 0)
        {
            // Rotate obj right
            currentTransform.transform.RotateAround(currentTransform.transform.position, Vector3.down, (vec2.x * maxObjRotateSpeed) * Time.deltaTime);
        }
    }

    public void ReleaseObject()
    {
        if (!currentTransform)
            return;
        
        currentTransform.GetComponent<Rigidbody>().isKinematic = false;
        currentTransform = null;
        
        lineRndr.material.color = Color.white;
    }
}
