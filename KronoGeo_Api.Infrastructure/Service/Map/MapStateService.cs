using KronoGeo_Api.Interface.Service;
using KronoGeo_Api.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace KronoGeo_Api.Infrastructure.Service.Map
{
    /// <summary>
    /// classe permet de gérer l'état de la map et de passer les localisations vers la map
    /// </summary>
    public class MapStateService : IMapStateService
    {
        public IEnumerable<Localisation>? CurrentLocalisations { get; set; }

        public event Action? OnOpenRequested;

        /// <summary>
        /// methode pour charger les localisations au niveau de la map
        /// et lance les intructions de event
        /// </summary>
        /// <param name="localisations"></param>
        public void OpenMapwithLocalisations(IEnumerable<Localisation>? localisations)
        {
            CurrentLocalisations = localisations;
            OnOpenRequested?.Invoke();
        }

        /// <summary>
        /// désabonnement
        /// </summary>
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}
