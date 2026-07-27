using KronoGeo_Api.Models.Model.DTO;
using System.Text.Json.Serialization.Metadata;

namespace KronoGeo_Api.Applications.ExtendMethods
{
    public static class  JsonOptionsExtends
    {
        extension ( IServiceCollection services)
        {
            /// <summary>
            /// ajoute les options de sérialisation pour la classe LocalisationDTO 
            /// & de sa classe fille LocalisationPhotoDTO
            /// pour la sérialisation au niveau du controleur
            /// </summary>
            /// <returns></returns>
            public IServiceCollection AddJsonOptionsLocalisation(  )
            {
                services.AddControllers().AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
                    //options.JsonSerializerOptions.PropertyNamingPolicy = null; // -- respect de la case pour les propirétés
                    
                    // -- plus besoin voir le model DTO voir classe mère qui implémente facilement
                    // -- le polymorphisme
                    /*options.JsonSerializerOptions.TypeInfoResolver = new DefaultJsonTypeInfoResolver
                    {
                        Modifiers =
                        {
                            typeInfo =>
                            {
                                if (typeInfo.Type == typeof(LocalisationDTO))
                                {
                                    typeInfo.PolymorphismOptions = new JsonPolymorphismOptions
                                    {
                                        TypeDiscriminatorPropertyName = "TypeObjet",
                                        DerivedTypes =
                                        {
                                            new JsonDerivedType(typeof(LocalisationDTO), (int)TypeLocalisation.Base),
                                            new JsonDerivedType(typeof(LocalisationPhotoDTO), (int)TypeLocalisation.Photo)
                                        }
                                    };
                                }
                            }
                        }
                    };*/
                });
                

                return services;
            }
        }
    }
}
