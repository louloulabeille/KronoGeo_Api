using CommunityToolkit.Mvvm.Messaging.Messages;
using System;
using System.Collections.Generic;
using System.Text;

namespace KronoGeo_Maui.Applications.Message
{
    public class PolyneMapMessage(Location? value) : ValueChangedMessage<Location?>(value) 
    {
    }
}
