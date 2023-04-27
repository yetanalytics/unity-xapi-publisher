using System.Text.Json.Serialization;
using XAPI.Metadata;

namespace XAPI {
    public class Extension {
        [JsonPropertyName("https://docs.unity3d.com/ScriptReference/XR.XRDisplaySubsystem.html")]
        public VRSubsystems vrSubsystemMetadata { set; get; }
        [JsonPropertyName("https://docs.unity3d.com/ScriptReference/XR.XRSettings.html")]
        public VRSettings vrSettingsMetadata { set; get; }
        [JsonPropertyName("https://docs.unity3d.com/ScriptReference/Application-platform.html")]
        public PlatformSettings platformSettingsMetadata { set; get; }
        [JsonPropertyName("http://ip-api.com/location")]
        public Location location { set; get; }
    }
}