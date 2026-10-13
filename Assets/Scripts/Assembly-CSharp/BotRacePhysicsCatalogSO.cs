using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BotRacePhysicsCatalog", menuName = "Scriptable/Racing/Bot Race Physics Catalog", order = 6)]
public class BotRacePhysicsCatalogSO : ScriptableObject
{
	[Serializable]
	public struct ClassEntry
	{
		[Tooltip("Race class id as returned by CircuitRaceControlller.CurrentClassId (e.g. 8..14).")]
		public int ClassId;

		public BotRacePhysicsProfileSO Profile;
	}

	[Tooltip("Used when a class has no specific entry below. May be left empty — code defaults apply then.")]
	public BotRacePhysicsProfileSO DefaultProfile;

	[Tooltip("Per-class overrides. First matching ClassId wins.")]
	public List<ClassEntry> PerClass = new List<ClassEntry>();

	public BotRacePhysicsProfileSO GetProfileForClass(int classId)
	{
		if (PerClass != null)
		{
			for (int i = 0; i < PerClass.Count; i++)
			{
				if (PerClass[i].ClassId == classId && PerClass[i].Profile != null)
				{
					return PerClass[i].Profile;
				}
			}
		}
		return DefaultProfile;
	}
}
