using System;

namespace $safeprojectname$.Attributes
{
    internal class ImageAttribute : Attribute
    {
        readonly string _resourceName;
        public ImageAttribute(string resourceName)
        {
            this._resourceName = resourceName;
        }

        public ImageSource GetImage()
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
