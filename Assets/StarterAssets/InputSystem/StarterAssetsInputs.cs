using UnityEngine;
#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
using UnityEngine.InputSystem;
#endif

namespace StarterAssets
{
	public class StarterAssetsInputs : MonoBehaviour
	{
		public ThirdPersonController player;
		[Header("Character Input Values")]
		public bool onAction = false;
		public bool delayAfterCancel=false;
		public bool holdinShift=false;
		public bool actionCancel=false;
		public Vector2 move;
		public bool canMove=true;
		public float mouseScrollY;
		public bool lockCam;
		[Header("Movement Settings")]
		public bool analogMovement;
		private bool switchLock=false;

		[Header("Mouse Cursor Settings")]
		public bool cursorLocked = true;
		public bool cursorInputForLook = true;

#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED

        public void OnMove(InputValue value)
		{
			MoveInput(value.Get<Vector2>());
		}
		public void OnSkillRMB()
        {
            if (!player.skillRMB.onAction&&!delayAfterCancel&& player.playerControllStates != PlayerDebuffs.Stunned)
            {
				if (!player.skillRMB.onCooldown)
				{
					onAction = true;
					player.skillRMB.onAction = true;
				}
			}
		}
		public void OnSkillLMB()
        {
			if (!player.skillLMB.onAction && !delayAfterCancel && player.playerControllStates != PlayerDebuffs.Stunned)
			{
                if (!player.skillLMB.onCooldown)
                {
					onAction = true;
					player.skillLMB.onAction = true;
				}
			}
		}
		public void OnAdjustCam(InputValue value)
        {
			ZoomInput(value.Get<float>());
		}
		public void OnLockCam(InputValue value)
        {
			LockCam(value.isPressed);
		}
		public void	OnSkillQ()
        {
			//player.abilitySlots[0].ChangeAbilityState(AbilityState.Casting);
            if (holdinShift && player.skillShiftQ.energyRequirment<=player.playerSO.currentEnergy)
            {
				if (!player.skillShiftQ.onAction && !delayAfterCancel && player.playerControllStates != PlayerDebuffs.Stunned)
				{
					if (!player.skillShiftQ.onCooldown)
					{
						onAction = true;
						player.skillShiftQ.gameObject.SetActive(true);
						player.skillShiftQ.onAction = true;
					}
				}
			}
            else
            {
				if (!player.skillQ.onAction && !delayAfterCancel && player.playerControllStates != PlayerDebuffs.Stunned)
				{
					if (!player.skillQ.onCooldown)
					{
						onAction = true;
						player.skillQ.gameObject.SetActive(true);
						player.skillQ.onAction = true;
					}
				}
			}
			
		}
		public void OnSkillSpace()
		{
            if (holdinShift && player.skillShiftSpace.energyRequirment <= player.playerSO.currentEnergy)
            {
				if (!player.skillShiftSpace.onAction && !delayAfterCancel && player.playerControllStates != PlayerDebuffs.Stunned)
				{
					if (!player.skillShiftSpace.onCooldown)
					{
						onAction = true;
						player.skillShiftSpace.onAction = true;
					}
				}
			}
            else
            {
				if (!player.skillSpace.onAction && !delayAfterCancel && player.playerControllStates != PlayerDebuffs.Stunned)
				{
					if (!player.skillSpace.onCooldown)
					{
						onAction = true;
						player.skillSpace.onAction = true;
					}
				}
			}
			
		}
		public void OnSkillF()
		{
			if (!player.skillF.onAction && !delayAfterCancel && player.playerControllStates != PlayerDebuffs.Stunned && player.skillF.energyRequirment <= player.playerSO.currentEnergy)
			{
				if (!player.skillF.onCooldown)
				{
					onAction = true;
					player.skillF.onAction = true;
				}
			}
		}
		public void OnSkillR()
		{
			if (!player.skillR.onAction && !delayAfterCancel && player.playerControllStates != PlayerDebuffs.Stunned && player.skillR.energyRequirment <= player.playerSO.currentEnergy)
			{
				if (!player.skillSpace.onCooldown)
				{
					onAction = true;
					player.skillR.onAction = true;
				}
			}
		}
		public void OnSkillE()
		{
			if (!player.skillE.onAction && !delayAfterCancel && player.playerControllStates != PlayerDebuffs.Stunned)
			{
				if (!player.skillE.onCooldown)
				{
					onAction = true;
					player.skillE.onAction = true;
				}
			}
		}
		public void OnHoldShift(InputValue value)
        {
			holdinShift = value.Get<float>() != 0 ? true : false;
		}
		public void OnActionCancel()
        {
			if (onAction)
			{
				actionCancel = true;
			}
		}
#endif
		public void MoveInput(Vector2 newMoveDirection)
		{
			move = newMoveDirection;
		} 
		public void ZoomInput(float newZoom)
        {
			mouseScrollY = newZoom;
		}
		public void LockCam(bool newLockState)
        {
            if (switchLock)
            {
				lockCam = !newLockState;
				switchLock = false;
			}
            else
            {
				lockCam = newLockState;
				switchLock = true;
			}
		}

	}
	
}