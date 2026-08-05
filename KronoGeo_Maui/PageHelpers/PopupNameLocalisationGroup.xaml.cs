using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Views;

namespace KronoGeo_Maui.PageHelpers;

public partial class PopupNameLocalisationGroup : Popup<string>
{
	public PopupNameLocalisationGroup()
	{
		InitializeComponent();
	}

    #region method public clicked
	public async void OnClickedClose(object sender, EventArgs e)
	{
		if (string.IsNullOrEmpty(NameGroupLocalisation.Text))
		{
            CancellationTokenSource cancellationTokenSource = new ();

            string text = "Vous devez saisir un nom pour le groupe de localisation.";
            ToastDuration duration = ToastDuration.Short;
            double fontSize = 14;
            // -- message d'affichage
            var toast = Toast.Make(text, duration, fontSize);
            // -- focus sur le champ de saisie
            NameGroupLocalisation.Focus();
            // -- affichage
            await toast.Show(cancellationTokenSource.Token);
        }
        else
        {
            // -- fermeture et passage du nom du groupe de localisation
            await this.CloseAsync(NameGroupLocalisation.Text);
        }
        
	}
    #endregion
}