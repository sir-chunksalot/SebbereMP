using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagazineUI : MonoBehaviour
{ 
    [SerializeField] GameObject bulletUI;
    public void AddBullet()
    {
        GameObject poop = Instantiate(bulletUI, transform.position, Quaternion.identity, this.transform);
        Debug.Log("big fat cumsocK;");
        Debug.Log(poop);
        Debug.Log(bulletUI.transform.position);
    }

    public void RemoveBullet()
    {
        Destroy(transform.GetChild(0).gameObject);
    }
}
