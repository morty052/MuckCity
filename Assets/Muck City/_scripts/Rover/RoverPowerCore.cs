using System;
using System.Collections;
using System.Threading.Tasks;
using GogoGaga.OptimizedRopesAndCables;
using UnityEngine;
using UnityEngine.AI;

public class RoverPowerCore : MonoBehaviour
{
    readonly float _energyTransferRate = 1f; // Energy transfer rate per second
    readonly float _energyPerSecond = 5f; // Energy transferred per second

    [SerializeField] float _reserveEnergyLevel = 100f; // Energy level of the rover

    public Rope _cable;

    [SerializeField] AudioClip _ropeAttachSound;
    [SerializeField] AudioClip _energyTransferSound;
    [SerializeField] AudioClip _energyFullSound;

    AudioSource _audioSource;


    void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
    }


    public async void TransferEnergy(IUseEnergy receiver, GameObject chargingPort, Action OnComplete)
    {
        _cable.SetEndPoint(chargingPort.transform);
        _audioSource.PlayOneShot(_ropeAttachSound);
        await ChargeThingAsync(receiver);
        OnComplete?.Invoke();
    }


    public async Task ChargeThingAsync(IUseEnergy receiver)
    {
        _audioSource.PlayOneShot(_energyTransferSound);
        while (receiver.EnergyLevel < receiver.EnergyNeededToFunction && _reserveEnergyLevel > 0)
        {
            await Task.Delay((int)(_energyTransferRate * 1000));
            receiver.PowerUp(_energyPerSecond);
            _reserveEnergyLevel -= _energyPerSecond;
            Debug.Log($"Transferring energy to {receiver}. Energy level: {receiver.EnergyLevel}");
        }
        _cable.SetEndPoint(null);
    }


}
