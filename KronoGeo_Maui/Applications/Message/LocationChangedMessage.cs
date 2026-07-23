using CommunityToolkit.Mvvm.Messaging.Messages;
using System;
using System.Collections.Generic;
using System.Text;

namespace KronoGeo_Maui.Applications.Message
{
    public class LocationChangedMessage(Location value) : ValueChangedMessage<Location>(value)
    {
    }
}
