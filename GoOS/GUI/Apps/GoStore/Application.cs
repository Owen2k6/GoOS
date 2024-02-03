namespace GoOS.GUI.Apps.GoStore
{
    public struct Application
    {
        public Application(string[] metadata, string repo)
        {
            Name = metadata[0];
            Description = metadata[1];
            Version = metadata[2];
            Author = metadata[3];
            Category = metadata[4];
            GoOSVersion = metadata[5];
            Filename = metadata[6];
            Repository = repo;
        }

        public override string ToString()
        {
            return Name + "|" + Description + "|" + Version + "|" + Author + "|" + Category + "|" + GoOSVersion + "|" + Filename + "|" + Downloadable.ToString();
        }

        public string Name;
        public string Description;
        public string Version;
        public string Author;
        public string Category;
        public string GoOSVersion;
        public string Filename;
        public string Repository;
        public bool Downloadable = true;
    }
}
