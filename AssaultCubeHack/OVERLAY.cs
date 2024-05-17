using AssaultCubeHack;
using bbjuyhgazdf.Windows;
using RL.game;
using RL.Properties;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Utilities;
using VECRPROJECT.util;
using static Framework.Memory;
using Menu = AssaultCubeHack.Menu;

namespace Examples
{
    public class Example : IDisposable
    {
        private static int fixOverlay = 0;

        public static int _oldtargetWindowPositionLeft;
        public static int _oldtargetWindowPositionTop;

        [DllImport("User32.dll")]
        public static extern short GetAsyncKeyState(Keys vKey); // Keys enumeration

        //keyboard commands
        private GlobalKeyboardHook gkh = new GlobalKeyboardHook();

        //refresh windows
        public static int refreshWindows = 1500;

        //target process
        public const string processGame = "plutonium-bootstrapper-win32.exe";
        public const string keyHelpFindProcess = "t6mp";
        public const string processName = "Moonlight";
        public const string processClassName = "SDL_app";
        public static string processMainApp = @"\r\n";
        public static Process process;

        //threads for updating rendering
        private Thread overlayThread;
        public static bool isRunning = false;

        //game objects
        private static int numPlayers = 18;
        public static int gameWidth, gameHeight = 1;

        //GRAPHIC
        public static GraphicsWindow _window;
        public static Dictionary<string, bbjuyhgazdf.Drawing.SolidBrush> _brushes;
        public static Dictionary<string, bbjuyhgazdf.Drawing.Font> _fonts;
        //private readonly Dictionary<string, bbjuyhgazdf.Drawing.Image> _images;

        //MENU
        public static bool Menu_Showed = false;
        public static bool Menu_Loading = false;

        public Example()
        {
            //Security.CVP();
            //Security.security();

            if (Security.bpAcc == true && Security.pfpf == 1)
            {
                Menu menu = new Menu();
                menu.InitializationCONTROLS();
            }
            else
            {
                //fail
                Security.FAILACC();
            }

            _brushes = new Dictionary<string, bbjuyhgazdf.Drawing.SolidBrush>();
            _fonts = new Dictionary<string, bbjuyhgazdf.Drawing.Font>();
            //_images = new Dictionary<string, bbjuyhgazdf.Drawing.Image>();

            var gfx = new bbjuyhgazdf.Drawing.Graphics()
            {
                MeasureFPS = Settings.Default.Show_FPS,
                VSync = Settings.Default.VSYNC,
                TextAntiAliasing = Settings.Default.TextAntiAliasing,
                PerPrimitiveAntiAliasing = true,
                UseMultiThreadedFactories = true,
            };

            _window = new GraphicsWindow(0, 0, gameWidth, gameHeight, gfx)
            {
                FPS = Settings.Default.FPS,
                IsTopmost = true,
                IsVisible = true,
            };

            //MYCODE
            AttachToGameProcess();

            _window.SetupGraphics += _window_SetupGraphics;
            _window.DrawGraphics += _window_DrawGraphics;
            _window.DestroyGraphics += _window_DestroyGraphics;

            //CONTROLLER
            Controller.CONTROLLER.startController();
        }

        public void AttachToGameProcess()
        {

            if (Security.bpAcc == true && Security.pfpf == 1)
            {

                var ConsoleAPP = NativeMethods.GetConsoleWindow();
                bool success = false;

                do
                {
                    Console.WriteLine("Searching Moonlight process with SDL_app ClassName");

                    var ClassName = NativeMethods.FindWindow(processClassName, processName);

                    if (Memory.GetProcessesByName("Moonlight", out process) && ClassName != IntPtr.Zero)
                    {
                        Console.WriteLine("Attaching...");
                        //try to attach to game process

                        //success  

                        Console.WriteLine("success");

                        NativeMethods.ShowWindow(process.MainWindowHandle, NativeMethods.SW_RESTORE);
                        NativeMethods.SetForegroundWindow(process.MainWindowHandle);
                        success = true;
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("Attached Handle: " + ClassName.ToString("X"));
                        Console.WriteLine("Name: " + process.ProcessName);

                        Framework.Memory.init(processGame, "");

                        //NativeMethods.ShowWindow(ConsoleAPP, NativeMethods.SW_HIDE);
                    }
                    else
                    {
                        Thread.Sleep(2500);
                        AttachToGameProcess();
                        return;
                    }
                } while (!success);

                StartThreads();
            }

            else
            {
                //fail
                Security.FAILACC();
            }

        }

