using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace PSXShaderKit
{
    public class PSXExample_CameraMovement : MonoBehaviour
    {
        public float speed = 0.5f;

        private PlayerInput playerInput;
        [SerializeField] private InputAction moveAction;
        [SerializeField] private InputAction lookAction;
        // Start is called before the first frame update
        void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;

            playerInput = GetComponent<PlayerInput>();
            moveAction  = playerInput.actions["Move"];
            lookAction  = playerInput.actions["Look"];
        }

        // Update is called once per frame
        void Update()
        {
            Vector2 moveIn = moveAction.ReadValue<Vector2>();
            transform.position += new Vector3(transform.forward.x, 0, transform.forward.z) * moveIn[1] * speed * Time.deltaTime;
            transform.position += new Vector3(transform.right.x, 0, transform.right.z) * moveIn[0] * speed * Time.deltaTime;

            Vector2 mouseDelta = lookAction.ReadValue<Vector2>() * 0.1f;
            transform.RotateAround(transform.position, transform.right, -mouseDelta.y);
            transform.RotateAround(transform.position, Vector3.up, mouseDelta.x);
        }
    }
}
