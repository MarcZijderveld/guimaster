using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections;
using System.Collections.Generic;
using DictionaryExtension;

[ExecuteInEditMode]
public class GUIMaster : MonoBehaviour 
{
	public GUIStyle box;
	
	private int memberCount = 0;
	
	private static Dictionary<string, GUIMember> GUIMembers = new Dictionary<string, GUIMember>();
	
	public bool ChangeIdentifier(string oldIdentifier, string newIdentifier)
	{
		return GUIMembers.ChangeKey(oldIdentifier, newIdentifier);
	}
	
	public void UpdateMember(string identifier, GUIMember member)
	{
		OnResolutionChanged();
		GUIMembers[identifier] = member;
	}
	
	public void AddMember(string identifier, GUIMember member)
	{
		if(!GUIMembers.ContainsKey(identifier))
			GUIMembers.Add(identifier, member);
	}
	
	public void RemoveMember(string identifier)
	{
		GUIMembers.Remove(identifier);
	}
	
	public Rect ResolutionRect(Rect rectangle, ResolutionManager.scaleMode mode)
	{
		Rect returnRect = new Rect(0,0,0,0);	
		
		float scaleX = Screen.width / ResolutionManager.GetDefaultResolution().x;
		float scaleY = Screen.height / ResolutionManager.GetDefaultResolution().y;
		
//		Debug.Log("scaleX: " + scaleX + " " + " scaleY: " + scaleY);
		
		switch(mode)
		{
			case ResolutionManager.scaleMode.keepPixelSize:
				returnRect = new Rect(rectangle.x * scaleX, rectangle.y * scaleY, rectangle.width, rectangle.height);
				break;
			case ResolutionManager.scaleMode.scaleWidth:
				returnRect = new Rect(rectangle.x * scaleX, rectangle.y * scaleY, rectangle.width * scaleX, rectangle.height);
				break;
			case ResolutionManager.scaleMode.scaleHeight:
				returnRect = new Rect(rectangle.x * scaleX, rectangle.y * scaleY, rectangle.width, rectangle.height * scaleY);
				break;
			case ResolutionManager.scaleMode.scaleWithResolution:
				returnRect = new Rect(rectangle.x * scaleX, rectangle.y * scaleY, rectangle.width * scaleX, rectangle.height * scaleY);
				break;
		}
		
		returnRect = new Rect(Mathf.Round(returnRect.x), Mathf.Round(returnRect.y), Mathf.Round(returnRect.width), Mathf.Round(returnRect.height));
		
		return returnRect;
	}
	
	private void OnGUI()
	{
#if UNITY_EDITOR
		List<Transform> transforms = new List<Transform>(Selection.GetTransforms(SelectionMode.TopLevel | SelectionMode.OnlyUserModifiable));
#endif
		
		if(!Application.isPlaying)
		{
			foreach(KeyValuePair<string, GUIMember> r in GUIMembers)
			{
				if(r.Value.previewTexture != null)
				{
					GUIStyle style = new GUIStyle();
					style.normal.background = r.Value.previewTexture;
					GUI.Box(r.Value.GetScaledRect(), "", style);
				}
#if UNITY_EDITOR
				if(transforms.Contains(r.Value.transform))
				{
					GUI.color = Color.red;	
				}
#endif
				
				GUI.Box(r.Value.GetScaledRect(), "", box);
				
				GUI.color = Color.white;
			}
		}
		
		memberCount = GUIMembers.Count;
	}
	
	public void OnResolutionChanged()
	{
//		Debug.Log(GUIMembers.Count);
		foreach(KeyValuePair<string, GUIMember> r in GUIMembers)
		{
//			Debug.Log(r.Value.name);
			r.Value.SetScaling(ResolutionRect(r.Value.rect, r.Value.scaleMode));
		}
	}
	
	public Rect GetElementRect(string element)
	{
		return GUIMembers[element].GetScaledRect();
	}
}
