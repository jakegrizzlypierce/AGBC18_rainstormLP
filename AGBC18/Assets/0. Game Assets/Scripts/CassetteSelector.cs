﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class CassetteSelector : MonoBehaviour {

	enum SELECTOR_STATE {
		INVALID = -1,
		IDLE,
		SHIFT
	}
	SELECTOR_STATE currState = SELECTOR_STATE.INVALID;
	public string DEBUG_STATE = "";

	Ease shiftEase = Ease.OutBack;
	Ease hoverEase = Ease.OutBack;

	void SetState(SELECTOR_STATE state) {
		currState = state;
		DEBUG_STATE = state.ToString();
	}

	public GameObject
	inactiveSlot,
	rightQueue,
	leftQueue,
	rightNext,
	leftNext,
	activeSlot;

	public List<GameObject> cassettes;
	int cassetteStock = -1;


    [FMODUnity.EventRef]
    public string cartridgeScrollLeft;
    [FMODUnity.EventRef]
    public string cartridgeScrollRight;

	void Start () {
		if(inactiveSlot == null)
			Destroy(this);
		
		for (int i = 0; i < inactiveSlot.transform.childCount; ++i) {
			cassettes.Add(inactiveSlot.transform.GetChild(i).gameObject);
		}

		if (cassettes.Count >= 6) {
			cassettes.Sort(delegate(GameObject i1, GameObject i2) 
				{ 
					return i1.name.CompareTo(i2.name); 
				}
			);
		}
		else
			Destroy(this);

		cassetteStock = cassettes.Count - 5;
		SetState(SELECTOR_STATE.IDLE);

		SetStartPositions();
	}
	
	void Update()
	{

	}

	void SetStartPositions() {
		for(int i = 0; i < cassettes.Count; ++i) {
			resetPosition(cassettes[i], inactiveSlot);
		}

		resetPosition(cassettes[0], activeSlot);
		resetPosition(cassettes[1], rightNext);
		resetPosition(cassettes[2], rightQueue);
		resetPosition(cassettes[cassettes.Count - 1], leftNext);
		resetPosition(cassettes[cassettes.Count - 2], leftQueue);
	}

	void resetPosition(GameObject cassette, GameObject slot) {
		cassette.transform.SetParent(slot.transform);
		cassette.GetComponent<RectTransform>().localPosition = Vector3.zero;
	}

	public void ShiftRight() {
		if(currState == SELECTOR_STATE.IDLE) {
			currState = SELECTOR_STATE.SHIFT;

			// CALCULATE NEW CASSETTES
			int[] _active = new int[5];
			int _prevActive = int.Parse(activeSlot.transform.GetChild(0).name);
			//Debug.Log("NEW ACTIVE");
			int _newActive = calculateWrap(_prevActive, -1);

			//Debug.Log("NEW LEFT");
			int _newLeft = calculateWrap(_newActive, - 1);
			//Debug.Log("NEW LEFTQ");
			int _leftQ = calculateWrap(_newActive - 1, - 1);

			int _newRight = _prevActive;
			//Debug.Log("NEW RIGHTQ");
			int _rightQ = calculateWrap(_prevActive, 1);

			_active[0] = _leftQ;
			_active[1] = _newLeft;
			_active[2] = _newActive;
			_active[3] = _newRight;
			_active[4] = _rightQ;

			//Debug.Log(_leftQ + "  >>>  " + _newLeft + "  >>>  " + _newActive + "  >>>  " + _newRight + "  >>>  " + _rightQ);

			// CALCULATE INACTIVE CASSETTES
			int[] _inactive = new int[cassetteStock];

			int temp = _rightQ;
			for (int i = 0 ; i < cassetteStock; ++i) {
				temp = ++temp;

				if (temp > cassettes.Count - 1) {
					temp = 0;
				}

				if(temp < 0) {
					temp = cassettes.Count - 1;
				}

				_inactive[i] = temp;

				//print(_inactive[i].ToString());
			}

			FMODUnity.RuntimeManager.PlayOneShot(cartridgeScrollRight);

			StartCoroutine(Shift(_inactive, _active));
		}
	}

	public void ShiftLeft() {
		if(currState ==SELECTOR_STATE.IDLE) {
			currState = SELECTOR_STATE.SHIFT;

			// CALCULATE NEW CASSETTES
			int[] _active = new int[5];
			int _prevActive = int.Parse(activeSlot.transform.GetChild(0).name);
			//Debug.Log("NEW ACTIVE");
			int _newActive = calculateWrap(_prevActive, 1);

			//Debug.Log("NEW LEFT");
			int _newRight = calculateWrap(_newActive, 1);
			//Debug.Log("NEW LEFTQ");
			int _rightQ = calculateWrap(_newActive + 1, 1);

			int _newLeft = _prevActive;
			//Debug.Log("NEW RIGHTQ");
			int _leftQ = calculateWrap(_prevActive, -1);

			_active[0] = _leftQ;
			_active[1] = _newLeft;
			_active[2] = _newActive;
			_active[3] = _newRight;
			_active[4] = _rightQ;

			//Debug.Log(_leftQ + "  <<<  " + _newLeft + "  <<<  " + _newActive + "  <<<  " + _newRight + "  <<<  " + _rightQ);


			// 8  |  0  |  1  |  2  |  3

			// CALCULATE INACTIVE CASSETTES
			int[] _inactive = new int[cassetteStock];

			int temp = _rightQ;
			for (int i = 0 ; i < cassetteStock; ++i) {
				temp = ++temp;

				if (temp > cassettes.Count - 1) {
					temp = 0;
				}

				if(temp < 0) {
					temp = cassettes.Count - 1;
				}

				_inactive[i] = temp;

				//print(_inactive[i].ToString());
			}

			FMODUnity.RuntimeManager.PlayOneShot(cartridgeScrollLeft);

			StartCoroutine(Shift(_inactive, _active));
		}
	}

	public void Hover(int side) {
		resetHovers();
		switch(side) {
			case -1:
			//Debug.Log("LEFT");
			leftNext.GetComponent<RectTransform>().DOLocalMove(new Vector3(leftNext.transform.localPosition.x, 47, 0), .5f).SetEase(hoverEase);		
			leftNext.transform.GetChild(0).GetChild(0).GetComponent<Text>().DOFade(.75f, .25f);
			break;

			case 0:
			activeSlot.GetComponent<RectTransform>().DOLocalMove(new Vector3(activeSlot.transform.localPosition.x, 53, 0), .5f).SetEase(hoverEase);
			break;

			case 1:
			//Debug.Log("RIGHT");
			rightNext.GetComponent<RectTransform>().DOLocalMove(new Vector3(rightNext.transform.localPosition.x, 47, 0), .5f).SetEase(hoverEase);
			rightNext.transform.GetChild(0).GetChild(0).GetComponent<Text>().DOFade(.75f, .25f);
			break;

			default:
			//Debug.Log("Invalid Selection");
			break;
		}
	}

	public void resetHovers() {
		leftNext.GetComponent<RectTransform>().DOLocalMove(new Vector3(leftNext.transform.localPosition.x, 40, 0), .5f);
		leftNext.transform.GetChild(0).GetChild(0).GetComponent<Text>().DOFade(0f, .25f);

		rightNext.GetComponent<RectTransform>().DOLocalMove(new Vector3(rightNext.transform.localPosition.x, 40, 0), .5f);
		rightNext.transform.GetChild(0).GetChild(0).GetComponent<Text>().DOFade(0f, .25f);

		activeSlot.GetComponent<RectTransform>().DOLocalMove(new Vector3(activeSlot.transform.localPosition.x, 48, 0), .5f);
	}

	public void centerSelected() {
		if(activeSlot.GetComponentInChildren<CartridgeData>().IsKnown())
			StartCoroutine(clicked());
	}

	IEnumerator clicked() {
		activeSlot.GetComponentInChildren<RectTransform>().DOScale(1.05f, .25f).SetEase(Ease.InBack);
		
		yield return new WaitForSeconds(.25f);

		activeSlot.GetComponentInChildren<RectTransform>().DOScale(1f, .1f);		
	}

	int calculateWrap(int num, int dir) {

		int calc = num + dir;
		// Debug.Log("calculating wrap... calculation is " + calc);

		if(calc < -1) {
			return (cassettes.Count - 2);		
		}

		if (calc < 0) {
			//Debug.Log("Resetting to " + (cassettes.Count - 1));
			return (cassettes.Count - 1);
		}

		if(calc > cassettes.Count) {
			//Debug.Log("Resetting to 1");
			return 1;
		}

		if (calc == cassettes.Count) {
			//Debug.Log("Resetting to 0");
			return 0;
		}

		return calc;
	}

	IEnumerator Shift(int[] inactive, int[] order) {

		resetHovers();

		for(int i = 0; i < inactive.Length; ++i) {
			resetPosition(cassettes[inactive[i]], inactiveSlot);
		}

		cassettes[order[0]].transform.SetParent(leftQueue.transform);
		cassettes[order[0]].GetComponent<RectTransform>().DOLocalMove(Vector3.zero, 1f).SetEase(shiftEase);
		
		cassettes[order[1]].transform.SetParent(leftNext.transform);
		cassettes[order[1]].GetComponent<RectTransform>().DOLocalMove(Vector3.zero, 1f).SetEase(shiftEase);
		cassettes[order[1]].GetComponent<RawImage>().DOFade(.5f, .5f);

		cassettes[order[2]].transform.SetParent(activeSlot.transform);
		cassettes[order[2]].GetComponent<RectTransform>().DOLocalMove(Vector3.zero, 1f).SetEase(shiftEase);
		cassettes[order[2]].GetComponent<RawImage>().DOFade(1f, .5f);
		cassettes[order[2]].GetComponentInChildren<Text>().DOFade(1f, .25f);

		cassettes[order[3]].transform.SetParent(rightNext.transform);
		cassettes[order[3]].GetComponent<RectTransform>().DOLocalMove(Vector3.zero, 1f).SetEase(shiftEase);
		cassettes[order[3]].GetComponent<RawImage>().DOFade(.5f, .5f);

		cassettes[order[4]].transform.SetParent(rightQueue.transform);
		cassettes[order[4]].GetComponent<RectTransform>().DOLocalMove(Vector3.zero, 1f).SetEase(shiftEase);

		yield return new WaitForSeconds(1f);

		SetState(SELECTOR_STATE.IDLE);
	}

    public CartridgeData ReturnNextUnknownCassette() {
        CartridgeData data;
        for (int i = 0; i <= cassettes.Count; i++) {
            data = cassettes[i].GetComponent<CartridgeData>();
            if (!data.IsKnown()) {
                return data;
            }
        }
        return null;
    }
}
