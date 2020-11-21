using extOSC;
using System.Collections.Generic;

namespace MusicControl
{
    public class Track
    {
        private string id;
        private string canonicalPath;
        private List<ClipSlot> clipSlots;
        public List<ClipSlot> ClipSlots
        {
            get => this.clipSlots;
            set
            {
                this.clipSlots = value;
                foreach (var clipSlot in this.clipSlots)
                {
                    clipSlot.GetClip();
                }
            }
        }

        public Track(string id, string canonicalPath)
        {
            this.id = id;
            this.canonicalPath = canonicalPath;
        }
    }

    public class ClipSlot
    {
        private bool hasClip;
        public Clip Clip;
        private string id;
        private readonly string canonicalPath;

        public void GetClip()
        {
            var message = new OSCMessage(this.canonicalPath);
            message.AddValue(OSCValue.String("get"));
            message.AddValue(OSCValue.String("clip"));
            SettingsSingleton.Instance.ExternalOSCTransmitter.Send(message);
        }

        public ClipSlot(string id, string canonicalPath)
        {
            this.id = id;
            this.canonicalPath = canonicalPath;
        }
    }

    public class Clip
    {
        private string id;
        private string canonicalPath;
        public string Name;
        public int Color;
        public int PitchCoarse;
        public float Length;
        private List<string> propsToGet = new List<string>{"length", "name", "color", "pitch_coarse"};

        public Clip(string id, string canonicalPath)
        {
            this.id = id;
            this.canonicalPath = canonicalPath;

            foreach (var prop in this.propsToGet)
            {
                var message = new OSCMessage(this.canonicalPath);
                message.AddValue(OSCValue.String("get"));
                message.AddValue(OSCValue.String(prop));
                SettingsSingleton.Instance.ExternalOSCTransmitter.Send(message);
            }
        }
    }
}
