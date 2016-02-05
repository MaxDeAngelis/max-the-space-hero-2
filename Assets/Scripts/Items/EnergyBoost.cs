using UnityEngine;
using System.Collections;

public class EnergyBoost : Boost {
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 								     		PROTECTED FUNCTION											     ///
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// <summary>
	/// Initiates the energy boost
	/// </summary>
	protected override void initiate() {
		// Set the new boosted max energy and add the same amount to current energy
		EnergyManager.Instance.setMaxEnergy(EnergyManager.Instance.getMaxEnergy() + bonus);
		EnergyManager.Instance.addEnergy(bonus);

		// Call base version for core functionality
		base.initiate();
	}

	/// <summary>
	/// Extenstion point to be used for finishing the boost
	/// </summary>
	protected override void finish() {
		// When duration is finished then revert the max energy
		EnergyManager.Instance.setMaxEnergy(EnergyManager.Instance.getMaxEnergy() - bonus);

		// Call base version for core functionality
		base.finish();
	}
}
