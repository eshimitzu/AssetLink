using System;
using System.Reflection;

public class Reflex
{
    private object obj;
    private Type type;

    public object Value => obj;
    public Type Type => type;

    
    public Reflex(object obj, Type type)
    {
        this.obj = obj;
        this.type = type;
    }


    public Reflex(object obj)
    {
        this.obj = obj;
        this.type = obj.GetType();
    }
    
    
    public Reflex(Type type)
    {
        this.obj = null;
        this.type = type;
    }
    
    
    public Reflex(string typeName)
    {
        this.obj = null;
        this.type = GetType(typeName);
    }
    
    
    public Reflex GetField(string name)
    {
        var field = type.GetField(name, BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);
        if (field != null)
        {
            object result = field.GetValue(obj);
            return new Reflex(result, field.FieldType);
        }

        return null;
    }
    
    
    public Reflex SetField(string name, object value)
    {
        var field = type.GetField(name, BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);
        if (field != null)
        {
            field.SetValue(obj, value);
        }

        return this;
    }
    
    
    public Reflex GetProperty(string name)
    {
        var property = type.GetProperty(name, BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.GetProperty);
        if (property != null)
        {
            object result = property.GetValue(obj);
            return new Reflex(result, property.PropertyType);
        }

        return null;
    }
    
    
    public Reflex SetProperty(string name, object value)
    {
        var property = type.GetProperty(name, BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.SetProperty);
        if (property != null)
        {
            property.SetValue(obj, value);
        }

        return this;
    }
    

    public Reflex Invoke(string name, params object[] parameters)
    {
        MethodInfo method = type.GetMethod(name, BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);
        if (method != null)
        {
            if (method.ReturnType == typeof(void))
            {
                method.Invoke(obj, parameters);
                return this;
            }
            else
            {
                object result = method.Invoke(obj, parameters);
                return new Reflex(result);
            }
        }

        return null;
    }
    
    // UnityEngine.Debug.Log($"{property.propertyPath}");
    // if(property.isArray)
    // return;
    //     		
    // var type = GetCurrentType("ScriptAttributeUtility");
    // var method = type.GetMethod("GetHandler", BindingFlags.Static | BindingFlags.NonPublic);
    //     		
    // var array = GetParent(property);
    // var parameters = new object[] { array };
    // var handler = method.Invoke(null, parameters);
    //     		
    //     if (handler != null)
    // {
    //     var handlerType = handler.GetType();
    //     var m_PropertyDrawer = handlerType.GetField("m_PropertyDrawer", BindingFlags.Instance | BindingFlags.NonPublic);
    //     m_PropertyDrawer.SetValue(handler, this);
    //     			
    //     			
    //     // this.m_PropertyDrawer = (PropertyDrawer) Activator.CreateInstance(forPropertyAndType);
    //     // this.m_PropertyDrawer.m_FieldInfo = field;
    //     // this.m_PropertyDrawer.m_Attribute = attribute;
    // }
    
        
    
    private Type GetType(string name)
    {
        foreach(Assembly a in AppDomain.CurrentDomain.GetAssemblies())
        {
            Type[] types = a.GetTypes();

            foreach(Type t in types)
            {
                if(t.Name.Equals(name))
                {
                    return t;
                }
            }
        }

        return null;
    }
}