        private void StartThreads()
        {

            if (Security.bpAcc == true && Security.pfpf == 1)
            {
                //CALCUL SPECIFIC ADDRESS
                Offsets.baseAddress = Framework.Memory.Read<int>(Convert.ToUInt32(Offsets.baseAddress));
                Offsets.PlayersLIST = (Offsets.baseAddress) - Offsets.PlayersLIST;
                Offsets.VSAT = (Offsets.baseAddress) - Offsets.VSAT;

                //Console.WriteLine("Base Address: " + Offsets.baseAddress.ToString("X"));
                //Console.WriteLine("Players List Address: " + Offsets.PlayersLIST.ToString("X"));
                //Console.WriteLine("VSAT Address: " + Offsets.VSAT.ToString("X"));

                Console.WriteLine("Ready.");

                //END

                //start thread flag
                isRunning = true;

                //start thread for positioning and sizing overlay on top of target process
                overlayThread = new Thread(UpdateWindow);
                overlayThread.Start();

                //start thread for playing with memory and drawing overlay
                overlayThread = new Thread(UpdateHack);
                overlayThread.Start();

                //start thread for change the settings value
                overlayThread = new Thread(KeysPress);
                overlayThread.Start();

                //set up low level keyboard hooking to recieve key events while not in focus
                gkh.HookedKeys.Add(Settings.Default.Key_Show_MENU);
                gkh.KeyUp += new KeyEventHandler(KeyOpenMenu);

            }
            else
            {
                //fail
                Security.FAILACC();
            }
        }

        public void KeysPress()
        {
            while (isRunning)
            {
                if (GetAsyncKeyState((Keys)Enum.Parse(typeof(Keys), Settings.Default.AIM_Key)) < 0)
                {
                    Aimbot.aim = true;
                }
                else
                {
                    Aimbot.aim = false;
                }

                Thread.Sleep(1);
            }
        }

        private void KeyOpenMenu(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Settings.Default.Key_Show_MENU)
            {

                Menu myForm = new Menu();

                foreach (Form f in Application.OpenForms)
                {
                    if (Menu_Showed == true)
                    {
                        f.Close();
                        Menu_Showed = false;
                        return;

                    }
                }

                if (Menu_Showed == false && Menu_Loading == false)
                {
                    try
                    {
                        Menu_Loading = true;
                        Application.Run(myForm);
                        myForm.BringToFront();
                        myForm.Activate();
                    }
                    catch { }
                }
                e.Handled = true;
            }
        }

        private void UpdateWindow()
        {
            //update flag, make sure game is still running
            while (isRunning)
            {
                var ClassName = NativeMethods.FindWindow(processClassName, processName);

                if (!Memory.IsProcessRunning(process) || ClassName == IntPtr.Zero)
                {
                    isRunning = false;
                    Security.FAILACC();
                    return;
                }

                //ensure we are in focus and on top of game
                SetOverlayPosition(_window.Handle);

                //Refresh FPS
                _window.FPS = Settings.Default.FPS;
                //_window.Graphics.TextAntiAliasing = Settings.Default.TextAntiAliasing;

                //CLEAN MEMORY
                Security.CleanMemory();

                //Refresh Open Menu button
                gkh.HookedKeys.Clear();
                gkh.HookedKeys.Add(Settings.Default.Key_Show_MENU);

                //REFRESH INI FILE
                Clsini.INIConfig = new Clsini("profiles/" + Settings.Default.Profiles + "/config.ini"); //get new config

                //sleep for a bit, we don't need to move around constantly
                Thread.Sleep(refreshWindows);
            }
        }

