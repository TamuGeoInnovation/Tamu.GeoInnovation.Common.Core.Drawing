namespace USC.GISResearchLab.Common.GeoreferencedImages
{
    public class GeoreferencedWebImage : GeoreferencedImage
    {
        #region Properties
        private string _URL;

        public string URL
        {
            get { return _URL; }
            set { _URL = value; }
        }
        #endregion
    }
}