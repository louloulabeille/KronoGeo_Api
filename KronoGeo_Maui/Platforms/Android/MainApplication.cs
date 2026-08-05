using Android.App;
using Android.Content.Res;
using Android.Runtime;
using Microsoft.Maui.Handlers;

namespace KronoGeo_Maui
{
    [Application]
    public class MainApplication : MauiApplication
    {
        public MainApplication(IntPtr handle, JniHandleOwnership ownership)
            : base(handle, ownership)
        {
        }

        protected override MauiApp CreateMauiApp() 
        {
            // -- modification de Entry pour enlever sur Android la ligne de soulignement du champ de saisie
            EntryHandler.Mapper.AppendToMapping("NoUnderline", (handler, view) =>
            //EntryHandler.Mapper.AppendToMapping(nameof(Entry), (handler, view) =>
            {
                if (view is Entry)
                {
                    // Remove underline
                    handler.PlatformView.BackgroundTintList = ColorStateList.ValueOf(Android.Graphics.Color.Transparent);

                    // Change placeholder text color
                    //handler.PlatformView.SetHintTextColor(ColorStateList.ValueOf(Android.Graphics.Color.Red));
                }
            });

            return MauiProgram.CreateMauiApp();
        }
    }
}
