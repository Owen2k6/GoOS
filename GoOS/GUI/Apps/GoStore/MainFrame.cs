using Gold.Graphics;
using GoOS.Networking; // HttpHelper
using System;
using System.Collections.Generic;
using static GoOS.Resources;

namespace GoOS.GUI.Apps.GoStore
{
    public class MainFrame : Window
    {
        // ------------------ UI + data fields ------------------
        Button[] catagoryButtons;
        Button[] _repoFilesButtons;
        string[] Catagories = { "ERROR" }; // Hardcoded fallback
        public static string Version = "1.0";

        private int catagory = 0;
        private int page = 0;

        private List<Application> _repoFiles;

        private Button nextButton;
        private Button prevousButton;

        public static readonly List<string> AllowDLFrom = new List<string>
        {
            "1.5"
        };

        private Canvas _infoBoard;
        private string _infoBoardText = "Welcome to GoStore! Please wait while we load application data...";
        private int _infoTextWidthPx = -1; // cached pixel width for the scrolling marquee
        private int textX;

        // Hard-coded example data to prevent freezing if server is unavailable
        private string[] sampleRepos = { "api.goos.owen2k6.com" };
        private string sampleInfoFile = "GoOS Test|Test application for GoOS|1.0|Owen2k6|Utilities|1.5|test|goexe";

        // ------------------ Regional/server block state ------------------
        private bool _regionBlocked = false;
        private string _regionBlockReason = "Access to Owen2k6 Network is unavailable in your country.";
        private const string RegionBlockTitle = "Service unavailable in your country";

        // If true, we abort opening entirely (no paint, no run)
        private bool _abortOpen = false;

        // Defer closing so the Dialogue can render at least a frame (only used if construction proceeds)
        private bool _deferClose = false;
        private int _framesUntilClose = 0;
        private void RequestCloseAfterDialogue(int frames = 2)
        {
            if (frames < 1) frames = 1;
            _deferClose = true;
            _framesUntilClose = frames;
        }

        // ============================================================
        // Optional static pre-open probe (kept for convenience)
        // ============================================================
        public static bool CanOpenStore(out string reason)
        {
            reason = "";
            try
            {
                HttpHelper.SimpleHttpGet("api.goos.owen2k6.com", "/GoOS/cat.gostore");
                return true;
            }
            catch (HttpHelper.RegionBlockedException ex)
            {
                reason = ex.Message;
                return false;
            }
            catch
            {
                reason = "GoStore services are not available at the moment.";
                return false;
            }
        }

        // ============================================================
        // Constructor
        // ============================================================
        public MainFrame()
        {
            // ---------- HARD GATE: do not open if blocked/unavailable ----------
            try
            {
                // Small, cheap probe that will 401/403/451 for region blocks,
                // or throw for server unavailability.
                HttpHelper.SimpleHttpGet("api.goos.owen2k6.com", "/GoOS/cat.gostore");
            }
            catch (HttpHelper.RegionBlockedException ex)
            {
                _abortOpen = true;
                _regionBlocked = true;
                _regionBlockReason = string.IsNullOrWhiteSpace(ex.Message)
                    ? "Service unavailable in your jurisdiction."
                    : ex.Message;

                // Inform user; do not bring up the GoStore window at all
                Dialogue.Show("Error - GoStore", _regionBlockReason, default, WindowManager.errorIcon);

                // Ensure WindowManager culls us before any draw
                Visible = false;
                Closing = true;
                return;
            }
            catch
            {
                _abortOpen = true;

                Dialogue.Show("Error - GoStore",
                    "GoStore services are not available at the moment.",
                    default,
                    WindowManager.errorIcon);

                Visible = false;
                Closing = true;
                return;
            }
            // ---------- END HARD GATE ----------

            try
            {
                // Create the window only after we know it is allowed
                Contents = new Canvas(800, 600);
                Title = "GoStore";
                Visible = true;
                Closable = true;
                SetDock(WindowDock.Center);

                // Initialise basic objects
                _repoFiles = new List<Application>();
                _infoBoard = new Canvas(424, 16);
                textX = 424;

                // prime the cached width for the default message
                SetInfoBoardText(_infoBoardText);

                // Try to load data
                LoadData();

                InitialiseButtons();
                Render(Catagories[0]);
            }
            catch (Exception)
            {
                Dialogue.Show("Error - GoStore", "Failed to connect to GoOS Services", default, WindowManager.errorIcon);
                // Keep the window invisible and flag for close to avoid a blank shell
                Visible = false;
                Closing = true;
            }
        }

