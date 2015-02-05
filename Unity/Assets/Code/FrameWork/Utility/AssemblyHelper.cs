using UnityEngine;
using System.Collections;
using System.Reflection;

public class AssemblyHelper 
{
    public static Assembly GetCSharpAssembly()
    {
        return Assembly.GetExecutingAssembly();
    }

}
