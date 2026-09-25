using KronoGeo_Api.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace KronoGeo_Api.Interface.Service
{
    public interface IMapStateService : IDisposable
    {
        #region public properties
        public IEnumerable<Localisation>? CurrentLocalisations { get; set; }
        public event Action? OnOpenRequested;
        #endregion

        #region public methods
        public void OpenMapwithLocalisations(IEnumerable<Localisation>? localisations);
        #endregion 

    }
}
