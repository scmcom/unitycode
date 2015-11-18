//EVENTS MANAGER CLASS
// for receiving notifications and notifying listeners
//==================================================================
using UnityEngine;
using System.Collections;
using System.Collectins.Generic;

public class NotificationsManager : MonoBehaviour
{

	//==================================================================
	// Private Variables
	//==================================================================
	//internal reference to all listeners for notifications

				//.NET: Dictionary<TKey, TValue>

	//strings are events - ie KEY VALUES
	//listeners are lists of components
	private Dictionary<string, List<Component>> Listeners = 
		new Dictionary<string, List<Component>>();

	//to access ALL Listeners:
		// List MyListenersList = Listeners[N];

	//==================================================================
	// Methods
	//==================================================================
	// method to add a listener to NM sig 
	public void AddListener(Component Listener string NotificationName)
	{
		//add listener to dictionary
		if(!Listeners.ContainsKey(NotificationName))
		{
			Listeners.Add(NotificationName, new List<Component>());
		}
		//add object to listener list for this notification
		Listeners[NotificationName].Add(Listener);
	}
	//==================================================================
	// method to remove a listener for a notification
	public void RemoveListener(Component Sender, string NotificationName)
	{
		//if no key in dictionary exists, then exit
		if(!Listeners.ContainsKey(NotificationName)) return;

		// cycle through listeners and identify component, and then remove
		for(int i = Listeners[NotificationName].Count-1; i>=0; i--)
		{
			// check instance ID
			if(Listeners[NotificationName][i].GetInstanceID() == Sender.GetInstanceID())
				Listeners[NotificationName].RemoveAt(i); 	//matched. remove from list
		}
	}
	//==================================================================
	// method to post a notification to a listener
	public void PostNotification(ComponentListener, string NotificationName)
	{
		//if no key in dictionary exists, then exit
		if(!Listeners.ContainsKey(NotificationName)) return;

		// else post notification to all matching listeners
		foreach(Component Listener in Listeners[NotificationName])
			Listener.SendMessage(NotificationName, Listener,
				SendMessageOptions.DontRequireReceiver);
	}
	//==================================================================
	// method to clear all listeners
	public void ClearListeners()
	{
		//remove all listeners
		Listeners.Clear();
	}
	//==================================================================
	// method to remove redundant listeners - deleted and removed listeners
	public void RemoveRedundancies()
	{
		//create new dictionary
		Dictionary<string, List<Component>> TmpListeners = 
			new Dictionary<string, List<Component>>();

		//cycle through all dictionary entries
		foreach(KeyValuePair<string, List<Component>> Item in Listeners)
		{
			//cycle through all listener objects in list, remove null objects
			for(int i = Item.Value.Count-1; i>=0; i--)
			{
				//if null then remove item
				if(Item.Value[i] == null)
				{
					Item.Value.RemoveAt(i);
				}
				if(Item.Value.Count > 0)
				{
					TmpListeners.Add(Item.Key, Item.Value);
				}
			}
		}
		//replace listeners object with new, optimized dictionary
		Listeners = TmpListeners;
	}
	//==================================================================
	// method to call when a new level is loaded; 
	// remove redundant entries from dictionary; 
	// in case left-over from previous scene
	void OnLevelWasLoaded()
	{
		//clear redundancies 
		RemoveRedundancies();
	}
	//==================================================================
}