        private void SetOverlayPosition(IntPtr overlayHandle)
        {

            //get window handle
            IntPtr gameProcessHandle = process.MainWindowHandle;
            if (gameProcessHandle == IntPtr.Zero)
                return;

            //get position and size of window
            NativeMethods.RECT targetWindowPosition, targetWindowSize;

            //GAME
            if (!NativeMethods.GetWindowRect(gameProcessHandle, out targetWindowPosition))
                return;
            if (!NativeMethods.GetClientRect(gameProcessHandle, out targetWindowSize))
                return;
            if (overlayHandle.ToString() == null)
                return;
            if (gameProcessHandle == IntPtr.Zero)
                return;

            //calculate width and height of full target window
            int width = targetWindowPosition.Right - targetWindowPosition.Left;
            int height = targetWindowPosition.Bottom - targetWindowPosition.Top;

            //calculate inner window size without borders      
            int bWidth = targetWindowPosition.Right - targetWindowPosition.Left;
            int bHeight = targetWindowPosition.Bottom - targetWindowPosition.Top;

            width = targetWindowSize.Right - targetWindowSize.Left;
            height = targetWindowSize.Bottom - targetWindowSize.Top;

            int borderWidth = (bWidth - targetWindowSize.Right) / 2;
            int borderHeight = (bHeight - targetWindowSize.Bottom);
            borderHeight -= borderWidth; //remove bottom

            targetWindowPosition.Left += borderWidth;
            targetWindowPosition.Top += borderHeight;


            //Pass three times for fixing the screen in startup program
            if (fixOverlay < 1)
            {
                gameWidth = 0; //FIX RESOLUTION
                gameHeight = 0; //FIX RESOLUTION

                _window.Width = gameWidth;
                _window.Height = gameHeight;

                fixOverlay += 1;
            }


            //Return function only if the windows doesn't have moved
            if (targetWindowPosition.Left == _oldtargetWindowPositionLeft
                && targetWindowPosition.Top == _oldtargetWindowPositionTop
                && gameWidth == width
                && gameHeight == height
                && _window.Width == width
                && _window.Height == height
                && _window.IsVisible == true
                && _window.IsRunning == true
                && _window.IsInitialized == true) { return; }

            //save window size for ESP WorldToScreen translation
            gameWidth = width;
            gameHeight = height;

            _window.Width = gameWidth;
            _window.Height = gameHeight;

            _window.X = targetWindowPosition.Left;
            _window.Y = targetWindowPosition.Top;

            //SAVE NEW TARGET WINDOWS POSITION
            _oldtargetWindowPositionLeft = targetWindowPosition.Left;
            _oldtargetWindowPositionTop = targetWindowPosition.Top;

            NativeMethods.MoveWindow(overlayHandle, targetWindowPosition.Left, targetWindowPosition.Top, width, height, true);
        }

        private void UpdateHack()
        {

            overlayThread = new Thread(readAddressThreads);
            overlayThread.Start();

            //update loop
            while (isRunning == true)
            {

                //read
                ReadGameMemory();

                //aimbot
                if (Security.pfpf == 1 && Settings.Default.Active_Func_Key == 0 && Security.bpAcc == true)
                {
                    Aimbot.aimbot.UpdateAimbot();
                }

                //refresh
                Thread.Sleep(8);
            }

            //cleanup
            Memory.CloseProcess();

        }

