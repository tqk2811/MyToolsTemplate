using System;
using System.Drawing;
using System.Windows.Media;
using TqkLibrary.WpfUi;

namespace $safeprojectname$.Attributes
{
    internal class ImageResourceAttribute : Attribute
    {
        readonly string _resourceName;
        public ImageResourceAttribute(string resourceName)
        {
            this._resourceName = resourceName;
        }

        public ImageSource? GetImage()
        {
            if (string.IsNullOrWhiteSpace(_resourceName)) return null;
            var obj = Resource.ResourceManager.GetObject(this._resourceName);
            if(obj is not null && obj is Bitmap bitmap)
            {
                return bitmap.ToImageSource();
            }
            return null;
        }
    }

}
