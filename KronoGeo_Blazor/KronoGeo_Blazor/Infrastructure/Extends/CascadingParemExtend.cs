using BruTile.Wms;
using KronoGeo_Api.Models.Parameter;
using Microsoft.AspNetCore.Components;
using System.ComponentModel;
using System.Net;
using System.Runtime.CompilerServices;

namespace KronoGeo_Blazor.Infrastructure.Extends
{
    public static class CascadingParemExtend
    {
        extension(IServiceCollection services)
        {
            /// <summary>
            /// ajout du services cascading paremeter pour la connexion dans blazor et
            /// les roles attachés
            /// </summary>
            /// <returns></returns>
            public IServiceCollection AddCcParemRole()
            {
                var roleBlazor = new RoleBlazor();
                services.AddCcParemRolePropertyChanged(roleBlazor);
                return services;
            }

            /// <summary>
            /// ajout du services cascading paremeter pour la connexion dans blazor et
            /// les roles attachés en OnproperpertyChanged 
            /// va mettre à jour tous les appels de cette variable
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="state"></param>
            /// <param name="isFixed"></param>
            /// <returns></returns>
            public IServiceCollection AddCcParemRolePropertyChanged<T>(T state, bool isFixed = false)
                where T : INotifyPropertyChanged
            {
                return services.AddCascadingValue<T>(sp =>
                {
                    return new CascadingStateValueSource<T>(state, isFixed);
                });
                
            }
        }
        /// <summary>
        /// code windows .net10 
        /// https://learn.microsoft.com/fr-fr/aspnet/core/blazor/components/cascading-values-and-parameters?view=aspnetcore-10.0
        /// </summary>
        /// <typeparam name="T"></typeparam>
        private sealed class CascadingStateValueSource<T>
                : CascadingValueSource<T>, IDisposable where T : INotifyPropertyChanged
        {
            private readonly T state;
            private readonly CascadingValueSource<T> source;

            public CascadingStateValueSource(T state, bool isFixed = false)
                : base(state, isFixed = false)
            {
                this.state = state;
                source = new CascadingValueSource<T>(state, isFixed);
                this.state.PropertyChanged += HandlePropertyChanged;
            }

            private void HandlePropertyChanged(object? sender, PropertyChangedEventArgs e)
            {
                _ = NotifyChangedAsync();
            }

            public void Dispose()
            {
                state.PropertyChanged -= HandlePropertyChanged;
            }

        }

    }
}
