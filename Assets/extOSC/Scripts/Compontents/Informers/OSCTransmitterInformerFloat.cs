/* Copyright (c) 2019 ExT (V.Sigalkin) */

using extOSC.Components.Informers;
using UnityEngine;

namespace extOSC.Scripts.Compontents.Informers
{
    [AddComponentMenu("extOSC/Components/Transmitter/Float Informer")]
    public class OSCTransmitterInformerFloat : OSCTransmitterInformer<float>
    {
        #region Protected Methods

        protected override void FillMessage(OSCMessage message, float value)
        {
            message.AddValue(OSCValue.Float(value));
        }

        #endregion
    }
}