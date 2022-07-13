using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

namespace Util {
    public class XR
    {
        public static bool isPresent()
        {
            var xrDisplaySubsystems = new List<XRDisplaySubsystem>();
            SubsystemManager.GetInstances<XRDisplaySubsystem>(xrDisplaySubsystems);
            foreach (var xrDisplay in xrDisplaySubsystems)
            {
                if (xrDisplay.running)
                {
                    return true;
                }
            }
            return false;
        }

        // returns empty string if device is not present
        public static String deviceName()
        {
            return XRSettings.loadedDeviceName;
        }
    }

}