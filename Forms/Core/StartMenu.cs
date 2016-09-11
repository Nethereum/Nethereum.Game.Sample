using System.Threading.Tasks;
using Urho;
using Urho.Gui;
using Urho.Actions;
using Urho.Shapes;
using Urho.Resources;
using System;

namespace SamplyGame
{
    /// <summary>
    /// This is not used on Xamarin forms, as the settings are decoupled from the game
    /// </summary>
    public class UIInternalWindowSettings : UIElement
    {
        Window window;
        Application application;
        LineEdit lineEditPrivateKey;
        LineEdit lineEditUrl;

        public void Init(Application application)
        {
            this.application = application;
            // Initialize Window
            InitWindow();
          
            // Create and add some controls to the Window
            InitControls();

        }

        void InitControls()
        {
            var cache = application.ResourceCache;
            // Create a Button
            Button button = new Button();
            button.Name = "Button";
            button.MinHeight = 24;
            


            Text text = new Text();
            text.Value = "Save";
            text.SetFont(cache.GetFont(Assets.Fonts.Font), application.Graphics.Width / 30);
            text.HorizontalAlignment = HorizontalAlignment.Center;
            text.VerticalAlignment = VerticalAlignment.Center;
            button.AddChild(text);

            var textPrivateKey = new Text();
            textPrivateKey.Value = "Private Key";
            
            textPrivateKey.HorizontalAlignment = HorizontalAlignment.Center;
            textPrivateKey.VerticalAlignment = VerticalAlignment.Center;

            var textUrl = new Text();
            textUrl.Value = "Url";
            
            textUrl.HorizontalAlignment = HorizontalAlignment.Center;
            textUrl.VerticalAlignment = VerticalAlignment.Center;

            // Create a LineEdit
            lineEditPrivateKey = new LineEdit();
            lineEditPrivateKey.Name = "lineEditprivateKey";
            lineEditPrivateKey.TextCopyable = true;   
            lineEditPrivateKey.Cursor.VerticalAlignment = VerticalAlignment.Center;
            lineEditPrivateKey.TextElement.VerticalAlignment = VerticalAlignment.Center;
           // lineEditPrivateKey.Text = Ethereum.GameScoreService.PRIVATE_KEY;


            lineEditUrl = new LineEdit();
            lineEditUrl.Name = "lineEditUrl";
            lineEditUrl.Cursor.VerticalAlignment = VerticalAlignment.Center;
            lineEditUrl.TextElement.VerticalAlignment = VerticalAlignment.Center;
            lineEditUrl.TextCopyable = true;
           // lineEditUrl.Text = Ethereum.GameScoreService.DEFAULT_MORDEN;

            // Add controls to Window

            
            window.AddChild(textPrivateKey);
            window.AddChild(lineEditPrivateKey);
            window.AddChild(textUrl);
            window.AddChild(lineEditUrl);
            window.AddChild(button);
            // Apply previously set default style

            button.SetStyleAuto(null);
            lineEditPrivateKey.SetStyleAuto(null);
            lineEditUrl.SetStyleAuto(null);

            textPrivateKey.SetFont(cache.GetFont(Assets.Fonts.Font), application.Graphics.Width / 30);
            textUrl.SetFont(cache.GetFont(Assets.Fonts.Font), application.Graphics.Width / 30);
            lineEditPrivateKey.TextElement.SetFontSize(application.Graphics.Width / 20);
            lineEditUrl.TextElement.SetFontSize(application.Graphics.Width / 20);
            lineEditUrl.CursorPosition = 0;
            lineEditPrivateKey.CursorPosition = 0;

            button.SubscribeToReleased(_ => Save());
        }

        void Save()
        {
            if(!string.IsNullOrWhiteSpace(lineEditPrivateKey.Text) || !string.IsNullOrWhiteSpace(lineEditUrl.Text))
            {
                Ethereum.GameScoreService.Init(lineEditPrivateKey.Text, lineEditUrl.Text);
                Close();
            }

        }

        void ResetSettings()
        {
            if(Ethereum.GameScoreService.Current == null)
            {
               // this.lineEditPrivateKey.Text = Ethereum.GameScoreService.PRIVATE_KEY;
               // this.lineEditUrl.Text = Ethereum.GameScoreService.DEFAULT_MORDEN;
            }
            else
            {
                this.lineEditPrivateKey.Text = Ethereum.GameScoreService.Current.PrivateKey;
                this.lineEditUrl.Text = Ethereum.GameScoreService.Current.Url;
            }
        }
        

