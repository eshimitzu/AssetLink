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
		Object asset = null;
		SerializedProperty assetGUIDProperty = property.FindPropertyRelative("assetGUID");

		if(!string.IsNullOrEmpty(assetGUIDProperty.stringValue))
		{
			asset = GUIDToAsset(assetGUIDProperty.stringValue, VisibleType);
		}
		
		Object newAsset = EditorGUI.ObjectField(position, GetDisplayName(property, label), asset, VisibleType, false);
		if(asset != newAsset)
		{
			AssetLink link = GetTarget(property);

			SetAsset.Invoke(link, new object[] {newAsset});
			property.serializedObject.ApplyModifiedProperties();

			EditorUtility.SetDirty(property.serializedObject.targetObject);
		}
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
	
	
	
	private string dragDropIdentifier;
	
	protected void ExampleDragDropGUI(Rect dropArea, SerializedProperty property)
	{
		// Cache References:
		Event currentEvent = Event.current;
		EventType currentEventType = currentEvent.type;
	   
		// The DragExited event does not have the same mouse position data as the other events,
		// so it must be checked now:
		if ( currentEventType == EventType.DragExited )
			DragAndDrop.PrepareStartDrag();// Clear generic data when user pressed escape. (Unfortunately, DragExited is also called when the mouse leaves the drag area)
 
		if (!dropArea.Contains(currentEvent.mousePosition))
			return;
 
		switch (currentEventType){
			case EventType.MouseDown:
				DragAndDrop.PrepareStartDrag();// reset data
   
				CustomDragData dragData = new CustomDragData();
				// dragData.originalIndex = somethingYouGotFromYourProperty;
				// dragData.originalList = this.targetList;
   	
				DragAndDrop.SetGenericData(dragDropIdentifier, dragData);
   
				Object[] objectReferences = new Object[1]{property.objectReferenceValue};// Careful, null values cause exceptions in existing editor code.
				DragAndDrop.objectReferences = objectReferences;// Note: this object won't be 'get'-able until the next GUI event.
   
				currentEvent.Use();
   
				break;
			case EventType.MouseDrag:
				// If drag was started here:
				CustomDragData existingDragData = DragAndDrop.GetGenericData(dragDropIdentifier) as CustomDragData;
       
				if (existingDragData != null){
					DragAndDrop.StartDrag("Dragging List ELement");
					currentEvent.Use();
				}
       
				break;
			case EventType.DragUpdated:
				if (IsDragTargetValid())
				{
					DragAndDrop.visualMode = DragAndDropVisualMode.Link;
				}
				else
				{
					DragAndDrop.visualMode = DragAndDropVisualMode.Rejected;
				}
       
				currentEvent.Use();
				break;      
			case EventType.Repaint:
				if (
					DragAndDrop.visualMode == DragAndDropVisualMode.None||
					DragAndDrop.visualMode == DragAndDropVisualMode.Rejected) break;
       
				EditorGUI.DrawRect(dropArea, Color.grey);      
				break;
			case EventType.DragPerform:
				DragAndDrop.AcceptDrag();
       
				CustomDragData receivedDragData = DragAndDrop.GetGenericData(dragDropIdentifier) as CustomDragData;

				// if (receivedDragData != null && receivedDragData.originalList == this.targetList)
				// {
				// 	ReorderObject();
				// }
				// else
				// {
				// 	AddDraggedObjectsToList();
				// }
       
				currentEvent.Use();
				break;
			case EventType.MouseUp:
				// Clean up, in case MouseDrag never occurred:
				DragAndDrop.PrepareStartDrag();
				break;
		}
	}

	private bool IsDragTargetValid()
	{
		return false;
	}


	public class CustomDragData{
		public int originalIndex;
		public IList originalList;
	}
}
