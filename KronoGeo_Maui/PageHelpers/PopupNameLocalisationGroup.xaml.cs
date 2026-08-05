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

            var toast = Toast.Make(text, duration, fontSize);

			NameGroupLocalisation.Focus();
            await toast.Show(cancellationTokenSource.Token);
        }
        else
        {
            await this.CloseAsync(NameGroupLocalisation.Text);
        }
        
	}
    #endregion
}