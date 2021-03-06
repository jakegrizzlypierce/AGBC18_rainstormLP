﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class MenuManager : MonoBehaviour {

	public GameObject curtain, menuButton, closeButton, settingsView, creditsView, settingsTab, creditsTab;

	private GameObject LAST_PRESSED, NOW_AVAILABLE;

	bool isCurrentViewSettings = true;

	void Start() {
		GetComponent<RectTransform>().DOAnchorPos(new Vector2(0, 960), 0f, false);
	}

	public void Controller(bool _isOpening) {
		if(_isOpening) {
			LAST_PRESSED = menuButton;
			NOW_AVAILABLE = closeButton;
		}
		else {
			LAST_PRESSED = closeButton;
			NOW_AVAILABLE = menuButton;			
		}

		transform.parent.GetComponent<UIModes>().isMenuOpen = _isOpening;
		StartCoroutine(controllerSequence(_isOpening));
	}

		/* STEPS FOR MENU /*
		
		1. Turn off [LAST PRESSED] button interactivity
		2. Fade out [LAST PRESSED] button canvas group
		3. Transition in curtain + menu content
		4. Fade in [NOW AVAILABLE] button canvas group
		5. Turn on [NOW AVAILABLE button canvas group]

		*/


	public void onTabSelect(bool isSettings) {
		if(isSettings != isCurrentViewSettings) {
			isCurrentViewSettings = isSettings;
			transitionViews(isCurrentViewSettings);
		}
	}

	public void onHover(CanvasGroup canvasGroup) {
		if(!isItSelected(canvasGroup.name)) {		
			canvasGroup.DOFade(.75f, .25f);	
		}
	}

	public void onExit(CanvasGroup canvasGroup) {

		if(!isItSelected(canvasGroup.name)) {		
			canvasGroup.DOFade(.25f, .25f);	
		}
	}

	bool isItSelected(string _name) {
		if(isCurrentViewSettings && _name == "Settings Tab") {			
			return true;
		}
		else if(!isCurrentViewSettings && _name =="Credits Tab") {
			return true;
		}

		return false;
	}


	void transitionViews(bool isSettings) {
		creditsView.SetActive(!isSettings);
		settingsView.SetActive(isSettings);

		if(isSettings) {
			creditsTab.GetComponent<CanvasGroup>().DOFade(.25f, .25f);
			settingsTab.GetComponent<CanvasGroup>().DOFade(1f, .25f);
		}
		else {
			settingsTab.GetComponent<CanvasGroup>().DOFade(.25f, .25f);
			creditsTab.GetComponent<CanvasGroup>().DOFade(1f, .25f);			
		}
	}


	IEnumerator controllerSequence(bool _isOpening) {
		float _isOpeningVal = (float)(_isOpening ? 1 : 0);

	// PHASE 1: Turn off [LAST PRESSED] button interactivity
		LAST_PRESSED.GetComponent<CanvasGroup>().interactable = false;
		LAST_PRESSED.GetComponent<CanvasGroup>().blocksRaycasts = false;

	// PHASE 2: Fade out [LAST PRESSED] button canvas group
		LAST_PRESSED.GetComponent<CanvasGroup>().DOFade(0, .5f);

	// PHASE 3: Transition in curtain + menu content
		if(_isOpening) {
			GetComponent<RectTransform>().DOAnchorPos(new Vector2(0, 0), 1.5f, false);
		}
		else {
			GetComponent<RectTransform>().DOAnchorPos(new Vector2(0, 960), 1.5f, false);		
		}

		yield return new WaitForSeconds(1f);	

	// PHASE 4: Fade in [NOW AVAILABLE] button canvas group
		NOW_AVAILABLE.GetComponent<CanvasGroup>().DOFade(.5f, .25f);
		yield return new WaitForSeconds(.25f);

	// PHASE 5: Turn on [NOW AVAILABLE button canvas group]
		NOW_AVAILABLE.GetComponent<CanvasGroup>().interactable = true;
		NOW_AVAILABLE.GetComponent<CanvasGroup>().blocksRaycasts = true;
	}
}
