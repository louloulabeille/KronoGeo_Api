using CommunityToolkit.Mvvm.Messaging.Messages;
using System;
using System.Collections.Generic;
using System.Text;

namespace KronoGeo_Maui.Applications.Message
{
    /// <summary>
    /// Messager Toolkit pour fournir une liste de tracé gps pour la map ou autre
    /// </summary>
    /// <param name="value"></param>
    public class TraceMapMessage(List<Location>? value) : ValueChangedMessage<List<Location>?>(value)
    {
    }
}