        public static void readAddressThreads()
        {
            while (true)
            {
                //Matrix.viewMatrix = Framework.Memory.ReadMatrix(Convert.ToUInt32(Offsets.viewMatrix));

                Framework.Memory.addScatterReadRequest(Convert.ToUInt32(Offsets.viewMatrix), 64);

                Framework.Memory.addScatterReadRequest(Convert.ToUInt32(Offsets.SelfLocalPlayer + Offsets.SelfLocalPlayerTEAM), sizeof(int));
                Framework.Memory.addScatterReadRequest(Convert.ToUInt32(Offsets.SelfLocalPlayer + Offsets.SelfLocalPlayerNumberID), sizeof(int));
                Framework.Memory.addScatterReadRequest(Convert.ToUInt32(Offsets.SelfLocalPlayer + Offsets.SelfLocalPlayerPOSITION), sizeof(int));

                for (int i = 0; i <= numPlayers; i++)
                {
                    int strPlayerList = (Offsets.PlayersLIST + Offsets.ptrPlayerLISTArray * i);
                    int strPlayerPosition = Offsets.baseAddress + Offsets.ptrPlayerPositionArray * i;

                    Framework.Memory.addScatterReadRequest(Convert.ToUInt32(strPlayerList + Offsets.PlayerTEAM), sizeof(int));
                    Framework.Memory.addScatterReadRequest(Convert.ToUInt32(strPlayerList + Offsets.PlayerNumberID), sizeof(int));
                    Framework.Memory.addScatterReadRequest(Convert.ToUInt32(strPlayerList + Offsets.PlayerTEAMForFFA), sizeof(int));
                    Framework.Memory.addScatterReadRequest(Convert.ToUInt32(strPlayerList + Offsets.PlayerISALIVE), sizeof(int));
                    Framework.Memory.addScatterReadRequest(Convert.ToUInt32(strPlayerList + Offsets.PlayerISALIVE2), sizeof(int));
                    Framework.Memory.addScatterReadRequest(Convert.ToUInt32(strPlayerList + Offsets.PlayerCROUCH), sizeof(int));
                    Framework.Memory.addScatterReadRequest(Convert.ToUInt32(strPlayerList + Offsets.PlayerWeapon), sizeof(int));
                    Framework.Memory.addScatterReadRequest(Convert.ToUInt32(strPlayerList + Offsets.PlayerPING), sizeof(int));
                    Framework.Memory.addScatterReadRequest(Convert.ToUInt32(strPlayerList + Offsets.PlayerNAME), sizeof(int));


                    Framework.Memory.addScatterReadRequest(Convert.ToUInt32(strPlayerPosition + Offsets.headPos), 12);
                    Framework.Memory.addScatterReadRequest(Convert.ToUInt32(strPlayerPosition + Offsets.footPos), 12);

                }

                Framework.Memory.ExecuteReadScatter();

                Matrix.viewMatrix = Framework.Memory.ReadMatrixScatter(Convert.ToUInt32(Offsets.viewMatrix));

                Thread.Sleep(1);
            }
        }

        private void ReadGameMemory()
        {
            //passe seulement si le jeu est ouvert ou si le refreshdrawing == true + self local player est == a la valeur de base
            if (!isRunning || (GetActiveWindowTitle() == false)) return;

            //VSAT
            if (Settings.Default.VSAT == true)
            {
                Memory.Write<int>(Offsets.VSAT, 1);
            }
            //END

            for (int i = 0; i <= numPlayers; i++)
            {
                // str = structure
                int strPlayerPosition = Offsets.baseAddress + Offsets.ptrPlayerPositionArray * i;
                int strPlayerList = (Offsets.PlayersLIST + Offsets.ptrPlayerLISTArray * i);
                int PlayerTeam = Framework.Memory.ReadScatter<int>(Convert.ToUInt32(strPlayerList + Offsets.PlayerTEAM));

                int PlayerTeam2 = Framework.Memory.ReadScatter<int>(Convert.ToUInt32(strPlayerList + Offsets.PlayerTEAMForFFA));

                if (Player.players.Count >= numPlayers)
                {
                    // if Player list is > numPlayers remove Player list
                    if (Player.players.Count > numPlayers)
                    {
                        //Player.players.RemoveAt(i);
                        Player.players.Remove(Player.players[i]);
                        continue;
                    }

                    if (i <= numPlayers - 1)
                    {
                        Player.players[i] = new Player(strPlayerPosition, strPlayerList);
                        //END
                    }
                }
                else
                {
                    Player.players.Add(new Player(strPlayerPosition, strPlayerList));
                }

                //Enlever son propre ESP && GET SELF LOCAL PLAYER TEAM
                if (Player.SelfPlayerNumberID == i)
                {
                    //Offsets.SelfLocalPlayerTEAM = PlayerTeam; //decimal to hex

                    Player.players[i] = new Player(0, 0);
                    //Player.players.Remove(Player.players[i]);
                    continue;
                }
                //END

                //Enlever les esp ALLIES si desactivé
                if (Settings.Default.Show_Allies == false && PlayerTeam2 == 0)
                {
                    Console.WriteLine(Player.SelfPlayerTeam);
                    if (PlayerTeam == Player.SelfPlayerTeam)
                    {
                        Player.players[i] = new Player(0, 0);
                        continue;
                    }
                }

            }

            Framework.Memory.ExecuteReadScatter();
        }

