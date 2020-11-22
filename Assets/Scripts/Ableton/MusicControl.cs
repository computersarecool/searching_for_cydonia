using extOSC;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MusicControl
{
    public class Set
    {
        private const int delayForNetwork = 1000;
        public List<Track> tracks;
        private float tempo;
        public float Tempo
        {
            set
            {
                this.tempo = value;
                SettingsSingleton.Instance.TempoIndicator.text = value.ToString("F0");
            }
        }

        public void ChangeLiveTempo(bool increase)
        {
            // TODO: Make into overloaded function
            var newTempo = increase ? this.tempo + 1 : this.tempo - 1;
            var message = new OSCMessage("/live_set");
            message.AddValue(OSCValue.String("set"));
            message.AddValue(OSCValue.String("tempo"));
            message.AddValue(OSCValue.Float(newTempo));
            SettingsSingleton.Instance.ExternalOSCTransmitter.Send(message);
        }
        public Set()
        {
           Task.Delay(delayForNetwork).ContinueWith(_ => requestProperties());
        }

        private void requestProperties()
        {
            var propsToGet = new[] {"tempo", "tracks"};
            foreach (var prop in propsToGet)
            {
                AbletonController.GetProperty("/live_set", prop);
            }
        }
    }

    public class Track
    {
        private string id;
        private int trackIndex;
        private string canonicalPath;
        private List<ClipSlot> clipSlots;

        private int playingSlotIndex;
        public int PlayingSlotIndex
        {
            get => this.playingSlotIndex;
            set
            {
                this.playingSlotIndex = value;
                SettingsSingleton.Instance.FireButtonTexts[this.trackIndex].text = this.clipSlots[value].Clip.Name;
            }
        }

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

        public Track(string id, string canonicalPath, int trackIndex)
        {
            this.id = id;
            this.trackIndex = trackIndex;
            this.canonicalPath = canonicalPath;
            var message = new OSCMessage(this.canonicalPath);
            message.AddValue(OSCValue.String("get"));
            message.AddValue(OSCValue.String("clip_slots"));
            SettingsSingleton.Instance.ExternalOSCTransmitter.Send(message);
        }
    }

    public class ClipSlot
    {
        private bool hasClip;
        public Clip Clip;
        private string id;
        private int trackIndex;
        private readonly string canonicalPath;

        public void GetClip()
        {
            var message = new OSCMessage(this.canonicalPath);
            message.AddValue(OSCValue.String("get"));
            message.AddValue(OSCValue.String("clip"));
            SettingsSingleton.Instance.ExternalOSCTransmitter.Send(message);
        }

        public ClipSlot(string id, string canonicalPath, int trackIndex)
        {
            this.id = id;
            this.canonicalPath = canonicalPath;
        }
    }

    public class Clip
    {
        private string id;
        private string canonicalPath;
        private int trackIndex;
        private string name;

        public string Name
        {
            get => this.name;
            set
            {
                this.name = value;
            }
        }

        private int color;
        public int Color
        {
            get => this.color;
            set => this.color = value;
        }

        public int PitchCoarse;
        public float Length;

        private float playingPosition;
        public float PlayingPosition
        {
            get => this.playingPosition;
            set
            {
                this.playingPosition = value;
                SettingsSingleton.Instance.PositionIndicators[this.trackIndex].fillAmount = value / this.Length;
                SettingsSingleton.Instance.CycleOf8Indicators[this.trackIndex].fillAmount = (int)value % 8 / 7.0F;
            }
        }


        private List<string> propsToGet = new List<string>{"length", "name", "color", "pitch_coarse"};

        public Clip(string id, string canonicalPath, int trackIndex)
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
