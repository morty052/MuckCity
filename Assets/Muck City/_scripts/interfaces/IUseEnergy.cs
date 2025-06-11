

using UnityEngine;

public interface IUseEnergy
{
    float EnergyLevel { get; set; } // Property to get or set the energy level of the object
    bool IsPowered { get; } // Property to check if the object is powered

    float EnergyNeededToFunction { get; } // Property to check if energy is needed for the object to function

    Transform ChargingPort { get; }
    void PowerUp(float amount); // Method to power up the object with a specified amount of energy

    public void PromptToCharge(); // Method to prompt the player to charge the object if needed

    public void OnChargeComplete(); // Method to be called when the charging is complete
}