        void InitWindow()
        {
            // Create the Window and add it to the UI's root node
            window = new Window();
            this.AddChild(window);

            // Set Window size and layout settings
            window.SetMinSize(application.Graphics.Width - 30, application.Graphics.Height / 2);
            window.SetLayout(LayoutMode.Vertical, 6, new IntRect(6, 6, 6, 6));
            window.SetAlignment(HorizontalAlignment.Center, VerticalAlignment.Center);
            window.Name = "Window";

            // Create Window 'titlebar' container
            UIElement titleBar = new UIElement();
            titleBar.SetMinSize(0, 24);
            titleBar.VerticalAlignment = VerticalAlignment.Top;
            titleBar.LayoutMode = LayoutMode.Horizontal;

            // Create the Window title Text
            var windowTitle = new Text();
            windowTitle.Name = "WindowTitle";
            windowTitle.Value = "Ethereum Settings";

            // Create the Window's close button
            Button buttonClose = new Button();
            buttonClose.Name = "CloseButton";

            // Add the controls to the title bar
            titleBar.AddChild(windowTitle);
            titleBar.AddChild(buttonClose);

            // Add the title bar to the Window
            window.AddChild(titleBar);

            // Apply styles
            window.SetStyleAuto(null);
            windowTitle.SetStyleAuto(null);
            buttonClose.SetStyle("CloseButton", null);

            windowTitle.SetFont(application.ResourceCache.GetFont(Assets.Fonts.Font), application.Graphics.Width / 30);
      
            buttonClose.SubscribeToReleased(_ => this.Visible = false);


        }

        public void Close()
        {
            this.Visible = false;
            ResetSettings();
        }
    }

    public class StartMenu : Component
	{
		TaskCompletionSource<bool> menuTaskSource;
		Node bigAircraft;
		Node rotor;
		Text textBlock;
        UIInternalWindowSettings settingsWindow;
        Text topPlayers;
        Window windowMenu;
        
        public string TopPlayersText { get; set; }

        Node menuLight;
		bool finished = true;

        public event Action ShowSettingsClicked;

		public StartMenu()
		{
			ReceiveSceneUpdates = true;
		}

		public async Task ShowStartMenu(bool gameOver)
		{
			var cache = Application.ResourceCache;
			bigAircraft = Node.CreateChild();
			var model = bigAircraft.CreateComponent<StaticModel>();

			if (gameOver)
			{
				model.Model = cache.GetModel(Assets.Models.Enemy1);
				model.SetMaterial(cache.GetMaterial(Assets.Materials.Enemy1).Clone(""));
				bigAircraft.SetScale(0.3f);
				bigAircraft.Rotate(new Quaternion(180, 90, 20), TransformSpace.Local);
			}
			else
			{
				model.Model = cache.GetModel(Assets.Models.Player);
				model.SetMaterial(cache.GetMaterial(Assets.Materials.Player).Clone(""));
				bigAircraft.SetScale(1f);
				bigAircraft.Rotate(new Quaternion(0, 40, -50), TransformSpace.Local);
			}

			bigAircraft.Position = new Vector3(10, 2, 10);
			bigAircraft.RunActions(new RepeatForever(new Sequence(new RotateBy(1f, 0f, 0f, 5f), new RotateBy(1f, 0f, 0f, -5f))));

			//TODO: rotor should be defined in the model + animation
			rotor = bigAircraft.CreateChild();
			var rotorModel = rotor.CreateComponent<Box>();
			rotorModel.Color = Color.White;
			rotor.Scale = new Vector3(0.1f, 1.5f, 0.1f);
			rotor.Position = new Vector3(0, 0, -1.3f);
			var rotorAction = new RepeatForever(new RotateBy(1f, 0, 0, 360f*6)); //RPM
			rotor.RunActions(rotorAction);
			
			menuLight = bigAircraft.CreateChild();
			menuLight.Position = new Vector3(-3, 6, 2);
			menuLight.AddComponent(new Light { LightType = LightType.Point, Brightness = 0.3f });

			await bigAircraft.RunActionsAsync(new EaseIn(new MoveBy(1f, new Vector3(-10, -2, -10)), 2));

            InitMenu(cache);
            //InitSettings(cache);

            topPlayers = new Text();
            topPlayers.HorizontalAlignment = HorizontalAlignment.Center;
          
            
            topPlayers.SetFont(cache.GetFont(Assets.Fonts.Font), Application.Graphics.Width / 30);
            Application.UI.Root.AddChild(topPlayers);

            
            menuTaskSource = new TaskCompletionSource<bool>();
			finished = false;
			await menuTaskSource.Task;
		}

