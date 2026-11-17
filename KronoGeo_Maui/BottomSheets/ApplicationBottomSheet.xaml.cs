using CommunityToolkit.Maui.Behaviors;
using CommunityToolkit.Mvvm.Messaging;
using KronoGeo_Maui.Applications.Message;
using KronoGeo_Maui.ModelViews.BottomSheets;
using The49.Maui.BottomSheet;

namespace KronoGeo_Maui.BottomSheets;

public partial class ApplicationBottomSheet : BottomSheet
{
	public ApplicationBottomSheet(ApplicationBottomSheetViewModel modelView)
	{
		InitializeComponent();
		this.BindingContext = modelView;

        // -- messenger pour fermer le bottomsheet à partir du viewmodel 
        // -- directement à partir du code
        WeakReferenceMessenger.Default.Register<CloseBottomSheetMessage>(this, async (r, m) =>
            await DismissAsync());

        /*this.Behaviors.Add(new EventToCommandBehavior
        {
            EventName = nameof(Loaded),
            Command = modelView.LoadedExeCommand
        });*/
    }
}