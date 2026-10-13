using System;
using UnityEngine;

[AttributeUsage(AttributeTargets.Field)]
public class StringDropDownAttribute : PropertyAttribute
{
	public string path;
}
