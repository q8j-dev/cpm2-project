using System;
using System.Collections.Generic;
using UnityEngine;
using _Car_Parking.Scripts.Rally.Data;

namespace _Car_Parking.Scripts.Racing_Scripts
{
	public class RallyPilotSoundController
	{
		public const string PilotSwitchGroup = "Rally_Pilot_Switch";

		public const string PilotEvent = "Play_Rally_2Pilot_Talk";

		public const string PilotVersionPref = "RallyPilotVersion";

		private int _lookahead;

		private readonly RallySequenceData _sequenceData;

		private readonly Queue<string[]> _queue = new Queue<string[]>();

		private readonly Queue<string[]> _priorityQueue = new Queue<string[]>();

		private string[] _activeGroup;

		private int _activeGroupIdx;

		private bool _isPlaying;

		private bool _isRaceActive;

		private int _announcedUpToIndex = -1;

		private GameObject _target;

		private readonly AkCallbackManager.EventCallback _onEventEnd;

		public int AnnounceStep { get; set; }

		private string CurrentEvent => "Play_Rally_2Pilot_Talk";

		public RallyPilotSoundController(GameObject target, RallySequenceData sequenceData)
		{
			_target = target;
			_sequenceData = sequenceData;
			_onEventEnd = OnEventEnd;
		}

		public void AnnounceItem(RallySequenceItem item)
		{
			if (!_isRaceActive)
			{
				return;
			}
			int num = _sequenceData.RallySequenceItems.IndexOf(item);
			if (num >= 0)
			{
				int num2 = num + AnnounceStep;
				if (num2 > _announcedUpToIndex && num2 < _sequenceData.RallySequenceItems.Count)
				{
					_announcedUpToIndex = num2;
					EnqueueItem(_sequenceData.RallySequenceItems[num2]);
				}
			}
		}

		private void EnqueueItem(RallySequenceItem item)
		{
			List<string> list = BuildPhrase(item);
			if (list.Count > 0)
			{
				_queue.Enqueue(list.ToArray());
			}
			TryPlayNext();
		}

		private List<string> BuildPhrase(RallySequenceItem item)
		{
			List<string> list = new List<string>();
			if (item.SequenceTurnType == RallySequenceTurnType.None)
			{
				string distanceSwitch = GetDistanceSwitch(item.Distance);
				if (distanceSwitch != null)
				{
					list.Add(distanceSwitch);
				}
			}
			list.AddRange(GetTurnSwitches(item.SequenceTurnType));
			RallySequenceMark[] sortedMarks = GetSortedMarks(item.Mark);
			for (int i = 0; i < sortedMarks.Length; i++)
			{
				string markSwitch = GetMarkSwitch(sortedMarks[i]);
				if (markSwitch != null)
				{
					list.Add(markSwitch);
				}
			}
			if (item.Link != RallySequenceLink.None)
			{
				string linkSwitch = GetLinkSwitch(item.Link);
				if (linkSwitch != null)
				{
					list.Add(linkSwitch);
				}
			}
			return list;
		}

		private void TryPlayNext()
		{
			if (_isPlaying)
			{
				return;
			}
			if (_activeGroup != null && _activeGroupIdx < _activeGroup.Length)
			{
				PlaySwitch(_activeGroup[_activeGroupIdx++]);
				return;
			}
			_activeGroup = null;
			if (_priorityQueue.Count != 0 || _queue.Count != 0)
			{
				_activeGroup = ((_priorityQueue.Count > 0) ? _priorityQueue.Dequeue() : _queue.Dequeue());
				_activeGroupIdx = 0;
				if (_activeGroup.Length == 0)
				{
					_activeGroup = null;
					TryPlayNext();
				}
				else
				{
					PlaySwitch(_activeGroup[_activeGroupIdx++]);
				}
			}
		}

		private void PlaySwitch(string sw)
		{
			_isPlaying = true;
			AkSoundEngine.SetSwitch("Rally_Pilot_Switch", sw, _target);
			AkSoundEngine.PostEvent(CurrentEvent, _target, 1u, _onEventEnd, null);
		}

		private void OnEventEnd(object cookie, AkCallbackType type, AkCallbackInfo info)
		{
			_isPlaying = false;
			TryPlayNext();
		}

		public void ChangePacenotesQueueCount(int count)
		{
			_lookahead = count + 1;
			AnnounceStep = count;
		}

		public void OnRaceStart()
		{
			_isRaceActive = true;
			_announcedUpToIndex = -1;
			_queue.Enqueue(new string[1] { "start_go" });
			int count = _sequenceData.RallySequenceItems.Count;
			for (int i = 0; i < _lookahead && i < count; i++)
			{
				EnqueueItem(_sequenceData.RallySequenceItems[i]);
				_announcedUpToIndex = i;
			}
			TryPlayNext();
		}

		public void OnRaceFinish()
		{
			_isRaceActive = false;
			_queue.Clear();
			_priorityQueue.Clear();
			_isPlaying = false;
			_activeGroup = null;
			_activeGroupIdx = 0;
			_queue.Enqueue(new string[1] { "finish_approach" });
			TryPlayNext();
		}

