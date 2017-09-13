using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class ItemDataBase : MonoBehaviour {
	public List<Item> items = new List<Item>();




	// Use this for initialization
	void Start () {
		items.Add(new Item("Standard Sword",0,1,"A Standard Sword",1,0,1,Item.itemType.Weapon,10));
		items.Add(new Item("Standard Sword Epic",1,3,"A Epic Standard Sword",20,0,1,Item.itemType.Weapon,1000));
		items.Add(new Item("Standard Pistol",2,1,"A pistol",2,0,1,Item.itemType.Ranged,15));
		items.Add(new Item("Standard Helmet",3,1,"A Helmet",0,1,0,Item.itemType.Helmet,20));
		items.Add(new Item("Standard Chest",4,1,"A ChestPiece",0,2,0,Item.itemType.Chest,20));
		items.Add(new Item("Standard Leggings",5,1,"Pants",0,1,0,Item.itemType.Leggings,20));
		items.Add(new Item("Standard Gloves",6,1,"Gloves",0,1,0,Item.itemType.Gloves,20));
		items.Add(new Item("Standard Boots",7,1,"Boots",0,1,0,Item.itemType.Boots,20));
		items.Add(new Item("Standard Earing",8,1,"Piercing...Eww",0,1,0,Item.itemType.Earing,15));
		items.Add(new Item("Standard Necklace",9,1,"Fabulous",0,1,0,Item.itemType.Necklace,15));
		items.Add(new Item("Phill's Sword",10,1,"A very worn sword.",0,0,0,Item.itemType.Quest,0));
		items.Add (new Item ("Minor Healing Tonic", 11, 1, "Restores 100 HP", 0, 0, 0, Item.itemType.Consumable,10));
		items.Add (new Item ("Minor Strength Tonic", 12, 1, "Boosts Your Strength by 2 for 1 minute", 0, 0, 0, Item.itemType.Consumable,10));
		items.Add(new Item("Luxary Sword",13,2,"A Sword with special crests",10,0,1,Item.itemType.Weapon,50));
		items.Add(new Item("Casket",14,1,"An old casket.",0,0,0,Item.itemType.Quest,0));
	}
	

}
