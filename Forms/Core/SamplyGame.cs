using System.Threading.Tasks;
using SamplyGame.Aircrafts.Enemies;
using Urho;
using Urho.Gui;
using Urho.Physics;
using Urho.Actions;
using Urho.Shapes;
using System;

namespace SamplyGame
{
	public class SamplyGame : Application
	{
		const string CoinstFormat = "{0} coins";

		public int Coins { get; private set; }
        public StartMenu StartMenu { get; private set; }

        public event Action<StartMenu> NewStartMenu;
        public event Action<StartMenu> RemovedStartMenu;
        public event Action<int> FinishedGame;

        Scene scene;
		Text coinsText;

		public Player Player { get; private set; }

		public Viewport Viewport { get; private set; }

		public SamplyGame(ApplicationOptions options) : base(options) { }

		protected override void Start()
		{
			base.Start();
			CreateScene();
			Input.SubscribeToKeyDown(e =>
			{
				if (e.Key == Key.Esc) Exit();
				if (e.Key == Key.C) AddCollisionDebugBox(scene, true);
				if (e.Key == Key.V) AddCollisionDebugBox(scene, false);
			});
		}

		static void AddCollisionDebugBox(Node rootNode, bool add)
		{
			var nodes = rootNode.GetChildrenWithComponent<CollisionShape>(true);
			foreach (var node in nodes)
			{
				node.GetChild("CollisionDebugBox", false)?.Remove();
				if (!add)
					continue;
				var subNode = node.CreateChild("CollisionDebugBox");
				var box = subNode.CreateComponent<Box>();
				subNode.Scale = node.GetComponent<CollisionShape>().WorldBoundingBox.Size;
				box.Color = new Color(Color.Red, 0.4f);
			}
		}

		async void CreateScene()
		{
			scene = new Scene();
			scene.CreateComponent<Octree>();

			var physics = scene.CreateComponent<PhysicsWorld>();
			physics.SetGravity(new Vector3(0, 0, 0));

			// Camera
			var cameraNode = scene.CreateChild();
			cameraNode.Position = (new Vector3(0.0f, 0.0f, -10.0f));
			cameraNode.CreateComponent<Camera>();
			Viewport = new Viewport(Context, scene, cameraNode.GetComponent<Camera>(), null);

			if (Platform != Platforms.Android && Platform != Platforms.iOS)
			{
				RenderPath effectRenderPath = Viewport.RenderPath.Clone();
				var fxaaRp = ResourceCache.GetXmlFile(Assets.PostProcess.FXAA3);
				effectRenderPath.Append(fxaaRp);
				Viewport.RenderPath = effectRenderPath;
			}
			Renderer.SetViewport(0, Viewport);

			var zoneNode = scene.CreateChild();
			var zone = zoneNode.CreateComponent<Zone>();
			zone.SetBoundingBox(new BoundingBox(-300.0f, 300.0f));
			zone.AmbientColor = new Color(1f, 1f, 1f);
			
			// UI
			coinsText = new Text();
			coinsText.HorizontalAlignment = HorizontalAlignment.Right;
			coinsText.SetFont(ResourceCache.GetFont(Assets.Fonts.Font), Graphics.Width / 20);
			UI.Root.AddChild(coinsText);
			Input.SetMouseVisible(true, false);
            

			// Background
			var background = new Background();
			scene.AddComponent(background);
			background.Start();

			// Lights:
			var lightNode = scene.CreateChild();
			lightNode.Position = new Vector3(0, -5, -40);
			lightNode.AddComponent(new Light { Range = 120, Brightness = 0.8f });

			// Game logic cycle
			bool firstCycle = true;
			while (true)
			{
                //
				StartMenu = scene.CreateComponent<StartMenu>();
                NewStartMenu?.Invoke(StartMenu);
                if (!firstCycle)
                {
                    FinishedGame?.Invoke(Coins);
                }
				await StartMenu.ShowStartMenu(!firstCycle); //wait for "start"
				StartMenu.Remove();
                RemovedStartMenu?.Invoke(StartMenu);

                await StartGame();
				firstCycle = false;
			}
		}

		async Task StartGame()
		{
			UpdateCoins(0);
			Player = new Player();
			var aircraftNode = scene.CreateChild(nameof(Aircraft));
			aircraftNode.AddComponent(Player);
			var playersLife = Player.Play();
			Enemies enemies = new Enemies(Player);
			scene.AddComponent(enemies);
			SpawnCoins();
			enemies.StartSpawning();
			await playersLife;
			enemies.KillAll();
			aircraftNode.Remove();
		}
		
		async void SpawnCoins()
		{
			var player = Player;
			while (Player.IsAlive && player == Player)
			{
				var coinNode = scene.CreateChild();
				coinNode.Position = new Vector3(RandomHelper.NextRandom(-2.5f, 2.5f), 5f, 0);
				var coin = new Apple();
				coinNode.AddComponent(coin);
				await coin.FireAsync(false);
				await scene.RunActionsAsync(new DelayTime(3f));
				coinNode.Remove();
			}
		}

		public void OnCoinCollected() => UpdateCoins(Coins + 1);

		void UpdateCoins(int amount)
		{
			if (amount % 5 == 0 && amount > 0)
			{
				// give player a MassMachineGun each time he earns 5 coins
				Player.Node.AddComponent(new MassMachineGun());
			}
			Coins = amount;
			coinsText.Value = string.Format(CoinstFormat, Coins);
		}
	}
}
