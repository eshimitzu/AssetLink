using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class PropertyDrawerUtility
{
    public static void Setup(SerializedProperty property)
    {
        var sau = new Reflex("ScriptAttributeUtility");
        var dict = sau.GetProperty("propertyHandlerCache").GetField("m_PropertyHandlers").Value;
        if (dict is IDictionary dictionary)
        {
            foreach (DictionaryEntry entry in dictionary)
            {
                UnityEngine.Debug.Log($"{entry.Value}");
                var ph = new Reflex(entry.Value);
                if (ph.GetField("m_PropertyDrawer").Value == null)
                {
                    DefaultPropertyDrawer dpd = new DefaultPropertyDrawer();
                    ph.SetField("m_PropertyDrawer", dpd);
                }
            }
        }
    }
    
    // var sau = new Reflex("ScriptAttributeUtility");
    // var handler = sau.Invoke("GetHandler", property);
    // UnityEngine.Debug.Log($"{handler.GetField("m_PropertyDrawer").Value}");
    
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
    // this.m_PropertyDrawer = (PropertyDrawer) Activator.CreateInstance(forPropertyAndType);
    // this.m_PropertyDrawer.m_FieldInfo = field;
    // this.m_PropertyDrawer.m_Attribute = attribute;
    // }
}
