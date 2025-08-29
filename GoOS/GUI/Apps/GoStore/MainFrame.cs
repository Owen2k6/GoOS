// GoStore MainFrame – OS 9 styled, Charcoal everywhere, live ticker, system-style buttons,
// white content background, precise right-aligned GoOS tag on cards, blue header accent.

using System;
using System.Collections.Generic;
using Cosmos.System;
using GoGL.Graphics;
using GoGL.Graphics.Fonts;
using GoOS.Networking;
using GoOS.GUI.Models;
using static GoOS.Resources;

namespace GoOS.GUI.Apps.GoStore
{
    public class MainFrame : Window
    {
        private const string RegionBlockTitle = "Service unavailable in your country";
        public static string Version = "1.0";

        public static readonly List<string> AllowDLFrom = new() { "1.5" };

        // ----- Layout (content space) -----
        private const int HEADER_H = 33;
        private const int SIDEBAR_W = 136;
        private const int FOOTER_H = 23;

        // Ticker nominal placement (we clamp width each frame so it's 7px short of the right edge)
        private const int TICKER_X = 359;
        private const int TICKER_Y = 9;
        private const int TICKER_H = 19;
        private const int TICKER_W_BASE = 444; // upper bound before clamping

        // Colours (OS9 Platinum-ish + a hint of blue)
        private static readonly Color HeaderFill = new Color(0xFFDADADA);
        private static readonly Color SidebarFill = new Color(0xFFE7E7E7);
        private static readonly Color FooterFill = new Color(0xFFDADADA);
        private static readonly Color ContentWhite = Color.White;
        private static readonly Color DividerDark = new Color(0xFFB3B3B3);
        private static readonly Color DividerLite = Color.White;
        private static readonly Color HeaderBlueAcc = new Color(0xFF7DA2D9); // gentle blue accent
        private static readonly Color TickerBack = Color.Black;
        private static readonly Color TextBlack = Color.Black;
        private static readonly Color TextWhite = Color.White;
        private static readonly Color TextYellow = Color.Yellow;

        // Abort-open gate
        private readonly bool _abortOpen;

        // Marquee buffer (transparent)
        private readonly Canvas _infoBoard;

        private readonly List<Application> _repoFiles;

        private readonly string sampleInfoFile =
            "GoOS Test|Test application for GoOS|1.0|Owen2k6|Utilities|1.5|test|goexe";

        private readonly string[] sampleRepos = { "api.goos.owen2k6.com" };

        private bool _deferClose;
        private int _framesUntilClose;
        private string _infoBoardText = "Welcome to GoStore! Please wait while we load application data...";
        private int _infoTextWidthPx = -1;

        // Regional/server block state
        private bool _regionBlocked;
        private string _regionBlockReason = "Access to Owen2k6 Network is unavailable in your country.";
        private Button[] _repoFilesButtons;
        private string[] Catagories = { "ERROR" };
        private int catagory;

        // UI
        private Button[] catagoryButtons;
        private Button nextButton;
        private Button prevousButton;
        private int page;
        private int textX;

        // ============================================================
        // Constructor
        // ============================================================
        public MainFrame()
        {
            // ---------- HARD GATE ----------
            try
            {
                HttpHelper.SimpleHttpGet("api.goos.owen2k6.com", "/GoOS/cat.gostore");
            }
            catch (HttpHelper.RegionBlockedException ex)
            {
                _abortOpen = true;
                _regionBlocked = true;
                _regionBlockReason = string.IsNullOrWhiteSpace(ex.Message)
                    ? "Service unavailable in your jurisdiction."
                    : ex.Message;

                Dialogue.Show("Error - GoStore", _regionBlocked ? _regionBlockReason : "Service unavailable.", default, WindowManager.errorIcon);
                Visible = false;
                Closing = true;
                return;
            }
            catch
            {
                _abortOpen = true;
                Dialogue.Show("Error - GoStore", "GoStore services are not available at the moment.", default, WindowManager.errorIcon);
                Visible = false;
                Closing = true;
                return;
            }
            // ---------- END HARD GATE ----------

            try
            {
                Contents = new Canvas(800, 600);
                try { Contents.Clear(new Color(0x00000000)); } catch { }

                Title = "GoStore";
                Visible = true;
                Closable = true;
                SetDock(WindowDock.Center);

                _repoFiles = new List<Application>();
                _infoBoard = new Canvas(424, 16);
                try { _infoBoard.Clear(new Color(0x00000000)); } catch { }
                textX = 424;

                SetInfoBoardText(_infoBoardText);
                LoadData();
                InitialiseButtons();
                Render(Catagories[0]);
            }
            catch (Exception)
            {
                Dialogue.Show("Error - GoStore", "Failed to connect to GoOS Services", default, WindowManager.errorIcon);
                Visible = false;
                Closing = true;
            }
        }