		public void PlayStopControl()
		{
			_isRaceActive = false;
			_queue.Clear();
			_priorityQueue.Clear();
			_isPlaying = false;
			_activeGroup = null;
			_activeGroupIdx = 0;
			_queue.Enqueue(new string[1] { "stop_control" });
			TryPlayNext();
		}

		public void PlayCoPilotComments(CopilotCommentsType commentsType)
		{
			string text = commentsType switch
			{
				CopilotCommentsType.NotMoving => "penalty_notmoving", 
				CopilotCommentsType.WrongDirection => "penalty_wrongdirection", 
				CopilotCommentsType.CheckpointSkip => "penalty_checkpointskip", 
				CopilotCommentsType.Offtrack => "penalty_offtrack", 
				CopilotCommentsType.FrequentCrash => "penalty_frequentcrash", 
				_ => null, 
			};
			if (text != null)
			{
				_priorityQueue.Enqueue(new string[1] { text });
				TryPlayNext();
			}
		}

		public void PlayFinishBadResult()
		{
			_isRaceActive = false;
			_queue.Clear();
			_priorityQueue.Clear();
			_isPlaying = false;
			_activeGroup = null;
			_activeGroupIdx = 0;
			_queue.Enqueue(new string[1] { "dq" });
			TryPlayNext();
		}

		public void PlayFinishGoodResult(bool badResult = false)
		{
			_isRaceActive = false;
			_queue.Clear();
			_priorityQueue.Clear();
			_isPlaying = false;
			_activeGroup = null;
			_activeGroupIdx = 0;
			_queue.Enqueue(new string[1] { badResult ? "finish_good_result" : "finish_bad_result" });
			TryPlayNext();
		}

		public void StopAll()
		{
			_isRaceActive = false;
			_queue.Clear();
			_priorityQueue.Clear();
			_isPlaying = false;
			_activeGroup = null;
			_activeGroupIdx = 0;
		}

		public void Reset()
		{
			_isRaceActive = false;
			_queue.Clear();
			_priorityQueue.Clear();
			_isPlaying = false;
			_activeGroup = null;
			_activeGroupIdx = 0;
			_announcedUpToIndex = -1;
		}

		public void Dispose()
		{
			_isRaceActive = false;
			_queue.Clear();
			_priorityQueue.Clear();
			_isPlaying = false;
			_activeGroup = null;
			_activeGroupIdx = 0;
			AkSoundEngine.StopAll(_target);
		}

		private static string[] GetTurnSwitches(RallySequenceTurnType type)
		{
			return type switch
			{
				RallySequenceTurnType.Left6 => new string[2] { "n_six", "left" }, 
				RallySequenceTurnType.Left5 => new string[2] { "n_five", "left" }, 
				RallySequenceTurnType.Left4 => new string[2] { "n_four", "left" }, 
				RallySequenceTurnType.Left3 => new string[2] { "n_three", "left" }, 
				RallySequenceTurnType.Left2 => new string[2] { "n_two", "left" }, 
				RallySequenceTurnType.Left1 => new string[2] { "n_one", "left" }, 
				RallySequenceTurnType.Right6 => new string[2] { "n_six", "right" }, 
				RallySequenceTurnType.Right5 => new string[2] { "n_five", "right" }, 
				RallySequenceTurnType.Right4 => new string[2] { "n_four", "right" }, 
				RallySequenceTurnType.Right3 => new string[2] { "n_three", "right" }, 
				RallySequenceTurnType.Right2 => new string[2] { "n_two", "right" }, 
				RallySequenceTurnType.Right1 => new string[2] { "n_one", "right" }, 
				RallySequenceTurnType.KinkLeft => new string[1] { "kink_left" }, 
				RallySequenceTurnType.KinkRight => new string[1] { "kink_right" }, 
				RallySequenceTurnType.Kinks => new string[1] { "kinks" }, 
				RallySequenceTurnType.FlatLeft => new string[1] { "flat_left" }, 
				RallySequenceTurnType.FlatRight => new string[1] { "flat_right" }, 
				RallySequenceTurnType.SquareLeft => new string[1] { "square_left" }, 
				RallySequenceTurnType.SquareRight => new string[1] { "square_right" }, 
				RallySequenceTurnType.SuddenLeft => new string[1] { "sudden_left" }, 
				RallySequenceTurnType.SuddenRight => new string[1] { "sudden_right" }, 
				RallySequenceTurnType.HairpinLeft => new string[1] { "hairpin_left" }, 
				RallySequenceTurnType.HairpinRight => new string[1] { "hairpin_right" }, 
				RallySequenceTurnType.TightLeft => new string[1] { "tight_hairpin_left" }, 
				RallySequenceTurnType.TightRight => new string[1] { "tight_hairpin_right" }, 
				RallySequenceTurnType.ChicaneLeft => new string[1] { "chicane_left" }, 
				RallySequenceTurnType.ChicaneRight => new string[1] { "chicane_right" }, 
				_ => Array.Empty<string>(), 
			};
		}

