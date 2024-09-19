using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public List<ArtefactItem> items = new List<ArtefactItem>();

    // Add an item to the inventory
    public void AddItem(ArtefactItem newItem)
    {
        // Check if the item is already in the inventory
        ArtefactItem existingItem = items.Find(item => item.itemName == newItem.itemName);

        if (existingItem != null)
        {
            // Increase the quantity if it already exists
            existingItem.quantity += newItem.quantity;
        }
        else
        {
            // Add new item to the list
            items.Add(newItem);
        }
    }

    // Remove an item from the inventory
    public void RemoveItem(ArtefactItem itemToRemove)
    {
        ArtefactItem existingItem = items.Find(item => item.itemName == itemToRemove.itemName);

        if (existingItem != null)
        {
            if (existingItem.quantity > 1)
            {
                existingItem.quantity -= 1; // Decrease quantity
            }
            else
            {
                items.Remove(existingItem); // Remove the item completely
            }
        }
    }
}

[System.Serializable]
public class ArtefactItem
{
    public string itemName;   // Name of the item
    public int quantity;      // Amount of this item in the inventory
}