        // Prevent any painting if we aborted open (WindowManager calls Paint() right after AddWindow)
        public override void Paint()
        {
            if (_abortOpen) return;
            base.Paint();
        }

        // ============================================================
        // Info board text
        // ============================================================
        private void SetInfoBoardText(string text)
        {
            _infoBoardText = text ?? string.Empty;
            _infoTextWidthPx = Font_1x.MeasureString(_infoBoardText);
            if (textX < -_infoTextWidthPx) textX = 424;
        }

        // ============================================================
        // Data loading with runtime block handling (belt and braces)
        // ============================================================
        private void LoadData()
        {
            bool anyDataLoaded = false;

            try
            {
                // Categories
                try
                {
                    string[] serverCategories = GetCatagoriesFile();
                    if (serverCategories != null && serverCategories.Length > 0)
                    {
                        Catagories = serverCategories;
                        anyDataLoaded = true;
                    }
                }
                catch (HttpHelper.RegionBlockedException)
                {
                    throw;
                }
                catch
                {
                    // Use default categories
                }

                // Repos
                string[] repos;
                try
                {
                    repos = GetReposFile();
                    if (repos == null || repos.Length == 0) repos = sampleRepos;
                }
                catch (HttpHelper.RegionBlockedException)
                {
                    throw;
                }
                catch
                {
                    repos = sampleRepos;
                }

                // UI arrays
                catagoryButtons = new Button[Catagories.Length];
                _repoFilesButtons = new Button[50];

                // Info board
                try
                {
                    string infoText = GetInfoBoardFile();
                    if (!string.IsNullOrEmpty(infoText))
                        SetInfoBoardText(infoText);
                    else
                        SetInfoBoardText(_infoBoardText);
                }
                catch (HttpHelper.RegionBlockedException)
                {
                    throw;
                }
                catch
                {
                    SetInfoBoardText("Welcome to GoStore! Some features may not be available due to connection issues.");
                }

                // Apps
                foreach (string repo in repos)
                {
                    if (string.IsNullOrEmpty(repo)) continue;

                    try
                    {
                        string infoFileContent = GetInfoFile(repo);
                        if (string.IsNullOrEmpty(infoFileContent)) continue;

                        Infofile infoFile = new Infofile(infoFileContent.Split('\n'), repo);

                        foreach (string programme in infoFile.Contents)
                        {
                            if (string.IsNullOrEmpty(programme)) continue;

                            string[] appData = programme.Split('|');
                            if (appData.Length < 5) continue;

                            Application app = new Application(appData, infoFile.URL);
                            app.Downloadable = AllowDLFrom.Contains(app.GoOSVersion);
                            _repoFiles.Add(app);
                            anyDataLoaded = true;
                        }
                    }
                    catch (HttpHelper.RegionBlockedException)
                    {
                        throw;
                    }
                    catch
                    {
                        // next repo
                    }
                }

                if (_repoFiles.Count == 0)
                {
                    Application sampleApp = new Application(sampleInfoFile.Split('|'), "api.goos.owen2k6.com");
                    sampleApp.Downloadable = true;
                    _repoFiles.Add(sampleApp);

                    if (!anyDataLoaded)
                        Dialogue.Show("GoStore", "Failed to load application data from server", default, WindowManager.errorIcon);
                }
            }
            catch (HttpHelper.RegionBlockedException ex)
            {
                _regionBlocked = true;
                _regionBlockReason = string.IsNullOrWhiteSpace(ex.Message)
                    ? "Service unavailable in your jurisdiction."
                    : ex.Message;

                Dialogue.Show("GoStore", _regionBlockReason, default, WindowManager.errorIcon);

                // Close shortly after to ensure the dialogue paints
                RequestCloseAfterDialogue(2);
            }
        }

