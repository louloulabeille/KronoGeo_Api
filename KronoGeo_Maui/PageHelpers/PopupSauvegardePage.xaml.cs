using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Views;

namespace KronoGeo_Maui.PageHelpers;

public partial class PopupSauvegardePage : Popup<string>
{
	public PopupSauvegardePage()
	{
		InitializeComponent();
	}

	/// <summary>
	/// bouton de sortie de la fenêtre et retourne la réponse
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
    private async void Button_Clicked(object? sender, EventArgs e)
    {
		var param = (sender as Button)?.CommandParameter;
		
		if (param != null && param is string result)
		{
            await this.CloseAsync(result);
		}
    }
}