using System;
using Car_Parking.Scripts.Racing;
using UnityEngine;
using _Car_Parking.Scripts.Analytics.Analyzers;

public interface IRaceResultCounter : IDisposable
{
	void OnStartRace();

	void Init(FreeRaceCar freeRaceCar);

	float GetFinalResult();

	float GetCurrentResult();

	void Restart();

	void OnCarToRoad(GameObject raceCarGameObject);

	TimeAttackAnalyticData GetAnalyticData();
}
