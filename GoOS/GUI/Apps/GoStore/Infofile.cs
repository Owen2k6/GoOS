namespace GoOS.GUI.Apps.GoStore
{
    public struct Infofile
    {
        public Infofile(string[] contents, string url)
        {
            Contents = contents;
            URL = url;
        }

        public string[] Contents;
        public string URL;
    }
}
