using KronoGeo_Maui.ModelViews;
using System;
using System.Collections.Generic;
using System.Text;

namespace KronoGeo_Maui.Applications.Outils.Views
{
    /// <summary>
    /// Template selector par défaut Map - 
    /// systeme d'affichage
    /// </summary>
    public class KronoGeoCarouselTemplateSelector : DataTemplateSelector
    {
        // On déclare nos designs templates pour afficher
        public DataTemplate? MapTemplate { get; set; } = default;
        public DataTemplate? CameraTemplate { get; set; } = default;


        // La méthode magique qui fait le choix
        protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
        {
            if (MapTemplate is null || CameraTemplate is null) return (DataTemplate)item;
            return item switch
            {
                MapViewModel => MapTemplate,
                CameraViewModel => CameraTemplate,
                _ => MapTemplate // Choix par défaut par sécurité
            };
        }
    }
}
