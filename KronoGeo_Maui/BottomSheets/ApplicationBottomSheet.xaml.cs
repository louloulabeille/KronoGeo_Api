using CommunityToolkit.Maui.Behaviors;
using KronoGeo_Maui.ModelViews.BottomSheets;
using The49.Maui.BottomSheet;

namespace KronoGeo_Maui.BottomSheets;

public partial class ApplicationBottomSheet : BottomSheet
{
	public ApplicationBottomSheet(ApplicationBottomSheetViewModel modelView)
	{
		InitializeComponent();
		this.BindingContext = modelView;

        /*this.Behaviors.Add(new EventToCommandBehavior
        {
            EventName = nameof(Loaded),
            Command = modelView.LoadedExeCommand
        });*/
    }
}