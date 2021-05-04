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
		
		Object newAsset = EditorGUI.ObjectField(position, ObjectNames.NicifyVariableName(property.name), asset, VisibleType, false);
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
}
