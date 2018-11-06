﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class interactable : MonoBehaviour {

    public PopupManager manager;
    public CartridgeData cassetteFound;
    Color cassetteColor;

    public string preDialog;
    public string postDialog; 
    public bool interactedWith = false;


    void Start() {
        if(cassetteFound == null) {
            cassetteColor = new Color(0, 0, 0, 0);
        }
        else {
            cassetteColor = new Color(cassetteFound.dataHolder.color.r, cassetteFound.dataHolder.color.g, cassetteFound.dataHolder.color.b, 1f);
        }
    }

     void OnMouseDown()
     {
         if(manager.GetIfAvailable()) {
            if(!interactedWith) {
                manager.Pop(preDialog, cassetteColor, cassetteFound);
                interactedWith = true;
            }
            else {
                manager.Pop(postDialog, new Color(0, 0, 0, 0));               
            }
         }
     }
}
