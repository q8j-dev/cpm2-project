using System;
using CarInteractionAnimationsScripts;
using Photon.Pun;
using RootMotion.FinalIK;
using UnityEngine;
using _Car_Parking.Scripts.WorldInteraction;

public class RemoteLegIkSuppressor : MonoBehaviour
{
	private MainPerson _mainPerson;

	private PhotonView _view;

	private MultiPersonControl _multiPersonControl;

	private FullBodyBipedIK _boundIk;

	private bool _hasSavedMapping;

	private float _savedLeftLegMapping;

	private float _savedRightLegMapping;

	private void Awake()
	{
		TryGetComponent<MainPerson>(out _mainPerson);
		TryGetComponent<PhotonView>(out _view);
		TryGetComponent<MultiPersonControl>(out _multiPersonControl);
	}

	private void LateUpdate()
	{
		FullBodyBipedIK fullBodyBipedIK = (_mainPerson ? _mainPerson.ik : null);
		if ((object)fullBodyBipedIK != _boundIk)
		{
			Unbind();
			if ((bool)fullBodyBipedIK)
			{
				_boundIk = fullBodyBipedIK;
				IKSolverFullBodyBiped solver = fullBodyBipedIK.solver;
				solver.OnPreUpdate = (IKSolver.UpdateDelegate)Delegate.Combine(solver.OnPreUpdate, new IKSolver.UpdateDelegate(OnSolverPreUpdate));
			}
		}
	}

	private void OnDisable()
	{
		Unbind();
	}

	private void OnDestroy()
	{
		Unbind();
	}

	private void Unbind()
	{
		if ((bool)_boundIk)
		{
			IKSolverFullBodyBiped solver = _boundIk.solver;
			solver.OnPreUpdate = (IKSolver.UpdateDelegate)Delegate.Remove(solver.OnPreUpdate, new IKSolver.UpdateDelegate(OnSolverPreUpdate));
		}
		_hasSavedMapping = false;
		_boundIk = null;
	}

	private bool IsRemoteCopy()
	{
		if ((bool)_view && _view.ViewID != 0)
		{
			return !_view.IsMine;
		}
		return false;
	}

	private bool IsPoseIkActive()
	{
		WorldInteractionController worldInteractionController = (_multiPersonControl ? _multiPersonControl.WorldInteraction : null);
		if ((bool)worldInteractionController)
		{
			return worldInteractionController.IsPoseIkActive;
		}
		return false;
	}

	public void ReleaseForPose()
	{
		if ((bool)_boundIk && _boundIk.solver.initiated)
		{
			RestoreMapping(_boundIk.solver);
		}
	}

	private void RestoreMapping(IKSolverFullBodyBiped solver)
	{
		if (_hasSavedMapping)
		{
			_hasSavedMapping = false;
			solver.leftLegMapping.weight = _savedLeftLegMapping;
			solver.rightLegMapping.weight = _savedRightLegMapping;
		}
	}

	private bool IsInCarOrCarInteraction()
	{
		if (!_multiPersonControl)
		{
			return false;
		}
		if ((bool)_multiPersonControl.currentCar)
		{
			return true;
		}
		PersonAnimationsController personAnimationsController = _multiPersonControl.PersonAnimationsController;
		if (!personAnimationsController || personAnimationsController.SitGetOutCarSystem == null || personAnimationsController.SitGetOutCarSystem.SitGetOutData == null)
		{
			return false;
		}
		if (!personAnimationsController.SitGetOutCarSystem.SitGetOutData.IsSittingIntoCarAnimate)
		{
			return personAnimationsController.SitGetOutCarSystem.SitGetOutData.IsGettingOutOfTheCarAnimate;
		}
		return true;
	}

	private void OnSolverPreUpdate()
	{
		if (!_boundIk)
		{
			return;
		}
		IKSolverFullBodyBiped solver = _boundIk.solver;
		if (!solver.initiated)
		{
			return;
		}
		if (!IsRemoteCopy() || IsInCarOrCarInteraction() || IsPoseIkActive())
		{
			RestoreMapping(solver);
			return;
		}
		if (!_hasSavedMapping)
		{
			_savedLeftLegMapping = solver.leftLegMapping.weight;
			_savedRightLegMapping = solver.rightLegMapping.weight;
			_hasSavedMapping = true;
		}
		solver.leftFootEffector.positionWeight = 0f;
		solver.leftFootEffector.rotationWeight = 0f;
		solver.rightFootEffector.positionWeight = 0f;
		solver.rightFootEffector.rotationWeight = 0f;
		solver.leftThighEffector.positionWeight = 0f;
		solver.rightThighEffector.positionWeight = 0f;
		solver.leftLegChain.bendConstraint.weight = 0f;
		solver.rightLegChain.bendConstraint.weight = 0f;
		solver.leftLegMapping.weight = 0f;
		solver.rightLegMapping.weight = 0f;
	}
}
