using KronoGeo_Api.Models.Carousel;
using KronoGeo_Maui.ModelViews;
using System;
using System.Collections.Generic;
using System.Text;

namespace KronoGeo_Maui.Applications.Outils.Views
{
    /// <summary>
    /// Template selector par défaut Map
    /// pour le carousel
    /// </summary>
    public class KronoGeoCarouselTemplateSelector : DataTemplateSelector
    {
        // On déclare nos designs templates pour les afficher
        public DataTemplate? MapTemplate { get; set; } = default;
        public DataTemplate? CameraTemplate { get; set; } = default;
        public DataTemplate? ResumeTemplate { get; set; } = default;

        // La méthode magique qui fait le choix
        protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
        {
            if (MapTemplate is null || CameraTemplate is null 
                || ResumeTemplate is null ) return (DataTemplate)item;
            return item switch
            {
                MapViewModel => MapTemplate,
                CameraViewModel => CameraTemplate,
                ResumeViewModel => ResumeTemplate,
                _ => MapTemplate // Choix par défaut par sécurité
            };
        }
    }
}
