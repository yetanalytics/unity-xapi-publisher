using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

namespace Util {
    public class XR
    {
        public static bool isPresent()
        {
            return XRDevice.isPresent;
        }

        // returns empty string if device is not present
        public static String deviceName()
        {
            return XRSettings.loadedDeviceName;
        }
    }

}