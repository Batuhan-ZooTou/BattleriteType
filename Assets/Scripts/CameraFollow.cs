using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraFollow : MonoBehaviour
{
	[Header("Camera")]
	public Camera mainCamera;
	public Transform player;
	public Transform rootCam;
	public LayerMask floorMask;
	public bool canRotate = true;
	public float threshHold=10;
	public float rotationTime;
	public bool camLocked=false;
	[HideInInspector]public Vector3 skillPoint;
	public float currentSkillRange =12;
	private bool movingMouse;
	public CinemachineVirtualCamera virtualCam;
	private CinemachineComponentBase componentBase;
	public Vector3 dir;
	private void Start()
    {
		//rotationTime = RotationSmoothTime;
		componentBase = virtualCam.GetCinemachineComponent(CinemachineCore.Stage.Body);
	}
    void FixedUpdate()
	{
		Ray camRay = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit floorHit;
		if (Physics.Raycast(camRay, out floorHit, 100f, floorMask))
		{
			Vector3 playerToMouse = floorHit.point - player.position;
			Vector3 playerToMouse1 = floorHit.point - player.position;
			playerToMouse.y = 0f;
			playerToMouse1.y = 0f;
			dir = playerToMouse.normalized;
			//Vector3 maxDistance = new Vector3(Mathf.Clamp(floorHit.point.x, player.position.x - threshHoldx, player.position.x + threshHoldx), 0, Mathf.Clamp(floorHit.point.z, player.position.z - threshHoldz, player.position.z + threshHoldz));
			playerToMouse = new Vector3(Mathf.Clamp(playerToMouse.x, - (threshHold * 2f), (threshHold * 2f)), 0, Mathf.Clamp(playerToMouse.z,- threshHold,threshHold));
			Vector3 middlePoint = playerToMouse / 2;
			//cam point
            if (!camLocked )
            {
				rootCam.position = player.position + middlePoint;
			}
            else if (camLocked)
            {
				rootCam.position = player.position;
			}
            if (canRotate)
            {
				Quaternion newRotation = Quaternion.LookRotation(playerToMouse);
				player.rotation = Quaternion.Lerp(player.rotation, newRotation, rotationTime);
			}
			//skillpoint
			float distance = Vector3.Distance(player.position, floorHit.point);
			float x = Mathf.Abs(currentSkillRange * playerToMouse1.x / distance);
			float z = Mathf.Abs(currentSkillRange * playerToMouse1.z / distance);
			Vector3 clampedDistance = new Vector3(Mathf.Clamp(floorHit.point.x, player.position.x - x, player.position.x + x), 0.01f, Mathf.Clamp(floorHit.point.z, player.position.z - z, player.position.z + z));
			skillPoint = clampedDistance;
		}
	}
	public void ZoomCam(float value)
    {
		if (componentBase is CinemachineFramingTransposer)
		{
			var framingTransposer = componentBase as CinemachineFramingTransposer;
			threshHold = (framingTransposer.m_CameraDistance*5)/6 ;
			// Now we can change all its values easily.
			if (value>0 && framingTransposer.m_CameraDistance <= 12)
			{
				framingTransposer.m_CameraDistance += value*Time.deltaTime;
			}
            else if (value < 0 && framingTransposer.m_CameraDistance >= 8)
            {
				framingTransposer.m_CameraDistance += value * Time.deltaTime;
			}
			framingTransposer.m_CameraDistance = Mathf.Clamp(framingTransposer.m_CameraDistance, 8, 12);
		}
		
    }
}