        private void AssaultHack_FormClosing(object sender, FormClosingEventArgs e)
        {
            //kill threads
            isRunning = false;

            //wait for threads to finish
            overlayThread.Join(2000);

            //detach from process
            Memory.CloseProcess();
            Security.FAILACC();
        }

        public void _window_SetupGraphics(object sender, SetupGraphicsEventArgs e)
        {

            try
            {
                var gfx = e.Graphics;

                if (e.RecreateResources)
                {
                    foreach (var pair in _brushes) pair.Value.Dispose();
                    foreach (var pair in _fonts) pair.Value.Dispose();
                    //foreach (var pair in _images) pair.Value.Dispose();
                }

                _brushes["black"] = gfx.CreateSolidBrush(0, 0, 0);
                _brushes["red"] = gfx.CreateSolidBrush(255, 0, 0);
                _brushes["white"] = gfx.CreateSolidBrush(255, 255, 255);
                _brushes["green"] = gfx.CreateSolidBrush(0, 255, 0);
                _brushes["OVERLAY01"] = gfx.CreateSolidBrush(Settings.Default.Default_Fore_Color.R, Settings.Default.Default_Fore_Color.G, Settings.Default.Default_Fore_Color.B);
                _brushes["OVERLAY02"] = gfx.CreateSolidBrush(Settings.Default.Default_Background_Color.R, Settings.Default.Default_Background_Color.G, Settings.Default.Default_Background_Color.B);
                _brushes["ESPCOLOR"] = gfx.CreateSolidBrush(Settings.Default.ESP_Color.R, Settings.Default.ESP_Color.G, Settings.Default.ESP_Color.B);
                _brushes["CROSSHAIR"] = gfx.CreateSolidBrush(Settings.Default.CROSSHAIR_Color.R, Settings.Default.CROSSHAIR_Color.G, Settings.Default.CROSSHAIR_Color.B);
                _brushes["ESPFILLEDBORDERCOLOR"] = gfx.CreateSolidBrush(Settings.Default.ESP_FILLED_BORDER_COLOR.R, Settings.Default.ESP_FILLED_BORDER_COLOR.G, Settings.Default.ESP_FILLED_BORDER_COLOR.B);
                _brushes["AIMBOT_FOV"] = gfx.CreateSolidBrush(Settings.Default.AIMBOT_FOV_Color.R, Settings.Default.AIMBOT_FOV_Color.G, Settings.Default.AIMBOT_FOV_Color.B);
                _brushes["ESP_VISIBLE_TEXT"] = gfx.CreateSolidBrush(Settings.Default.ESP_VISIBLE_TEXT.R, Settings.Default.ESP_VISIBLE_TEXT.G, Settings.Default.ESP_VISIBLE_TEXT.B);
                _brushes["SNAPLINE"] = gfx.CreateSolidBrush(Settings.Default.SNAPLINE_Color.R, Settings.Default.SNAPLINE_Color.G, Settings.Default.SNAPLINE_Color.B);
                _brushes["background"] = gfx.CreateSolidBrush(0, 0, 0, 0);
                _brushes["TextRectangle"] = gfx.CreateSolidBrush(0, 0, 0, 150);

                if (e.RecreateResources) return;

                _fonts["arial"] = gfx.CreateFont("Arial", 12);
                _fonts["consolas"] = gfx.CreateFont("Consolas", 14);
                _fonts["OVERLAY01"] = gfx.CreateFont("Consolas", 9);
                _fonts["OVERLAY02"] = gfx.CreateFont("Consolas", 12);
                _fonts["ESP_TEXT_SIZE"] = gfx.CreateFont(Settings.Default.FontText, Settings.Default.ESP_SIZE_TEXT);

            }
            catch
            {
                //FAIL
                File.WriteAllBytes("profiles/" + Settings.Default.Profiles + "/config.ini", Resources.config);
                Security.FAILACC();
            }

        }

        private void _window_DestroyGraphics(object sender, DestroyGraphicsEventArgs e)
        {
            foreach (var pair in _brushes) pair.Value.Dispose();
            foreach (var pair in _fonts) pair.Value.Dispose();
            //foreach (var pair in _images) pair.Value.Dispose();
            var gfx = e.Graphics;
            gfx.Destroy();
            gfx.Dispose();
            gfx.EndScene();
        }

