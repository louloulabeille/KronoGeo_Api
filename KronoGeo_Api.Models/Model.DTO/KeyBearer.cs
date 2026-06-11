namespace KronoGeo_Api.Applications.Model.DTO
{
    // - pour le moment les données sont dans secret 
    public class KeyBearer
    {
        // - clé de chiffrement pour la signature du token
        // (ex: une clé secrète ou une clé publique/privée)
        public string Key { get; set; } = string.Empty;
        // - Pour s'assurer que le token est destiné à notre API pou l'audiance du pays
        // - url de l'audience du token (ex: https://api.monsite.com)
        public bool ValidateAudience { get; set; } = false;
        // - Pour s'assurer que le token a été émis par une source de confiance (ex: notre serveur d'authentification)
        // - url de l'autorité d'émission du token (ex: https://auth.monsite.com)
        public bool ValidateIssuer { get; set; } = false;
        // - valider l'acteur qui est à l'origine de la demande d'authentification OAuth2.0
        public bool ValidateActor { get; set; } = false; 
        // durée de vie à paramétrer lors de la création du token envoyer vers l'user
        public bool ValidateLifetime { get; set; } = true;
    }
}
