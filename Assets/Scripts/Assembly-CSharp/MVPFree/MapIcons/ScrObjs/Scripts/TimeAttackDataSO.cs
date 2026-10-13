using UnityEngine;

namespace MVPFree.MapIcons.ScrObjs.Scripts
{
	[CreateAssetMenu(fileName = "TimeAttackData", menuName = "Scriptable/TimeAttackData")]
	public class TimeAttackDataSO : ScriptableObject
	{
		public TimeAttackData[] TimeAttackDatas;
	}
}
