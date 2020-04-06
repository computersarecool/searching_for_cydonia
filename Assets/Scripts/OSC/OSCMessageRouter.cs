using extOSC;
using UnityEngine;

namespace OSC_routing
{
    public class OSCMessageRouter : MonoBehaviour
    {
        public OSCReceiver receiver;

        public void Start()
        {
            // From Live
            receiver.Bind(Constants.LiveEqBandAddress, UpdateBand);
            receiver.Bind(Constants.LiveTempoAddress, UpdateTempo);
            receiver.Bind(Constants.LiveClipNameAddress, UpdateClipName);
            receiver.Bind(Constants.ClipColorsAddress, UpdateClipColor);
            receiver.Bind(Constants.LivePlayingClipPositionAddress, UpdateTrackPosition);
            receiver.Bind(Constants.PlayingClipLengthAddress, UpdateClipLength);
            receiver.Bind(Constants.PlayingSlotIndexAddress, UpdatePlayingSlotIndex);

            // From Unity
            receiver.Bind(Constants.CameraRotateAddress, RotateMainCamera);
            receiver.Bind(Constants.CameraMoveAddress, MoveMainCamera);
            receiver.Bind(Constants.GuiHueAddress, UpdateGuiHue);
            
            // From others
            receiver.Bind(Constants.TimeAddress, UpdateTime);
        }

        private static void UpdateBand(OSCMessage message)
        {
            if (!message.ToFloat(out var val)) return;
            
            const int trackAddressIndex = 3;
            var addressArray = message.Address.Split('/');
            var bandIndex = int.Parse(addressArray[trackAddressIndex]);
            SettingsSingleton.Instance.visualizerController.UpdateBand(bandIndex, val);
        }
        
        private static void UpdateTempo(OSCMessage message)
        {
            if (message.ToFloat(out var val))
                SettingsSingleton.Instance.LiveSetTempo = val;
        }
        
        private static void UpdateClipName(OSCMessage message)
        {
            if (!message.ToString(out var data)) return;
            
            const int clipAddressIndex = 5;
            var addressArray = message.Address.Split('/');
            var clipIndex = int.Parse(addressArray[clipAddressIndex]);
            SettingsSingleton.Instance.UpdateClipName(clipIndex, data);
        }
        
        private static void UpdateClipColor(OSCMessage message)
        {
            
            if (!message.ToInt(out var color)) return;

            const int clipAddressIndex = 5;
            var addressArray = message.Address.Split('/');
            var clipIndex = int.Parse(addressArray[clipAddressIndex]);
            SettingsSingleton.Instance.UpdateClipColor(clipIndex, color);
        }
        
        private static void UpdateTrackPosition(OSCMessage message)
        {
            if (!message.ToFloat(out var position)) return;
            
            const int trackAddressIndex = 3;
            var addressArray = message.Address.Split('/');
            var trackIndex = int.Parse(addressArray[trackAddressIndex]);
            SettingsSingleton.Instance.UpdatePlayingClipPosition(trackIndex, position);

        }

        private static void UpdateClipLength(OSCMessage message)
        {
            if (!message.ToFloat(out var length)) return;
            
            const int trackAddressIndex = 3;
            var addressArray = message.Address.Split('/');
            var trackIndex = int.Parse(addressArray[trackAddressIndex]);
            SettingsSingleton.Instance.UpdateClipLength(trackIndex, length);
        }
        
        private static void UpdatePlayingSlotIndex(OSCMessage message)
        {
            if (!message.ToString(out var name)) return;

            const int trackAddressIndex = 3;
            var addressArray = message.Address.Split('/');
            var trackIndex = int.Parse(addressArray[trackAddressIndex]);
            SettingsSingleton.Instance.UpdatePlayingClipName(trackIndex, name);
        }
        
        // From Unity
        private static void RotateMainCamera(OSCMessage message)
        {
            if (message.ToVector3(out var vector))
                SettingsSingleton.Instance.UpdateCameraRotation(vector);
        }
        
        private static void MoveMainCamera(OSCMessage message)
        {
            if (message.ToVector3(out var vector))
                SettingsSingleton.Instance.MoveMainCameraAndEnvironment(vector);
        }

        private static void UpdateGuiHue(OSCMessage message)
        {
            if (message.ToFloat(out var value))
                SettingsSingleton.Instance.UpdateGuiHue(value);
        }
        
        // From others
        private static void UpdateTime(OSCMessage message)
        {
            if (message.ToFloat(out var value))
                SettingsSingleton.Instance.clock = value;
        }
    }
}
