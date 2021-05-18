using System;
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.Reflection;
using System.Collections;
using Object = UnityEngine.Object;


[CustomPropertyDrawer(typeof(ResourceTypeAttribute))]
[CustomPropertyDrawer(typeof(AssetLink))]
public class AssetLinkDrawer : PropertyDrawer
{
	private ResourceTypeAttribute NamedAttribute => ((ResourceTypeAttribute)attribute);
	private Type VisibleType => NamedAttribute?.ResourceType ?? typeof(GameObject);

	private static Dictionary<string, Object> guidAssetMapper = new Dictionary<string, Object>();
	private static MethodInfo SetAsset = typeof(AssetLink).GetMethod("SetAsset", BindingFlags.Instance | BindingFlags.NonPublic);

	
	private static Object GUIDToAsset(string guid, System.Type type)
	{
		if(!guidAssetMapper.TryGetValue(guid, out Object asset) || (asset == null))
		{
			string assetPath = AssetDatabase.GUIDToAssetPath(guid);
			asset = AssetDatabase.LoadAssetAtPath(assetPath, type);
			guidAssetMapper.Remove(guid);
			guidAssetMapper.Add(guid, asset);
		}

		return asset;
	}


	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
	{		
		EditorGUI.BeginProperty(position, label, property);

		Object asset = null;
		SerializedProperty assetGUIDProperty = property.FindPropertyRelative("assetGUID");

		if(!string.IsNullOrEmpty(assetGUIDProperty.stringValue))
		{
			asset = GUIDToAsset(assetGUIDProperty.stringValue, VisibleType);
		}

		bool isMissing = !string.IsNullOrEmpty(assetGUIDProperty.stringValue) && !asset;

		var color = GUI.contentColor;
		GUI.contentColor = isMissing ? Color.red : color;
		
		Object newAsset = EditorGUI.ObjectField(position, GetDisplayName(property, label), asset, VisibleType, false);
		
		GUI.contentColor = color;
		
		if(asset != newAsset)
		{
			AssetLink link = GetTarget(property);

			SetAsset.Invoke(link, new object[] {newAsset});
			property.serializedObject.ApplyModifiedProperties();

			EditorUtility.SetDirty(property.serializedObject.targetObject);
		}

		DropAreaGUI(position, property, label);

		EditorGUI.EndProperty();
	}
	

	//The way to receive AssetLink from SerializedProperty
	private AssetLink GetTarget(SerializedProperty property)
	{
		object current = property.serializedObject.targetObject;
		string[] fields = property.propertyPath.Split('.');

		for (int i = 0; i < fields.Length; i++) 
		{
			string fieldName = fields[i];

			if(fieldName.Equals("Array"))
			{
				fieldName = fields[++i];
				string indexString = fieldName.Substring(5, fieldName.Length - 6);
				int index = int.Parse(indexString);

				System.Type type = current.GetType();
				if(type.IsArray)
				{
					System.Array array = current as System.Array;
					current = array.GetValue(index);
				}
				else if(type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>)) 
				{
					IList list = current as IList;
					current = list[index];
				}
			}
			else
			{
				FieldInfo field = current.GetType().GetField(fields[i], BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
				current = field.GetValue(current);
			}
		}

		return current as AssetLink;
	}


	private GUIContent GetDisplayName(SerializedProperty property, GUIContent label)
	{
		if(IsArray(property, out int index))
		{
			label = new GUIContent($"Element {index}");
		}
		
		return label;
	}


	private bool IsArray(SerializedProperty property, out int index)
	{
		index = -1;
		
		string[] fields = property.propertyPath.Split('.');
		
		if (fields.Length > 1)
		{
			string array = fields[fields.Length - 2];
			if (array.Equals("Array"))
			{
				string element = fields[fields.Length - 1];
				string indexStr = element.Split('[', ']')[1];
				index = int.Parse(indexStr);
				
				return true;
			}
		}

		return false;
	}
	
	
	private SerializedProperty GetParent(SerializedProperty current)
	{
		SerializedProperty parent = null;

		int depth = current.propertyPath.Split('.').Length;
		
		var serializedProperty = current.serializedObject.GetIterator();
		while (serializedProperty.NextVisible(true))
		{
			if (current.propertyPath.Contains(serializedProperty.propertyPath))
			{
				int pDepth = serializedProperty.propertyPath.Split('.').Length;
				if (pDepth < depth)
				{
					parent = serializedProperty.Copy();
				}
			}
		}

		return parent;
	}
	
	
	private IList GetArray(SerializedProperty property)
	{
		object current = property.serializedObject.targetObject;
		string[] fields = property.propertyPath.Split('.');

		for (int i = 0; i < fields.Length - 2; i++) 
		{
			string fieldName = fields[i];

			if(fieldName.Equals("Array"))
			{
				fieldName = fields[++i];
				string indexString = fieldName.Substring(5, fieldName.Length - 6);
				int index = int.Parse(indexString);

				System.Type type = current.GetType();
				if(type.IsArray)
				{
					System.Array array = current as System.Array;
					current = array.GetValue(index);
				}
				else if(type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>)) 
				{
					IList list = current as IList;
					current = list[index];
				}
			}
			else
			{
				FieldInfo field = current.GetType().GetField(fields[i], BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
				current = field.GetValue(current);
			}
		}

		return current as IList;
	}
	
	
	private void SetArray(SerializedProperty property, IList value)
	{
		object current = property.serializedObject.targetObject;
		string[] fields = property.propertyPath.Split('.');

		for (int i = 0; i < fields.Length - 3; i++) 
		{
			string fieldName = fields[i];

			if(fieldName.Equals("Array"))
			{
				fieldName = fields[++i];
				string indexString = fieldName.Substring(5, fieldName.Length - 6);
				int index = int.Parse(indexString);

				System.Type type = current.GetType();
				if(type.IsArray)
				{
					System.Array array = current as System.Array;
					current = array.GetValue(index);
				}
				else if(type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>)) 
				{
					IList list = current as IList;
					current = list[index];
				}
			}
			else
			{
				FieldInfo field = current.GetType().GetField(fields[i], BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
				current = field.GetValue(current);
			}
		}

		{
			FieldInfo field = current.GetType().GetField(fields[fields.Length - 3], BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			field?.SetValue(current, value);
		}
	}
	
	
	public void DropAreaGUI (Rect position, SerializedProperty property, GUIContent label)
	{
		if(IsArray(property, out int index) && index == 0)
		{
			Event evt = Event.current;

			position.y -= 18 * 2;
     
			switch (evt.type) {
				case EventType.DragUpdated:
				case EventType.DragPerform:
					if (!position.Contains (evt.mousePosition))
						return;
             
					DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
         
					if (evt.type == EventType.DragPerform) 
					{
						DragAndDrop.AcceptDrag ();
             
						IList array = GetArray(property);
						if (array != null)
						{
							List<AssetLink> links = new List<AssetLink>();
							foreach (Object drag in DragAndDrop.objectReferences) 
							{
								if(VisibleType.IsInstanceOfType(drag))
								{
									var link = new AssetLink(drag);
									if (link.IsResource)
									{
										links.Add(link);
									}
								}
							}
							
							if (!array.IsFixedSize)
							{
								foreach (AssetLink link in links)
								{
									array.Add(link);
								}
							}
							else if (array is AssetLink[] linksArray)
							{
								int size = linksArray.Length;
								System.Array.Resize(ref linksArray, size + links.Count);
								for (int i = 0; i < links.Count; i++)
								{
									linksArray[size + i] = links[i];
								}
								SetArray(property, linksArray);
							}
						}
					}
					break;
			}
		}
	}
}
