using System;
using MoreMountains.Feedbacks;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    [SerializeField] private CharacterController m_characterController;
    [SerializeField] private Animator m_animator;
    
    [SerializeField] private float m_speed = 5f;
    
    [SerializeField] private float m_lookXSensitivity = 2f;
    [SerializeField] private float m_lookYSensitivity = 2f;
    
    [SerializeField] private float m_clampMinX = -80f;
    [SerializeField] private float m_clampMaxX = 80f;
    [SerializeField] private MMFeedbacks m_footstepFeedback;
    
    private float m_mouseX;
    private Vector3 m_moveDirection;

    private void OnEnable()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        if(m_characterController == null)
        {
            Debug.LogError("Camera or CharController is not assigned.");
            return;
        }

        // Get input for movement
        Vector2 input = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        
        // Calculate movement direction based on camera orientation
        Vector3 cameraForward = transform.forward;
        cameraForward.y = 0; // Ignore vertical component
        Vector3 cameraRight = transform.right;
        cameraRight.y = 0; // Ignore vertical component
        m_moveDirection = (cameraForward * input.y + cameraRight * input.x).normalized;

        float animTime = m_animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
        if (m_moveDirection != Vector3.zero)
        {
            animTime = Mathf.Clamp(animTime + Time.deltaTime * 1.8f, 0f, 1f);
            if (animTime > 0.99f)
            {
                m_footstepFeedback.PlayFeedbacks();
                animTime = 0f; // Reset animation time if it exceeds 1
            }
        }
        else
        {
            animTime = 0;
        }
        
        m_animator.Play("PlayerWalk", 0, animTime);
        
        // Look around w/ mouse
        m_mouseX = Input.GetAxis("Mouse X") * m_lookXSensitivity;
        
        if (m_mouseX != 0)
        {
            // Rotate the player based on mouse input
            Vector3 rotation = new Vector3(0, m_mouseX, 0);
            transform.Rotate(rotation);
        }
    }

    private void FixedUpdate()
    {
        var collisionFlags = m_characterController.Move(
            new Vector3(m_moveDirection.x, Physics.gravity.y, m_moveDirection.z) * m_speed * Time.deltaTime);
    }
    
    private void Footstep()
    {
        m_footstepFeedback.PlayFeedbacks();
    }
}