        // ============================================================
        // Buttons and rendering
        // ============================================================
        private void InitialiseButtons()
        {
            for (int i = 0; i < Catagories.Length; i++)
            {
                string label = Catagories[i].Trim();
                catagoryButtons[i] = new Button(this,
                    Convert.ToUInt16(5),
                    Convert.ToUInt16(45 + i * 20),
                    Convert.ToUInt16(label.Length * 8),
                    20,
                    label)
                {
                    Name = label,
                    UseSystemStyle = false,
                    BackgroundColour = new Color(0, 0, 0, 0),
                    ClickedAlt = CategoryButtonClick,
                    RenderWithAlpha = true
                };
            }

            nextButton = new Button(this, 685, 556, 109, 35, "Next")
            {
                Name = "next",
                UseSystemStyle = false,
                BackgroundColour = new Color(0, 0, 0, 0),
                Clicked = nextPage,
                RenderWithAlpha = true
            };

            prevousButton = new Button(this, 514, 556, 109, 35, "Previous")
            {
                Name = "Previous",
                UseSystemStyle = false,
                BackgroundColour = new Color(0, 0, 0, 0),
                Clicked = previousPage,
                RenderWithAlpha = true
            };
        }

        private void CategoryButtonClick(string cat)
        {
            page = 0;
            Render(cat);
        }

        private int GetCatagoriesIndex(string cat)
        {
            for (int i = 0; i < Catagories.Length; i++)
            {
                if (Catagories[i].Trim() == cat)
                    return i;
            }
            return 0; // Return first category instead of -1
        }

        // Keep old name for compatibility with existing calls
        private int GetCatagoryIndex(string cat) => GetCatagoriesIndex(cat);

        public override void HandleRun()
        {
            if (_abortOpen) return;

            base.HandleRun();

            _infoBoard.DrawImage(0, 0, GoStoreinfoboard, false);
            _infoBoard.DrawString(textX, 0, _infoBoardText, Font_1x, Color.Yellow);
            Contents.DrawImage(359, 9, _infoBoard, false);

            int width = (_infoTextWidthPx >= 0) ? _infoTextWidthPx : Font_1x.MeasureString(_infoBoardText);
            if (textX < -width) textX = 424;
            textX--;

            // Deferred close after showing dialogue
            if (_deferClose)
            {
                _framesUntilClose--;
                if (_framesUntilClose <= 0)
                {
                    Closing = true; // WindowManager will remove us cleanly
                    _deferClose = false;
                }
            }
        }

        private void Render(string category)
        {
            if (_abortOpen) return;

            if (_regionBlocked)
            {
                Contents.Clear();
                Contents.DrawImage(0, 0, Resources.GoStore, false);

                string line1 = RegionBlockTitle;
                string line2 = _regionBlockReason;
                string line3 = "If you believe this is in error, contact staff on Discord or email help@owen2k6.com.";

                int cx = 150, cy = 120;
                Contents.DrawString(cx, cy, line1, Font_1x, Color.Red);
                Contents.DrawString(cx, cy + 18, line2, Font_1x, Color.White);
                Contents.DrawString(cx, cy + 36, line3, Font_1x, Color.Yellow);

                RenderSystemStyleBorder();
                return;
            }

            catagory = GetCatagoryIndex(category);

            if (_repoFilesButtons != null)
            {
                foreach (Button b in _repoFilesButtons)
                {
                    if (b != null) Controls.Remove(b);
                }
                for (int i = 0; i < _repoFilesButtons.Length; i++)
                    _repoFilesButtons[i] = null;
            }

            Contents.Clear();
            Contents.DrawImage(0, 0, Resources.GoStore, false);

            int Line = 0;
            int Colum = 0;
            int buttonCount = 0;

            for (int i = page * 18; i < Math.Min(_repoFiles.Count, page * 18 + 18); i++)
            {
                if (i >= _repoFiles.Count) break;

                int appCat = GetCatagoryIndex(_repoFiles[i].Category);

                if (Line >= 6 && appCat == catagory)
                {
                    Line = 0;
                    Colum++;
                }

                if (appCat == catagory)
                {
                    string appName = _repoFiles[i].Name;
                    string goosVer = _repoFiles[i].GoOSVersion.TrimEnd();
                    int padCount = 25 - appName.Length - (goosVer.Length + 6);
                    if (padCount < 0) padCount = 0;
                    string VersionSpaces = new string(' ', padCount);

                    int x = 150 + Colum * (207 + 5);
                    int y = 45 + (Line) * (78 + 5);

                    _repoFilesButtons[buttonCount] = new Button(this,
                        Convert.ToUInt16(x),
                        Convert.ToUInt16(y),
                        207, 78,
                        appName + VersionSpaces + "GoOS " + goosVer + "+" +
                        "\nBy " + _repoFiles[i].Author + "\n" +
                        _repoFiles[i].Version.Replace(@"\n", "\n"))
                    {
                        Name = appName,
                        UseSystemStyle = false,
                        BackgroundColour = new Color(0, 0, 0, 0),
                        ClickedAlt = _repoFiles_Click,
                        RenderWithAlpha = true,
                        CenterTitle = false,
                        textX = 5,
                        textY = 2
                    };

                    buttonCount++;
                    Line++;

                    Contents.DrawImage(x, y, StoreButton);
                    _repoFilesButtons[buttonCount - 1].Render();
                }
            }

            int p1 = page + 1;
            int pagex = 650;
            int digits = (p1 < 10) ? 1 : (p1 < 100 ? 2 : 3);
            if (digits == 2) pagex -= 4;
            else if (digits == 3) pagex -= 8;

            foreach (Button i in catagoryButtons) i.Render();
            prevousButton.Render();
            nextButton.Render();

            Contents.DrawString((144 / 2) - (Font_1x.MeasureString(category.Trim()) / 2), 563, category.Trim(), Font_1x, Color.White);
            Contents.DrawString(pagex, 563, p1.ToString(), Font_1x, Color.White);

            RenderSystemStyleBorder();
        }

