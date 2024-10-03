using MoreMountains.FeedbacksForThirdParty;
using MoreMountains.NiceVibrations;
using UnityEngine;

public class HapticFeedBack : MonoBehaviour
{
    public static void TriggerHaptic()
    {
        MMVibrationManager.Haptic(HapticTypes.LightImpact); 
    }
}