        private void _window_DrawGraphics(object sender, DrawGraphicsEventArgs e)
        {
            var gfx = e.Graphics;

            gfx.ClearScene();

            if (GetActiveWindowTitle() == false) return; //Enter inside only if your windows is entered in game

            if (Settings.Default.Show_FPS == true)
            {
                var padding = 16;
                var infoText = new StringBuilder()
                    .Append("FPS: " + gfx.FPS.ToString().PadRight(padding))
                    //.Append("FrameTime: ").AppendLine(e.FrameTime.ToString().PadRight(padding))
                    //.Append("FrameCount: ").AppendLine(e.FrameCount.ToString().PadRight(padding))
                    //.Append("DeltaTime: ").AppendLine(e.DeltaTime.ToString().PadRight(padding))
                    .ToString();

                //gfx.MeasureFPS = Settings.Default.ShowFPS;
                bbjuyhgazdf.Drawing.Point outputSize = gfx.MeasureString(_fonts["consolas"], infoText);

                if (Settings.Default.FPS_CORNER == 1) //TOP LEFT
                {
                    gfx.DrawTextWithBackground(_fonts["consolas"], _brushes["green"], _brushes["black"], 0, 0, infoText);
                }
                else if (Settings.Default.FPS_CORNER == 2) //TOP RIGHT
                {
                    gfx.DrawTextWithBackground(_fonts["consolas"], _brushes["green"], _brushes["black"], gameWidth - (outputSize.X), 0, infoText);
                }
                else if (Settings.Default.FPS_CORNER == 3) //BOT LEFT
                {
                    gfx.DrawTextWithBackground(_fonts["consolas"], _brushes["green"], _brushes["black"], 0, gameHeight - (outputSize.Y), infoText);
                }
                else if (Settings.Default.FPS_CORNER == 4) //BOT RIGHT
                {
                    gfx.DrawTextWithBackground(_fonts["consolas"], _brushes["green"], _brushes["black"], gameWidth - (outputSize.X), gameHeight - (outputSize.Y), infoText);
                }
            }

            DrawFigure(gfx);
        }

        private void DrawFigure(bbjuyhgazdf.Drawing.Graphics gfx)
        {

            if (GetActiveWindowTitle() == false) return; //Enter inside only if your windows is entered in game

            try
            {
                //OVERLAY
                Draw.DRAW.DrawOverlay(gfx);
                //END

                //AIM FOV
                Draw.DRAW.DrawAimFov(gfx);
                //END 

                //CROSSHAIR
                Draw.DRAW.DrawCrosshair(gfx);
                //END

                //DRAWING  //DRAW ESP, SNAPLINE
                Draw.DRAW.DrawESP(gfx);
                //END

            }
            catch { }
        }

        public static bool GetActiveWindowTitle()
        {
            var handle = NativeMethods.GetForegroundWindow();
            var length = NativeMethods.GetWindowTextLength(handle);
            var builder = new StringBuilder(length + 1);

            NativeMethods.GetWindowText(handle, builder, builder.Capacity);

            if (builder.ToString().ToLower().Contains(processName.ToLower()) == true || (builder.ToString().ToLower().Contains(processMainApp.ToLower()) == true))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool GetActiveWindowTitleAIMBOT()
        {
            var handle = NativeMethods.GetForegroundWindow();
            var length = NativeMethods.GetWindowTextLength(handle);
            var builder = new StringBuilder(length + 1);

            NativeMethods.GetWindowText(handle, builder, builder.Capacity);

            if (builder.ToString().ToLower().Contains(processMainApp.ToLower()) == true) return false;

            if (builder.ToString().ToLower().Contains(processName.ToLower()) == true)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void Run()
        {
            _window.Create();
            _window.Join();
        }

        public void Stop()
        {
            _window.Dispose();
        }

        public void Join()
        {
            _window.Join();
        }

        public void ReCreate()
        {
            _window.Recreate();
        }

        ~Example()
        {
            Dispose(false);
        }

        #region IDisposable Support
        private bool disposedValue;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