        private int GetIndexByTitle(string title)
        {
            for (int i = 0; i < _repoFiles.Count; i++)
                if (_repoFiles[i].Name == title) return i;
            return 0;
        }

        private void _repoFiles_Click(string i)
        {
            if (_regionBlocked || _abortOpen) return;
            WindowManager.AddWindow(new DescriptionFrame(_repoFiles[GetIndexByTitle(i)]));
        }

        private void nextPage()
        {
            if (_regionBlocked || _abortOpen) return;

            int itemsInCategory = 0;
            foreach (Application app in _repoFiles)
            {
                if (GetCatagoryIndex(app.Category) == catagory)
                    itemsInCategory++;
            }

            int maxPages = (itemsInCategory + 17) / 18;

            if (page < maxPages - 1)
            {
                page++;
                Render(Catagories[catagory]);
            }
        }

        private void previousPage()
        {
            if (_regionBlocked || _abortOpen) return;

            if (page > 0)
            {
                page--;
                Render(Catagories[catagory]);
            }
        }

        // ============================================================
        // Network wrappers (via HttpHelper)
        // ============================================================
        private string GetInfoFile(string repo)
        {
            try
            {
                return HttpHelper.SimpleHttpGet(repo, "/info.glist");
            }
            catch (HttpHelper.RegionBlockedException) { throw; }
            catch { return ""; }
        }

        private string GetInfoBoardFile()
        {
            try
            {
                return HttpHelper.SimpleHttpGet("api.goos.owen2k6.com", "/GoOS/" + Kernel.edition + "-status.gostore");
            }
            catch (HttpHelper.RegionBlockedException) { throw; }
            catch { return ""; }
        }

        private string[] GetReposFile()
        {
            try
            {
                string result = HttpHelper.SimpleHttpGet("api.goos.owen2k6.com", "/GoOS/repos.gostore");
                if (string.IsNullOrEmpty(result)) return new string[0];
                return result.Split('\n');
            }
            catch (HttpHelper.RegionBlockedException) { throw; }
            catch { return new string[0]; }
        }

        private string[] GetCatagoriesFile()
        {
            try
            {
                string result = HttpHelper.SimpleHttpGet("api.goos.owen2k6.com", "/GoOS/cat.gostore");
                if (string.IsNullOrEmpty(result)) return new string[0];
                return result.Split('\n');
            }
            catch (HttpHelper.RegionBlockedException) { throw; }
            catch { return new string[0]; }
        }
    }
}
