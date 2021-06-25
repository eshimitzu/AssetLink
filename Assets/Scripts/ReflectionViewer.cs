using UnityEngine;
using System;
using System.Reflection;


[ExecuteInEditMode]
public class ReflectionViewer : MonoBehaviour 
{
	#region Variables
    public BindingFlags flags = BindingFlags.Static | BindingFlags.Public | BindingFlags.Instance;
    public String type = "ReflectionViewer";

    public string Btn1 = "PrintFields";
    public string Btn2 = "PrintProperties";
    public string Btn3 = "PrintMethods";
    public string Btn4 = "PrintEvents";
	#endregion


	private void OnGUI()
	{
		if (GUI.Button(new Rect(100, 100, 100, 100), "PrintMethods"))
		{
			PrintMethods();
		}
	}


	#region Public

    public void PrintFields()
    {
        Type targetType = GetCurrentType();
        FieldInfo[] fields = targetType.GetFields(flags);

        foreach(FieldInfo field in fields)
        {
            Debug.Log(field);
        }
    }


    public void PrintProperties()
    {
        Type targetType = GetCurrentType();
        PropertyInfo[] properties = targetType.GetProperties(flags);

        foreach(PropertyInfo property in properties)
        {
            Debug.Log(property);
        }
    }


    public void PrintMethods()
    {
        Type targetType = GetCurrentType();
        MethodInfo[] methods = targetType.GetMethods(flags);

        foreach(MethodInfo method in methods)
        {
            Debug.Log(method);
        }
    }


    public void PrintEvents()
    {
        Type targetType = GetCurrentType();
        EventInfo[] events = targetType.GetEvents(flags);

        foreach(EventInfo e in events)
        {
            Debug.Log(e);
        }
    }

	#endregion


	#region Private

    Type GetCurrentType()
    {
		foreach(Assembly a in AppDomain.CurrentDomain.GetAssemblies())
		{
			Type[] types = a.GetTypes();

			foreach(Type t in types)
			{
				if(t.Name.Equals(type))
				{
					return t;
				}
			}
		}

		return null;
    }

	#endregion
}