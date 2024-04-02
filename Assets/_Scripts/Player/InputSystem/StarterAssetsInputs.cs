using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
using Unity.Netcode;
#endif

namespace StarterAssets
{
	public class StarterAssetsInputs : NetworkBehaviour
	{
		[Header("Character Input Values")]
		public Vector2 move;
		public Vector2 look;
		public bool jump;
		public bool sprint;

		[Header("Movement Settings")]
		public bool analogMovement;

		[Header("Mouse Cursor Settings")]
		public bool cursorLocked = false;
		public bool cursorInputForLook = true;

		private PlayerInputClass playerinputClass;

#if ENABLE_INPUT_SYSTEM
		public override void OnNetworkSpawn()
		{
			if (!IsOwner) return;

			playerinputClass = new PlayerInputClass();
			playerinputClass.Player.Enable();
			playerinputClass.Player.Jump.performed += OnJump;
			playerinputClass.Player.Sprint.performed += OnSprint;

			base.OnNetworkSpawn();
		}


		private void Update()
		{
			if (!IsOwner) return;
			MoveInput(playerinputClass.Player.Move.ReadValue<Vector2>());
			LookInput(playerinputClass.Player.Look.ReadValue<Vector2>());
		}


		public void OnJump(InputAction.CallbackContext value)
		{
			JumpInput(value.performed);
		}

		public void OnSprint(InputAction.CallbackContext value)
		{
			SprintInput(value.performed);
		}
#endif

		public void MoveInput(Vector2 newMoveDirection)
		{
			move = newMoveDirection;
		}

		public void LookInput(Vector2 newLookDirection)
		{
			look = newLookDirection;
		}

		public void JumpInput(bool newJumpState)
		{
			jump = newJumpState;
		}

		public void SprintInput(bool newSprintState)
		{
			sprint = newSprintState;
		}


		public void ChangeCursorState()
		{
			cursorLocked= !cursorLocked;
			Cursor.lockState = cursorLocked ? CursorLockMode.Locked : CursorLockMode.None;
		}
	}
	
}