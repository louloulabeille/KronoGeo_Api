using CommunityToolkit.Mvvm.Messaging.Messages;
using Microsoft.Maui.Controls.Maps;
using System;
using System.Collections.Generic;
using System.Text;

namespace KronoGeo_Maui.Applications.Message
{
    public class PinMapMessage (Pin value) : ValueChangedMessage<Pin>(value)
    {
    }
}
