using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/Items/New Item", fileName = "New Item")]
public class Item: ScriptableObject
{
    [SerializeField] private string itemName;
    [SerializeField] private Sprite iconInInventory;
    [SerializeField] private Sprite iconInWorld;
    [SerializeField] private bool canStack;

    public Sprite IconInInventory => iconInInventory;
    public Sprite IconInWorld => iconInWorld;
    public string ItemName => itemName;

    private int count;
    public Item(string name, Sprite invIcon, Sprite worldIcon, bool stackable, int count)
	{
        if(stackable)
		{
            this.count = count;
		}
        else
		{
            this.count = 1;
		}

        this.itemName = name;
        this.iconInInventory = invIcon;
        this.iconInWorld = worldIcon;
        this.canStack = stackable;
	}

    public void Add(int amount)
	{
        if (!canStack) return;

        count += amount;
	}

    public int Remove(int amount)
	{
        int removedCount = Mathf.Clamp(amount, 0, count);
        count -= removedCount;
        return removedCount;
	}

    public Item Take(int amount)
	{
        return new Item(itemName, iconInInventory, iconInWorld, canStack, Remove(amount));
	}

    public static Item GetMock()
	{
        return new Item("Mock", null, null, true, 1);
	}
}