        //private void InitSettings(ResourceCache cache)
        //{
        //    if (settingsWindow == null)
        //    {
        //        XmlFile defaultUIStyle = cache.GetXmlFile(Assets.UI.DefaultStyle);
        //        settingsWindow = new UIInternalWindowSettings();
        //        settingsWindow.HorizontalAlignment = HorizontalAlignment.Center;
        //        settingsWindow.VerticalAlignment = VerticalAlignment.Center;

        //        // Set the loaded style as default style
        //        settingsWindow.SetDefaultStyle(defaultUIStyle);
        //        settingsWindow.Init(Application);

        //        Application.UI.Root.AddChild(settingsWindow);
        //    }

        //    settingsWindow.Visible = false;
        //}

        private void InitMenu(ResourceCache cache)
        {
            windowMenu = new Window();
            windowMenu.SetMinSize(Application.Graphics.Width / 2, Application.Graphics.Height / 4);
            windowMenu.SetLayout(LayoutMode.Vertical, 6, new IntRect(6, 6, 6, 6));
            windowMenu.SetAlignment(HorizontalAlignment.Center, VerticalAlignment.Bottom);
            XmlFile defaultUIStyle = cache.GetXmlFile(Assets.UI.DefaultStyle);
            windowMenu.SetStyleAuto(null);
            // Set the loaded style as default style
            windowMenu.SetDefaultStyle(defaultUIStyle);

            var buttonStart = new Button();
            textBlock = new Text();
            textBlock.Value = "Start";
            textBlock.SetFont(cache.GetFont(Assets.Fonts.Font), Application.Graphics.Width / 15);

            textBlock.HorizontalAlignment = HorizontalAlignment.Center;
            textBlock.VerticalAlignment = VerticalAlignment.Center;
            buttonStart.AddChild(textBlock);
            buttonStart.SubscribeToReleased(_ => Start());

            //settings
            var buttonSettings = new Button();
            var textBlockSettings = new Text();
            textBlockSettings.Value = "Settings";
            textBlockSettings.SetFont(cache.GetFont(Assets.Fonts.Font), Application.Graphics.Width / 15);
            textBlockSettings.HorizontalAlignment = HorizontalAlignment.Center;
            textBlockSettings.VerticalAlignment = VerticalAlignment.Center;
            buttonSettings.AddChild(textBlockSettings);
            buttonSettings.SubscribeToReleased(_ => ShowSettings());

            windowMenu.AddChild(buttonStart);
            windowMenu.AddChild(buttonSettings);
            buttonStart.SetStyleAuto(null);
            buttonSettings.SetStyleAuto(null);
            Application.UI.Root.AddChild(windowMenu);
        }

        private void ShowSettings()
        {
            //this is not needed showsettings will be handled by forms and popupmodalasync
            //settingsWindow.Visible = true;
            //windowMenu.Visible = false;
            ShowSettingsClicked?.Invoke();
        }

        private async void Start()
        {
            finished = true;
          
            await bigAircraft.RunActionsAsync(new EaseIn(new MoveBy(1f, new Vector3(-10, -2, -10)), 3));
            rotor.RemoveAllActions();
            menuTaskSource.TrySetResult(true);
            Application.UI.Root.RemoveChild(windowMenu, 0);
            Application.UI.Root.RemoveChild(topPlayers, 0);
            Application.UI.Root.RemoveChild(settingsWindow, 0);
        }

		protected override async void OnUpdate(float timeStep)
		{
			if (finished)
				return;

            topPlayers.Value = TopPlayersText;

            //this is not needed showsettings will be handled by forms and popupmodalasync
            //if (settingsWindow.Visible == false && windowMenu.Visible == false)
            //{
            //    windowMenu.Visible = true;
            //}
        }
	}
}
