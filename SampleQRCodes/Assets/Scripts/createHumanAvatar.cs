using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class createHumanAvatar : MonoBehaviour, IMixedRealityGestureHandler
{
    // Start is called before the first frame update
    private Transform rightHand, leftHand;
    private GameObject rightShoulder, leftShoulder, head, rightHandBox, leftHandBox;
    private static bool left = true, both = true, initializedHands = false;
    private float armLength;
    [SerializeField]
    private MixedRealityInputAction selectAction;

    void Start()
    {
        head = Instantiate(Resources.Load("Head", typeof(GameObject))) as GameObject;
        leftShoulder = head.transform.Find("LeftShoulder").gameObject;
        rightShoulder = head.transform.Find("RightShoulder").gameObject;
        var handJointService = CoreServices.GetInputSystemDataProvider<IMixedRealityHandJointService>();
        if (handJointService != null)
        {
            rightHand = handJointService.RequestJointTransform(TrackedHandJoint.Palm, Microsoft.MixedReality.Toolkit.Utilities.Handedness.Right);
            leftHand = handJointService.RequestJointTransform(TrackedHandJoint.Palm, Microsoft.MixedReality.Toolkit.Utilities.Handedness.Left);
            rightHandBox = Instantiate(Resources.Load("HandBox", typeof(GameObject))) as GameObject;
            rightHandBox.name = "rightHandBox";
            rightHandBox.transform.SetParent(rightHand);
            leftHandBox = Instantiate(Resources.Load("HandBox", typeof(GameObject))) as GameObject;
            leftHandBox.name = "rightHandBox";
            leftHandBox.transform.SetParent(leftHand);
            // ...
        }
        armLength = 0.0f;
    }

    // Update is called once per frame
    void Update()
    {
        var yRotation = Camera.main.transform.rotation.eulerAngles.y;
        head.transform.rotation = Quaternion.Euler(0f, yRotation, 0f);
        if (false && !initializedHands)
        {
            var headPos = Camera.main.transform.TransformPoint(0, 0, 2f);
            head.transform.position = headPos;
            head.transform.Rotate(new Vector3(0, 180, 0));
        }
        else
            head.transform.position = Camera.main.transform.position;
       
        //if(rightShoulder != null && rightHand.transform.position != new Vector3())
        //{
        //    var shouldPos = rightShoulder.transform.position;
        //    shouldPos.z -= 0.5f;
        //    var rightDist = (rightHand.transform.position - shouldPos).magnitude;
            
        //}

    }
    private void OnEnable()
    {
        
    }

    private void OnDisable()
    {
        CoreServices.InputSystem?.UnregisterHandler<IMixedRealityGestureHandler>(this);
    }

    public void OnGestureCompleted(InputEventData eventData)
    {
        if (both)
        {
            both = false;
            return;
        }
        if (left)
        {
            float posX = leftHand.transform.position.x;
            float posY = leftHand.transform.position.y;
            float posZ = 0f;
            leftShoulder.transform.position = new Vector3(posX, posY, head.transform.position.z);
            var localLeft = Camera.main.transform.InverseTransformPoint(leftHand.transform.position);
            armLength = (leftHand.transform.position- leftShoulder.transform.position).magnitude;
            leftShoulder.transform.SetParent(head.transform, true);
            left = false;
            Debug.Log("LEFT SHOULDER SET");
        }
        else
        {
            float posX = rightHand.transform.position.x;
            float posY = rightHand.transform.position.y;
            float posZ = 0f;
            var shoulderConnector = head.transform.Find("ShoulderConnector/ShoulderConnectorMesh");
            rightShoulder.transform.position = new Vector3(posX, posY, leftShoulder.transform.position.z);
            var localRight = Camera.main.transform.InverseTransformPoint(rightHand.transform.position);
            armLength += (rightHand.transform.position - rightShoulder.transform.position).magnitude;
            armLength /= 2;
            rightShoulder.transform.SetParent(head.transform, true);
            left = true;
            both = true;
            var connectorPosy = (posY + leftShoulder.transform.position.y) / 2f;
            var connectorScaleY = (rightShoulder.transform.position - leftShoulder.transform.position).magnitude-0.125f;
            var oldScaleY = Mathf.Abs(posX - leftShoulder.transform.position.x) + 0.045f;
            shoulderConnector.position = new Vector3(shoulderConnector.position.x, connectorPosy, shoulderConnector.position.z);
            shoulderConnector.localScale = new Vector3(shoulderConnector.localScale.x, connectorScaleY/10.0f, shoulderConnector.localScale.z);
            var lclPos = rightShoulder.transform.localPosition;
            rightShoulder.transform.localPosition = new Vector3(lclPos.x + 0.03f, lclPos.y, lclPos.z);
            lclPos = leftShoulder.transform.localPosition;
            leftShoulder.transform.localPosition = new Vector3(lclPos.x - 0.03f, lclPos.y, lclPos.z);
            CoreServices.InputSystem?.UnregisterHandler<IMixedRealityGestureHandler>(this);
            initializedHands = true;
            head.transform.position = new Vector3(head.transform.position.x, head.transform.position.y, head.transform.position.z + armLength - 0.5f);

            leftShoulder.transform.SetParent(shoulderConnector.parent, true);
            rightShoulder.transform.SetParent(shoulderConnector.parent, true);

            head.GetComponent<ArmController>().setHands(rightHandBox, leftHandBox, armLength);
            initializedHands = true;
            Debug.Log("RIGHT SHOULDER SET");
        }
    }

    public void OnGestureStarted(InputEventData eventData) {
    }
    public void OnGestureUpdated(InputEventData eventData) { }
    public void OnGestureCanceled(InputEventData eventData) { }
    public void InitializeShoulders()
    {
        left = true;
        both = true;
        Debug.Log("SHOULD INITIALIZATION ACTIVATED");
        CoreServices.InputSystem?.RegisterHandler<IMixedRealityGestureHandler>(this);
    }
}
