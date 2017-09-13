using UnityEngine;
using System.Collections;
[System.Serializable]
public class Item {
	public string itemname;
	public int itemID;
	public int itemLevel;
	public Texture2D itemIcon;
	public string itemDesc;
	public int itemDamage;
	public int itemdefense;
	public int itemSpeed;
	public itemType ItemType;
	public int ShopPrice;
	public enum itemType{
		Weapon,Ranged,Rile,Offhand,Helmet,Chest,Leggings,Gloves,Boots,Earing,Necklace,Consumable,Quest,Shop,Bulltets,Cannonballs,Grenades
		
	};


	public Item(string name,int id,int level,string desc,int damage,int defense,int speed,itemType type,int priceG){
		itemname = name;
		itemLevel = level;
		itemID = id;
		itemIcon = Resources.Load<Texture2D> ("Item Icons/" + name);
		itemDesc = desc;
		itemDamage = damage;
		itemdefense = defense;
		itemSpeed = speed;
		ItemType = type;
	
		ShopPrice = priceG;
	}

	//this si for our empty inv slots
	public Item(){
		itemID = -1;
		}
	
}
