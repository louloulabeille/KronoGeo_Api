using KronoGeo_Api.Models.Model.DTO;
using System;
using System.Collections.Generic;
using System.Text;

namespace KronoGeo_Api.Models.ModelEventArgs
{
    public class PhotoEventArgs(PhotoDTO photoDTO)  : EventArgs
    {
        #region private readonly properties
        private readonly PhotoDTO _photoDTO = photoDTO;
        #endregion

        #region public get properties
        public PhotoDTO PhotoDTO { get { return _photoDTO; } }
        #endregion

    }
}