        private void RequestCloseAfterDialogue(int frames = 2)
        {
            if (frames < 1) frames = 1;
            _deferClose = true;
            _framesUntilClose = frames;
        }

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
            var font = Resources.Charcoal ?? Font_1x;
            _infoTextWidthPx = font.MeasureString(_infoBoardText);
            if (textX < -_infoTextWidthPx) textX = 424;
        }

        // ============================================================
        // Data
        // ============================================================
        private void LoadData()
        {
            var anyDataLoaded = false;

            try
            {
                // Categories
                try
                {
                    var serverCategories = GetCatagoriesFile();
                    if (serverCategories != null && serverCategories.Length > 0)
                    {
                        Catagories = serverCategories;
                        anyDataLoaded = true;
                    }
                }
                catch (HttpHelper.RegionBlockedException) { throw; }
                catch { }

                // Repos
                string[] repos;
                try
                {
                    repos = GetReposFile();
                    if (repos == null || repos.Length == 0) repos = sampleRepos;
                }
                catch (HttpHelper.RegionBlockedException) { throw; }
                catch { repos = sampleRepos; }

                // UI arrays
                catagoryButtons = new Button[Catagories.Length];
                _repoFilesButtons = new Button[50];

                // Info text
                try
                {
                    var infoText = GetInfoBoardFile();
                    if (!string.IsNullOrEmpty(infoText))
                        SetInfoBoardText(infoText);
                    else
                        SetInfoBoardText(_infoBoardText);
                }
                catch (HttpHelper.RegionBlockedException) { throw; }
                catch
                {
                    SetInfoBoardText("Welcome to GoStore! Some features may not be available due to connection issues.");
                }

                // Apps
                foreach (var repo in repos)
                {
                    if (string.IsNullOrEmpty(repo)) continue;

                    try
                    {
                        var infoFileContent = GetInfoFile(repo);
                        if (string.IsNullOrEmpty(infoFileContent)) continue;

                        var infoFile = new Infofile(infoFileContent.Split('\n'), repo);

                        foreach (var programme in infoFile.Contents)
                        {
                            if (string.IsNullOrEmpty(programme)) continue;

                            var appData = programme.Split('|');
                            if (appData.Length < 5) continue;

                            var app = new Application(appData, infoFile.URL);
                            app.Downloadable = AllowDLFrom.Contains(app.GoOSVersion);
                            _repoFiles.Add(app);
                            anyDataLoaded = true;
                        }
                    }
                    catch (HttpHelper.RegionBlockedException) { throw; }
                    catch { }
                }

                if (_repoFiles.Count == 0)
                {
                    var sampleApp = new Application(sampleInfoFile.Split('|'), "api.goos.owen2k6.com");
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

                Dialogue.Show("GoStore", _regionBlocked ? _regionBlockReason : "Service unavailable.", default, WindowManager.errorIcon);
                RequestCloseAfterDialogue();
            }
        }

        // ============================================================
        // Buttons & layout helpers
        // ============================================================
        private void InitialiseButtons()
        {
            var font = Resources.Charcoal ?? Font_1x;

            // Category buttons in the sidebar, below the header
            for (var i = 0; i < Catagories.Length; i++)
            {
                var label = Catagories[i].Trim();
                var w = (ushort)(font.MeasureString(label) + 8);

                catagoryButtons[i] = new Button(this,
                    Convert.ToUInt16(5),
                    Convert.ToUInt16(HEADER_H + 12 + i * 20),
                    w, 20, label)
                {
                    Name = label,
                    UseSystemStyle = true,        // OS9 bevel
                    RenderWithAlpha = true,
                    CenterTitle = true,
                    ClickedAlt = CategoryButtonClick
                };
            }

            // Footer buttons (right side), sized for 23px footer
            int btnH = 20, btnW = 95;
            int footerTop = Contents.Height - FOOTER_H;
            int btnY = footerTop + (FOOTER_H - btnH) / 2;

            nextButton = new Button(this, (ushort)(Contents.Width - 10 - btnW), (ushort)btnY, (ushort)btnW, (ushort)btnH, "Next")
            {
                Name = "next",
                UseSystemStyle = true,
                RenderWithAlpha = true,
                Clicked = nextPage
            };

            prevousButton = new Button(this, (ushort)(Contents.Width - 20 - (btnW * 2)), (ushort)btnY, (ushort)btnW, (ushort)btnH, "Previous")
            {
                Name = "Previous",
                UseSystemStyle = true,
                RenderWithAlpha = true,
                Clicked = previousPage
            };
        }

        private void CategoryButtonClick(string cat)
        {
            page = 0;
            Render(cat);
        }

        private int GetCatagoriesIndex(string cat)
        {
            for (var i = 0; i < Catagories.Length; i++)
                if (Catagories[i].Trim() == cat) return i;
            return 0;
        }

        private int GetCatagoryIndex(string cat) => GetCatagoriesIndex(cat);

        // ============================================================
        // Frame-by-frame updates (ticker + header repaint ONLY)
        // ============================================================
        public override void HandleRun()
        {
            if (_abortOpen) return;

            base.HandleRun();

            var font = Resources.Charcoal ?? Font_1x;

            // 1) advance & draw marquee in its tiny buffer
            try { _infoBoard.Clear(new Color(0x00000000)); } catch { }
            _infoBoard.DrawString(textX, 0, _infoBoardText, font, TextYellow);
            var width = _infoTextWidthPx >= 0 ? _infoTextWidthPx : font.MeasureString(_infoBoardText);
            if (textX < -width) textX = 424;
            textX--;

            // 2) repaint header/title each frame so it never gets stale
            Contents.DrawFilledRectangle(0, 0, (ushort)Contents.Width, HEADER_H, 0, HeaderFill);
            // blue accent just above the grey seam
            Contents.DrawLine(0, HEADER_H - 2, Contents.Width, HEADER_H - 2, HeaderBlueAcc);
            // grey seam at bottom of header
            Contents.DrawLine(0, HEADER_H - 1, Contents.Width, HEADER_H - 1, DividerDark);

            int headTextY = (HEADER_H - font.Size) / 2;
            Contents.DrawString(8, headTextY, "Welcome to the GoStore!", font, TextBlack);

            // 3) repaint ticker box (width clamped to stop 7px before the right edge)
            int maxRight = Contents.Width - 7;
            int tickerW = Math.Min(TICKER_W_BASE, Math.Max(0, maxRight - TICKER_X));
            if (tickerW < 10) tickerW = 10; // guard for tiny windows
            Contents.DrawFilledRectangle(TICKER_X, TICKER_Y, (ushort)tickerW, TICKER_H, 0, TickerBack);
            Contents.DrawImage(TICKER_X + 4, TICKER_Y + ((TICKER_H - font.Size) / 2), _infoBoard, true);

            // no per-frame re-stamping of card overlays (single-threaded perf)
            // deferred close
            if (_deferClose)
            {
                _framesUntilClose--;
                if (_framesUntilClose <= 0)
                {
                    Closing = true;
                    _deferClose = false;
                }
            }
        }

        // ============================================================
        // Main render (static chrome + cards + footer)
        // ============================================================
        private void Render(string category)
        {
            if (_abortOpen) return;

            var font = Resources.Charcoal ?? Font_1x;

            // Clear content to transparent (Window draws background), then paint white body
            Contents.Clear(new Color(0x00000000));
            int contentLeft = SIDEBAR_W;
            int contentTop = HEADER_H;
            int contentW = Contents.Width - SIDEBAR_W;
            int contentH = Contents.Height - HEADER_H - FOOTER_H;
            if (contentW < 0) contentW = 0;
            if (contentH < 0) contentH = 0;
            Contents.DrawFilledRectangle(contentLeft, contentTop, (ushort)contentW, (ushort)contentH, 0, ContentWhite);

            // Sidebar & footer
            DrawSidebar();
            DrawFooter(font, category);

            // Region block
            if (_regionBlocked)
            {
                int cx = SIDEBAR_W + 14;
                int cy = HEADER_H + 24;
                Contents.DrawString(cx, cy, RegionBlockTitle, font, Color.Red);
                Contents.DrawString(cx, cy + 18, _regionBlockReason, font, TextWhite);
                Contents.DrawString(cx, cy + 36,
                    "If you believe this is in error, contact staff on Discord or email help@owen2k6.com.",
                    font, TextYellow);
                RenderSystemStyleBorder();
                return;
            }

            catagory = GetCatagoryIndex(category);

            // wipe old app buttons
            if (_repoFilesButtons != null)
            {
                foreach (var b in _repoFilesButtons)
                    if (b != null)
                        Controls.Remove(b);
                for (var i = 0; i < _repoFilesButtons.Length; i++)
                    _repoFilesButtons[i] = null;
            }

            // Cards grid (system-style buttons, text positioned manually)
            int gridLeft = SIDEBAR_W + 14;
            int gridTop = HEADER_H + 12;
            int cardW = 207, cardH = 78, gap = 5;

            int Line = 0, Colum = 0, buttonCount = 0;

            for (var i = page * 18; i < Math.Min(_repoFiles.Count, page * 18 + 18); i++)
            {
                if (i >= _repoFiles.Count) break;
                int appCat = GetCatagoryIndex(_repoFiles[i].Category);
                if (appCat != catagory) continue;

                if (Line >= 6) { Line = 0; Colum++; }

                int x = gridLeft + Colum * (cardW + gap);
                int y = gridTop + Line * (cardH + gap);

                var app = _repoFiles[i];

                // Build strings
                string name = app.Name;
                string by = "By " + app.Author;
                string ver = app.Version.Replace(@"\n", "\n");
                string goos = "GoOS " + app.GoOSVersion.TrimEnd() + "+";

                // Create a system-style button with EMPTY title — we’ll draw text ourselves
                var btn = new Button(this, (ushort)x, (ushort)y, (ushort)cardW, (ushort)cardH, string.Empty)
                {
                    Name = app.Name,
                    UseSystemStyle = true,
                    RenderWithAlpha = true,
                    CenterTitle = false,
                    ClickedAlt = _repoFiles_Click   // restore click behaviour
                };
                _repoFilesButtons[buttonCount] = btn;

                // Render the beveled panel first
                btn.Render();

                // TOP-LEFT text block (exact placement retained)
                int leftPad = 5;     // a touch in from the inner edge
                int topPad  = 6;     // aligns with GoOS tag baseline
                int lineStep = font.Size + 2;

                btn.Contents.DrawString(leftPad,                 topPad,                name, font, TextBlack);
                btn.Contents.DrawString(leftPad,                 topPad + lineStep,     by,   font, TextBlack);
                btn.Contents.DrawString(leftPad,                 topPad + lineStep * 2, ver,  font, TextBlack);

                // RIGHT-ALIGNED GoOS tag on the same top baseline
                int goosW  = font.MeasureString(goos);
                int rightX = cardW - 10 - goosW;                 // 10px right padding
                if (rightX < leftPad) rightX = leftPad;          // guard for tiny widths
                btn.Contents.DrawString(rightX, topPad, goos, font, TextBlack);

                // Blit the updated control back to the window
                RenderControls();

                buttonCount++;
                Line++;
            }

            // Render category buttons & pager
            foreach (var i in catagoryButtons) i.Render();
            prevousButton.Render();
            nextButton.Render();

            RenderSystemStyleBorder();
        }

        // Re-stamp the overlay text on all app cards (used only on click to avoid per-frame work)
        private void RestampCardTextOverlays()
        {
            var font = Resources.Charcoal ?? Font_1x;
            if (_repoFilesButtons == null) return;

            const int leftPad = 5;
            const int topPad  = 6;
            int lineStep = font.Size + 2;

            for (int i = 0; i < _repoFilesButtons.Length; i++)
            {
                var btn = _repoFilesButtons[i];
                if (btn == null) continue;

                int idx = GetIndexByTitle(btn.Name);
                if (idx < 0 || idx >= _repoFiles.Count) continue;

                var app = _repoFiles[idx];

                string name = app.Name;
                string by   = "By " + app.Author;
                string ver  = app.Version.Replace(@"\n", "\n");
                string goos = "GoOS " + app.GoOSVersion.TrimEnd() + "+";

                btn.Contents.DrawString(leftPad,                 topPad,                name, font, TextBlack);
                btn.Contents.DrawString(leftPad,                 topPad + lineStep,     by,   font, TextBlack);
                btn.Contents.DrawString(leftPad,                 topPad + lineStep * 2, ver,  font, TextBlack);

                int goosW  = font.MeasureString(goos);
                int rightX = (int)btn.Contents.Width - 10 - goosW; // 10 px right padding
                if (rightX < leftPad) rightX = leftPad;
                btn.Contents.DrawString(rightX, topPad, goos, font, TextBlack);
            }

            // push updated controls once
            RenderControls();
        }

        private void DrawSidebar()
        {
            int bodyH = Contents.Height - HEADER_H - FOOTER_H;
            if (bodyH < 0) bodyH = 0;

            Contents.DrawFilledRectangle(0, HEADER_H, SIDEBAR_W, (ushort)bodyH, 0, SidebarFill);
            // inset seam
            Contents.DrawLine(SIDEBAR_W, HEADER_H, SIDEBAR_W, HEADER_H + bodyH, DividerDark);
            Contents.DrawLine(SIDEBAR_W + 1, HEADER_H, SIDEBAR_W + 1, HEADER_H + bodyH, DividerLite);
        }

        private void DrawFooter(Font font, string category)
        {
            int footerTop = Contents.Height - FOOTER_H;

            Contents.DrawFilledRectangle(0, footerTop, (ushort)Contents.Width, FOOTER_H, 0, FooterFill);
            Contents.DrawLine(0, footerTop, Contents.Width, footerTop, DividerDark);

            // Selected category left
            string cat = (category ?? string.Empty).Trim();
            int catY = footerTop + (FOOTER_H - font.Size) / 2;
            Contents.DrawString(8, catY, cat, font, TextBlack);

            // Page number between buttons
            var p1 = page + 1;
            string pageText = p1.ToString();
            int pageTextW = font.MeasureString(pageText);

            int btnH = 20, btnW = 95;
            int prevX = Contents.Width - 20 - (btnW * 2);
            int nextX = Contents.Width - 10 - btnW;

            int pageX = (prevX + btnW + nextX) / 2 - (pageTextW / 2);
            int pageY = footerTop + (FOOTER_H - font.Size) / 2;
            Contents.DrawString(pageX, pageY, pageText, font, TextBlack);
        }

        private int GetIndexByTitle(string title)
        {
            for (var i = 0; i < _repoFiles.Count; i++)
                if (_repoFiles[i].Name == title)
                    return i;
            return -1;
        }

        private void _repoFiles_Click(string title)
        {
            if (_regionBlocked || _abortOpen) return;

            // Button render on press clears overlays; re-stamp them once here.
            RestampCardTextOverlays();

            int idx = GetIndexByTitle(title);
            if (idx >= 0)
                WindowManager.AddWindow(new DescriptionFrame(_repoFiles[idx]));
        }

        private void nextPage()
        {
            if (_regionBlocked || _abortOpen) return;

            int itemsInCategory = 0;
            foreach (var app in _repoFiles)
                if (GetCatagoryIndex(app.Category) == catagory)
                    itemsInCategory++;

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
        // Network wrappers
        // ============================================================
        private string GetInfoFile(string repo)
        {
            try { return HttpHelper.SimpleHttpGet(repo, "/info.glist"); }
            catch (HttpHelper.RegionBlockedException) { throw; }
            catch { return ""; }
        }

        private string GetInfoBoardFile()
        {
            try { return HttpHelper.SimpleHttpGet("api.goos.owen2k6.com", "/GoOS/" + Kernel.edition + "-status.gostore"); }
            catch (HttpHelper.RegionBlockedException) { throw; }
            catch { return ""; }
        }

        private string[] GetReposFile()
        {
            try
            {
                var result = HttpHelper.SimpleHttpGet("api.goos.owen2k6.com", "/GoOS/repos.gostore");
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
                var result = HttpHelper.SimpleHttpGet("api.goos.owen2k6.com", "/GoOS/cat.gostore");
                if (string.IsNullOrEmpty(result)) return new string[0];
                return result.Split('\n');
            }
            catch (HttpHelper.RegionBlockedException) { throw; }
            catch { return new string[0]; }
        }
    }
}
