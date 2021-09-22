using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{


    public Transform PlayerTransform;
    private Vector3 cameraOffset;
    public Vector3 center;

    private bool completed;
    private Vector3 newPos;
    private Vector3 newCenter;
    private bool centered;
    private bool arrived;
    private int stopDistance;

    private float speed;
    private float zoomSpeed;
    private float rotateSpeed;
    private float rotateTime;
    private float camMoveSpeed;

    private Vector3 velocity = Vector3.zero;

    public bool COSelected;

    private float maxZoom;
    private float minZoom;

    //UI
    public GameObject UIContainer;


    //inertia things
    private Vector3 prevPos = Vector3.zero;
    private Vector3 frameVelocity;
    private Vector3 curVelocity;
    private bool underInertia;
    private bool isRotation;
    private float smoothTime = 3;
    private float time = 0.0f;



    // Start is called before the first frame update
    void Start()
    {
        completed = true;
        center = PlayerTransform.position;
        maxZoom = 500;
        minZoom = 20;
        speed = 300;
        zoomSpeed = 50;
        rotateSpeed = 0.2f;
        rotateTime = 0;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void LateUpdate() {

        if(!COSelected)
        {
            float scroll = Input.GetAxis ("Mouse ScrollWheel");
            if(Vector3.Distance(transform.position, center) > minZoom && scroll > 0 || Vector3.Distance(transform.position, center) < maxZoom && scroll < 0) transform.Translate(0, 0, scroll * zoomSpeed, Space.Self);
        }
        cameraOffset = transform.position - center;
    
        if(Input.GetKey(KeyCode.Mouse1) && completed == true)
        {

            underInertia = false;
            time = 0.0f;


            Vector3 newPos = (transform.right * Input.GetAxis("Mouse X") * -speed );
            newPos += (transform.up * Input.GetAxis("Mouse Y") * -speed);
            
            transform.Translate(newPos * Time.deltaTime, Space.World);
            center = transform.position - cameraOffset;
            transform.LookAt(center);
            
            curVelocity = (transform.position - prevPos) / Time.deltaTime;
            frameVelocity = Vector3.Lerp(frameVelocity, curVelocity, 0.1f) / 1.5f;
            prevPos = transform.position;
            
        }

        


        if(Input.GetKey(KeyCode.Mouse0) && completed == true)
        {
            underInertia = false;
            time = 0.0f;
            


            Quaternion camTurnAngle = Quaternion.AngleAxis(Input.GetAxis("Mouse X") * 5.0f, Vector3.up);
            double signX = cameraOffset.x;
            double signZ = cameraOffset.z;
            
            double cameraMaxY = Vector3.Distance(Vector3.zero, cameraOffset) * 95 / 100;
            double cameraMinY = -cameraMaxY;
            
            if(cameraOffset.y > 0 && ( cameraOffset.y < cameraMaxY || Input.GetAxis("Mouse Y") > 0) || cameraOffset.y < 0 && (cameraOffset.y > cameraMinY || Input.GetAxis("Mouse Y") < 0)) //prevent the camera from going to far up and down (caused jitter)
            {
                //bug on the z axis without this
                if(signZ * signX < 0) 
                {
                    if(signZ > 0)
                    {
                        camTurnAngle *= Quaternion.AngleAxis(Input.GetAxis("Mouse Y") * 5.0f * rotateSpeed, Vector3.right);
                        camTurnAngle *= Quaternion.AngleAxis(Input.GetAxis("Mouse Y") * -5.0f * rotateSpeed, Vector3.back);
                    }
                    else
                    {
                        camTurnAngle *= Quaternion.AngleAxis(Input.GetAxis("Mouse Y") * -5.0f * rotateSpeed, Vector3.right);
                        camTurnAngle *= Quaternion.AngleAxis(Input.GetAxis("Mouse Y") * 5.0f * rotateSpeed, Vector3.back);
                    }
                    
                    
                }
                else 
                {
                    if(signZ > 0)
                    {
                        camTurnAngle *= Quaternion.AngleAxis(Input.GetAxis("Mouse Y") * 5.0f * rotateSpeed, Vector3.right);
                        camTurnAngle *= Quaternion.AngleAxis(Input.GetAxis("Mouse Y") * 5.0f * rotateSpeed, Vector3.back);
                    }
                    else 
                    {
                        camTurnAngle *= Quaternion.AngleAxis(Input.GetAxis("Mouse Y") * -5.0f * rotateSpeed, Vector3.right);
                        camTurnAngle *= Quaternion.AngleAxis(Input.GetAxis("Mouse Y") * -5.0f * rotateSpeed, Vector3.back);
                    }
                    
                }
                
            }
            cameraOffset = camTurnAngle * cameraOffset;
            
            Vector3 newPos = center + cameraOffset;
            transform.position = Vector3.Slerp(transform.position, newPos, 0.5f);
            transform.LookAt(center);
            
            curVelocity = (transform.position - prevPos) /Time.deltaTime;
            frameVelocity = Vector3.Lerp(frameVelocity, curVelocity, 0.1f) / 1.5f;
            prevPos = transform.position;
            if(Input.GetAxis("Mouse X") > 0 && frameVelocity.x > 0 || Input.GetAxis("Mouse X") < 0 && frameVelocity.x < 0) frameVelocity.x = -frameVelocity.x;
        }


        if(underInertia && time < 1)
        {
            if(isRotation)
            {    
                double cameraMaxY = Vector3.Distance(Vector3.zero, cameraOffset) * 95 / 100;
                double cameraMinY = -cameraMaxY;

                if(cameraOffset.y > 0 && ( cameraOffset.y < cameraMaxY || -frameVelocity.y > 0) || cameraOffset.y < 0 && (cameraOffset.y > cameraMinY || -frameVelocity.y < 0))
                {
                    float distance = Vector3.Distance(Vector3.zero, cameraOffset);
                    transform.position = center;
                    transform.Rotate(Vector3.right, frameVelocity.y / 1.8f / (distance / 5));
                    transform.Rotate(Vector3.up, -frameVelocity.x / 1.8f / (distance / 5), Space.World);
                    transform.Translate(new Vector3(0,0, -distance));

                    
                    
                }
                
                float t = time / smoothTime;
                t = 1f - Mathf.Cos(t * Mathf.PI * 0.5f);
                frameVelocity = Vector3.Lerp(frameVelocity, Vector3.zero, t);
                time += Time.smoothDeltaTime;
                
                
            }
            else
            {
                transform.position = Vector3.Lerp(transform.position, transform.position + frameVelocity, 0.1f);
                center = Vector3.Lerp(center, center + frameVelocity, 0.1f);

                float t = time / smoothTime;
                t = 1f - Mathf.Cos(t * Mathf.PI * 0.5f);
                frameVelocity = Vector3.Lerp(frameVelocity, Vector3.zero, t);
                time += Time.smoothDeltaTime;
                
            }
            
        }
        else
        {
            underInertia = false;
            time = 0.0f;
             
        }


        if(!completed)
        {
            if(Vector3.Distance(transform.position, newPos) < stopDistance) arrived = true;
            MooveToPos();
            if(centered && arrived)
            {
                completed = true;
                center = newCenter;
                rotateTime = 0;
            } 
            
        }
        

        if(Input.GetMouseButtonUp(1))
        {
            underInertia = true;
            isRotation = false;
        }
        if(Input.GetMouseButtonUp(0))
        {
            underInertia = true;
            isRotation = true;
        }

    }
    

    public void SelectSystem(GameObject gameObject)
    {
        centered = false;
        arrived = false;
        completed = false;

        newPos = gameObject.transform.position;
        newCenter = newPos;
        
        camMoveSpeed = Vector3.Distance(transform.position, newPos) * 2;
        stopDistance = 75;
        
        
    }
    public void EnterSystem(GameObject gameObject)
    {
        underInertia = false;
        time = 0;

        centered = false;
        arrived = false;
        completed = false;
        
        float zOffset = transform.position.z - gameObject.transform.position.z;
        newPos = gameObject.transform.position + new Vector3(0, 5, 5 * Mathf.Clamp(zOffset, -1, 1));
        newCenter = gameObject.transform.position;

        camMoveSpeed = Vector3.Distance(transform.position, newPos) * 2;
        speed = 20;
        zoomSpeed = 3;
        maxZoom = 40;
        minZoom = 1;
        stopDistance = 5;
        
        gameObject.GetComponent<SpriteRenderer>().enabled = false;
        gameObject.GetComponent<SystemApparenceKeeper>().enabled = false;


    }

    public void ExitSystem(GameObject gameObject)
    {
        centered = false;
        arrived = false;
        completed = false;

        newPos = transform.TransformPoint(0, 0, -50);
        newCenter = gameObject.transform.position;

        camMoveSpeed = Vector3.Distance(transform.position, newPos) * 2;
        maxZoom = 500;
        minZoom = 20;
        speed = 300;
        zoomSpeed = 50;
        stopDistance = 1;

        GameObject.Destroy(gameObject.transform.GetChild(0).gameObject);
        gameObject.GetComponent<SpriteRenderer>().enabled = true;
        gameObject.GetComponent<SystemApparenceKeeper>().enabled = true;
    }

    public void MoveToCO(GameObject gameObject)
    {
        underInertia = false;
        time = 0;

        centered = false;
        arrived = false;
        completed = false;

        COSelected = true;

        stopDistance = 2;
        newPos = gameObject.transform.position;
        newCenter = newPos;

        camMoveSpeed = Vector3.Distance(transform.position, gameObject.transform.position) * 2;

        stopDistance = 1;

        
    }
    
    public void MooveToPos()
    {
        if(!arrived)
        {
            transform.position = Vector3.MoveTowards(transform.position, newPos, camMoveSpeed * Time.deltaTime);

        }
            
        if(!centered)
        {
            float t = rotateTime / smoothTime;
            t = 1f - Mathf.Cos(t * Mathf.PI * 0.5f);

            var newPosRotation = Quaternion.LookRotation(newCenter - transform.position);
            transform.rotation = Quaternion.Lerp(transform.rotation, newPosRotation, t);

            rotateTime += Time.smoothDeltaTime;

            if(newPosRotation == transform.rotation) centered = true;
        }
        else
        {
            transform.LookAt(newCenter);
        }
           
    }


}
