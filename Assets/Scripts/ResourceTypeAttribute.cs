using System;
using UnityEngine;

public class ResourceTypeAttribute : PropertyAttribute {

	private System.Type type;

	public Type ResourceType => type;
	

	public ResourceTypeAttribute() : this(typeof(GameObject)){}
	
	public ResourceTypeAttribute(System.Type resourceType)
	{
		this.type = resourceType;
	}
}
