using CommunityToolkit.Mvvm.Messaging.Messages;
using Microsoft.Maui.Maps;
using System;
using System.Collections.Generic;
using System.Text;

namespace KronoGeo_Maui.Applications.Message
{
    public class RecenterMapMessage(Location value) : ValueChangedMessage<Location>(value)
    {
    }
}
