using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace KronoGeo_Api.Models.Parameter
{
    public class RoleBlazor : INotifyPropertyChanged
    {
        #region public properties
        public bool IsAuthenticate { 
            get => _isAuthenticate; 
            set { 
                if( _isAuthenticate != value)
                {
                    _isAuthenticate = value;
                    OnPropertyChanged();
                }
            } 
        }
        public bool IsAdmin { 
            get => _isAdmin;
            set { 
                if (_isAdmin != value)
                {
                    _isAdmin = value;
                    OnPropertyChanged();
                }
            } 
        }
        public bool IsSuperAdmin { 
            get => _isSuperAdmin;
            set { 
                if( _isSuperAdmin!= value)
                {
                    _isSuperAdmin = value;
                    OnPropertyChanged();
                }
            }
        }
        #endregion

        #region private properties
        private bool _isSuperAdmin = false;
        private bool _isAdmin = false;
        private bool _isAuthenticate = false;
        #endregion

        #region event public interface INotifyPropertyChanged
        public event PropertyChangedEventHandler? PropertyChanged;
        #endregion

        #region protected method OnPropertyChanged
        protected virtual void OnPropertyChanged(
            [CallerMemberName] string? propertyName = default)
            => PropertyChanged?.Invoke(this, new(propertyName));
        #endregion
    }
}
