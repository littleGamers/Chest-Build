using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * A simple script that is used on an animated chest and let the player open/close the chest.
 */

public class TriggerChest : MonoBehaviour
{
    [Tooltip("The key to press near the chest to open or close it.")]
    [SerializeField] private KeyCode keyToOpenChest = KeyCode.E;

    [Tooltip("Set the tag for the player that is allowed to open/close chest.")]
    [SerializeField] private string triggeringTag;

    private Animator animator;

    // A bool to check if player is near the chest.
    private bool isPlayerNear = false;

    private void Start()
    {
        animator = transform.GetChild(0).gameObject.GetComponent<Animator>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == triggeringTag)
            isPlayerNear = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == triggeringTag)
            isPlayerNear = false;
    }
    private void Update()
    {
        // If player is near and pressed the right key:
        // Toggle open/close of the chest
        if (isPlayerNear && Input.GetKeyDown(keyToOpenChest))
        {
            bool oldState = animator.GetBool("isOpen");
            animator.SetBool("isOpen", !oldState);
        }
    }
}
