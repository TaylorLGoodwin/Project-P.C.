﻿using UnityEngine;
using UnityEngine.UI;

public class ContentPanel : MonoBehaviour {

	public void DeactivateInventory () {
		foreach (Transform child in transform) {
			child.GetComponent<Button>().interactable = false;
		}
	}
	
	public void ActivateInventory () {
		foreach (Transform child in transform) {
			child.GetComponent<Button>().interactable = true;
		}
	}
	
	public void DeleteInventory () {
		foreach (Transform child in transform) {
			if (child.gameObject.name == "Place Holder" || child.gameObject.tag == "Equipped Slot" ) {
			} else {
				Destroy (child.gameObject);
			}
		}
	}
	
	public void InitiateWeaponEvolutionMenuSlots () {
		EquipmentDatabase equipmentDatabase = GameObject.FindGameObjectWithTag("Equipment Database").GetComponent<EquipmentDatabase>();
		foreach (Transform child in transform) {
			if (child.gameObject.name == "Place Holder") {
			} else {
				Text weaponIDText = child.GetChild(1).GetComponent<Text>();
				int weaponID = int.Parse(weaponIDText.text);
				
				Text weaponNameText = child.GetChild(0).GetComponent<Text>();
				weaponNameText.text = equipmentDatabase.equipment[weaponID].equipmentName;
				
				Text weaponLevel = child.GetChild(3).GetComponent<Text>();
				//weaponLevel.text = equipmentDatabase.equipment[weaponID].quantity.ToString();
			}
		}
	}
	
	public void UpdateWeaponEvolutionMenu (int weaponID) {
		EquipmentDatabase equipmentDatabase = GameObject.FindGameObjectWithTag("Equipment Database").GetComponent<EquipmentDatabase>();
		foreach (Transform child in transform) {
			Text weaponIDText = child.GetChild(1).GetComponent<Text>();
			int weaponIDInt = int.Parse(weaponIDText.text);
			if (weaponIDInt == weaponID) {
				weaponIDText.text = (weaponID + 1).ToString();
				
				Text weaponNameText = child.GetChild(0).GetComponent<Text>();
				weaponNameText.text = equipmentDatabase.equipment[weaponID + 1].equipmentName;
				
				Text weaponLevel = child.GetChild(3).GetComponent<Text>();
				//weaponLevel.text = equipmentDatabase.equipment[weaponID + 1].quantity.ToString();
			}
		}
	}
}
