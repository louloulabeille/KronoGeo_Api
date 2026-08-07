using CommunityToolkit.Mvvm.Messaging.Messages;
using KronoGeo_Maui.Applications.Models;
using Microsoft.Maui.Controls.Maps;
using System;
using System.Collections.Generic;
using System.Text;

namespace KronoGeo_Maui.Applications.Message
{
    public class PinMapMessage (PinMessage? value ) : ValueChangedMessage<PinMessage?>(value)
    {
    }
}
