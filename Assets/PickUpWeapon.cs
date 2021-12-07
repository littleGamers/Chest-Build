using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpWeapon : MonoBehaviour
{
    [SerializeField] KeyCode pickUpKey = KeyCode.Space;
    [SerializeField] string weaponTag = "Weapon";
    [SerializeField] Vector3 weaponPositionOnPlayer;
    [SerializeField] Vector3 weaponRotation;

    private bool isWeaponAround = false;
    GameObject weapon = null;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Entered player ontrigger");
        if (other.gameObject.tag == weaponTag)
        {
            Debug.Log("Entered tag");

            isWeaponAround = true;
            weapon = other.gameObject;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Debug.Log("Exited player ontrigger");
        if (other.gameObject.tag == weaponTag)
        {
            Debug.Log("Exited tag");

            isWeaponAround = false;
            weapon = null;
        }
    }

    void Update()
    {

        if (isWeaponAround && Input.GetKeyDown(pickUpKey))
        {
            Debug.Log("Entered pick up");

            Vector3 realPostion = new Vector3(transform.position.x + weaponPositionOnPlayer.x,
                                                transform.position.y + weaponPositionOnPlayer.y,
                                                transform.position.z + weaponPositionOnPlayer.z);

            weapon.transform.SetPositionAndRotation(realPostion, Quaternion.Euler(weaponRotation));

            weapon.GetComponent<Collider>().enabled = false;
            isWeaponAround = false;

            weapon.transform.parent = transform;
        }
    }
}