		private static string GetDistanceSwitch(float distance)
		{
			return (int)(Math.Round((double)distance / 10.0) * 10.0) switch
			{
				10 => "n_ten", 
				20 => "n_twenty", 
				30 => "n_thirty", 
				40 => "n_forty", 
				50 => "n_fifty", 
				60 => "n_sixty", 
				70 => "n_seventy", 
				80 => "n_eighty", 
				90 => "n_ninety", 
				100 => "n_One_Hundred", 
				110 => "n_One_Hundred_Ten", 
				120 => "n_One_Hundred_Twenty", 
				130 => "n_One_Hundred_Thirty", 
				140 => "n_One_Hundred_Forty", 
				150 => "n_One_Hundred_Fifty", 
				160 => "n_One_Hundred_Sixty", 
				170 => "n_One_Hundred_Seventy", 
				180 => "n_One_Hundred_Eighty", 
				190 => "n_One_Hundred_Ninety", 
				200 => "n_Two_Hundred", 
				210 => "n_Two_Hundred_Ten", 
				220 => "n_Two_Hundred_Twenty", 
				230 => "n_Two_Hundred_Thirty", 
				240 => "n_Two_Hundred_Forty", 
				250 => "n_Two_Hundred_Fifty", 
				_ => null, 
			};
		}

		private static string GetLinkSwitch(RallySequenceLink link)
		{
			return link switch
			{
				RallySequenceLink.And => "link_word_and", 
				RallySequenceLink.Into => "link_word_into", 
				_ => null, 
			};
		}

		private static string GetMarkSwitch(RallySequenceMark? mark)
		{
			if (!mark.HasValue || mark.Value == RallySequenceMark.None)
			{
				return null;
			}
			return mark.Value switch
			{
				RallySequenceMark.Caution => "warning_caution", 
				RallySequenceMark.Cut => "warning_cut", 
				RallySequenceMark.DontCut => "warning_dontcut", 
				RallySequenceMark.Brake => "warning_brake", 
				RallySequenceMark.HandBrake => "warning_handbrake", 
				RallySequenceMark.WarningInside => "warning_inside", 
				RallySequenceMark.WarningOutside => "warning_outside", 
				RallySequenceMark.KeepMiddle => "warning_keepmiddle", 
				RallySequenceMark.KeepLeft => "warning_keepleft", 
				RallySequenceMark.KeepRight => "warning_keepright", 
				RallySequenceMark.BadCamber => "warning_badcamber", 
				RallySequenceMark.Blind => "warning_blind", 
				RallySequenceMark.Hidden => "warning_hidden", 
				RallySequenceMark.Slippery => "warning_slippery", 
				RallySequenceMark.Maybe => "warning_maybe", 
				RallySequenceMark.Opens => "corner_detail_opens", 
				RallySequenceMark.Tightens => "corner_detail_tightens", 
				RallySequenceMark.Narrows => "corner_detail_narrows", 
				RallySequenceMark.Widens => "corner_detail_widens", 
				RallySequenceMark.Long => "corner_detail_long", 
				RallySequenceMark.VeryLong => "corner_detail_verylong", 
				RallySequenceMark.Short => "corner_detail_short", 
				RallySequenceMark.Early => "corner_detail_early", 
				RallySequenceMark.Late => "corner_detail_late", 
				RallySequenceMark.Suddden => "corner_detail_sudden", 
				RallySequenceMark.OverCrest => "corner_detail_overcrest", 
				RallySequenceMark.OverJump => "corner_detail_overjump", 
				RallySequenceMark.ThroughDip => "corner_detail_throughdip", 
				RallySequenceMark.Kinks => "kinks", 
				_ => null, 
			};
		}

		private static RallySequenceMark[] GetSortedMarks(RallySequenceMark[] marks)
		{
			if (marks == null || marks.Length == 0)
			{
				return Array.Empty<RallySequenceMark>();
			}
			RallySequenceMark[] array = new RallySequenceMark[12]
			{
				RallySequenceMark.Caution,
				RallySequenceMark.DontCut,
				RallySequenceMark.HandBrake,
				RallySequenceMark.Brake,
				RallySequenceMark.Tightens,
				RallySequenceMark.Opens,
				RallySequenceMark.OverCrest,
				RallySequenceMark.OverJump,
				RallySequenceMark.Blind,
				RallySequenceMark.Long,
				RallySequenceMark.VeryLong,
				RallySequenceMark.Short
			};
			List<RallySequenceMark> list = new List<RallySequenceMark>();
			RallySequenceMark[] array2 = array;
			foreach (RallySequenceMark rallySequenceMark in array2)
			{
				foreach (RallySequenceMark rallySequenceMark2 in marks)
				{
					if (rallySequenceMark2 == rallySequenceMark)
					{
						list.Add(rallySequenceMark2);
						break;
					}
				}
			}
			array2 = marks;
			foreach (RallySequenceMark rallySequenceMark3 in array2)
			{
				if (Array.IndexOf(array, rallySequenceMark3) < 0 && !list.Contains(rallySequenceMark3))
				{
					list.Add(rallySequenceMark3);
				}
			}
			return list.ToArray();
		}
	}
}
