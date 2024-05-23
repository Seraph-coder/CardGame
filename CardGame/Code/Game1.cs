using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace CardGame.Code
{
    public class Game1 : Game
    {
        private readonly GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private SpriteFont _font;
        private Texture2D _buttonTexture;
        private Texture2D _cardTexture;
        private Rectangle _buttonRectangle;
        private Rectangle _rulesButtonRectangle;
        private bool _showCardSelection;
        private bool _showRules;
        private bool _gameOver;
        private bool _victory;
        private TimeSpan _endGameTimer;
        private readonly TimeSpan _endGameDuration = TimeSpan.FromSeconds(2);

        private GameObjects.Character _player;
        private GameObjects.Character _enemy;
        private Random _random;
        private bool _playerTurn;
        private bool _enemyTurn;
        private bool _buttonClicked;
        private TimeSpan _enemyTurnDelay;
        private const float EnemyTurnDelaySeconds = 1f;
        private int _enemiesDefeated;

        private Card[] _cards;
        private Card[] _currentCards;
        private Rectangle[] _cardRectangles;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            _player = new GameObjects.Character(accuracy: 35, evasion: 35, attack: 10, health: 100, maxHealth: 100);
            _enemy = new GameObjects.Character(accuracy: 25, evasion: 25, attack: 10, health: 100, maxHealth: 100);
            _random = new Random();
            _playerTurn = true;
            _enemyTurn = false;
            _buttonClicked = false;
            _enemyTurnDelay = TimeSpan.Zero;
            _enemiesDefeated = 0;
            _showCardSelection = false;
            _showRules = false;
            _gameOver = false;
            _victory = false;
            _endGameTimer = TimeSpan.Zero;

            _cards = Card.GenerateCards();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _font = Content.Load<SpriteFont>("Font");
            _buttonTexture = CreateTexture(Color.Gray, 100, 50);
            _cardTexture = CreateTexture(Color.White, 80, 120);

            _buttonRectangle = new Rectangle(GraphicsDevice.Viewport.Width / 2 - 50, GraphicsDevice.Viewport.Height / 2 - 25, 100, 50);
            _rulesButtonRectangle = new Rectangle(GraphicsDevice.Viewport.Width - 150, GraphicsDevice.Viewport.Height - 50, 100, 30);
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            if (_endGameTimer > TimeSpan.Zero)
            {
                _endGameTimer -= gameTime.ElapsedGameTime;
            }
            else
            {
                if (_showRules)
                {
                    if (Mouse.GetState().LeftButton == ButtonState.Pressed && !_rulesButtonRectangle.Contains(Mouse.GetState().Position))
                    {
                        _showRules = false;
                    }
                }
                else
                {
                    if (_gameOver || _victory)
                    {
                        if (Mouse.GetState().LeftButton == ButtonState.Pressed && _buttonRectangle.Contains(Mouse.GetState().Position))
                        {
                            RestartGame();
                        }
                    }
                    else
                    {
                        if (_showCardSelection)
                        {
                            HandleCardSelection();
                        }
                        else
                        {
                            HandleGameplay(gameTime);
                        }
                    }
                }
            }

            var mouseState = Mouse.GetState();
            if (mouseState.LeftButton == ButtonState.Pressed && _rulesButtonRectangle.Contains(mouseState.Position))
            {
                _showRules = true;
            }

            base.Update(gameTime);
        }

        private void HandleCardSelection()
        {
            var mouseState = Mouse.GetState();
            for (var i = 0; i < _cardRectangles.Length; i++)
            {
                if (_cardRectangles[i].Contains(mouseState.Position) && mouseState.LeftButton == ButtonState.Pressed)
                {
                    _player.IncreaseStats(_currentCards[i]);
                    _showCardSelection = false;
                    break;
                }
            }
        }

        private void HandleGameplay(GameTime gameTime)
        {
            if (Mouse.GetState().LeftButton == ButtonState.Pressed && _buttonRectangle.Contains(Mouse.GetState().Position) && !_buttonClicked)
            {
                if (_playerTurn)
                {
                    _player.Turn(_enemy, _random);
                    _playerTurn = false;
                    _enemyTurn = true;
                    _buttonClicked = true;
                }
            }
            else if (Mouse.GetState().LeftButton == ButtonState.Released)
            {
                _buttonClicked = false;
            }

            if (_player.Health <= 0)
            {
                _gameOver = true;
                _player.Health = 0;
                _endGameTimer = _endGameDuration;
            }
            else if (_enemy.Health <= 0)
            {
                _enemiesDefeated++;
                if (_enemiesDefeated >= 10)
                {
                    _victory = true;
                    _endGameTimer = _endGameDuration;
                }
                else
                {
                    _enemy = new GameObjects.Character(accuracy: 25 + (_enemiesDefeated * 5), evasion: 25 + (_enemiesDefeated * 5), attack: 10 + _enemiesDefeated * 1, health: 100 + (_enemiesDefeated * 10), maxHealth: 100 + (_enemiesDefeated * 10));
                    _player.Health = _player.MaxHealth;
                    _currentCards = _cards.OrderBy(x => _random.Next()).Take(3).ToArray();
                    _cardRectangles = new Rectangle[3];
                    for (var i = 0; i < 3; i++)
                    {
                        _cardRectangles[i] = new Rectangle(GraphicsDevice.Viewport.Width / 2 - 195 + i * 150, GraphicsDevice.Viewport.Height / 2 + 50, 80, 120);
                    }
                    _showCardSelection = true;
                }
            }
            else
            {
                if (_enemyTurn)
                {
                    _enemyTurnDelay += gameTime.ElapsedGameTime;
                    if (_enemyTurnDelay >= TimeSpan.FromSeconds(EnemyTurnDelaySeconds))
                    {
                        _enemy.Turn(_player, _random);
                        _enemyTurn = false;
                        _playerTurn = true;
                        _enemyTurnDelay = TimeSpan.Zero;
                    }
                }
            }
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin();

            if (_showRules)
            {
                DrawRules();
            }
            else
            {
                if (_gameOver)
                {
                    DrawGameOver();
                }
                else if (_victory)
                {
                    DrawVictory();
                }
                else
                {
                    if (_showCardSelection)
                    {
                        DrawCardSelection();
                    }
                    else
                    {
                        DrawGameplay();
                    }
                }
            }

            _spriteBatch.End();

            base.Draw(gameTime);
        }

        private void DrawGameplay()
        {
            _spriteBatch.DrawString(_font, $"Attack: {_player.Attack}", new Vector2(10, GraphicsDevice.Viewport.Height - 100), Color.White);
            _spriteBatch.DrawString(_font, $"Accuracy: {_player.Accuracy}", new Vector2(10, GraphicsDevice.Viewport.Height - 80), Color.White);
            _spriteBatch.DrawString(_font, $"Evasion: {_player.Evasion}", new Vector2(10, GraphicsDevice.Viewport.Height - 60), Color.White);
            _spriteBatch.DrawString(_font, $"Health: {_player.Health}/{_player.MaxHealth}", new Vector2(10, GraphicsDevice.Viewport.Height - 40), Color.White);

            _spriteBatch.DrawString(_font, $"Enemy - Attack: {_enemy.Attack}", new Vector2(10, 20), Color.White);
            _spriteBatch.DrawString(_font, $"Enemy - Health: {_enemy.Health}/{_enemy.MaxHealth}", new Vector2(10, 40), Color.White);
            _spriteBatch.DrawString(_font, $"Enemy - Accuracy: {_enemy.Accuracy}", new Vector2(10, 60), Color.White);
            _spriteBatch.DrawString(_font, $"Enemy - Evasion: {_enemy.Evasion}", new Vector2(10, 80), Color.White);

            _spriteBatch.Draw(_buttonTexture, _buttonRectangle, Color.White);

            _spriteBatch.DrawString(_font, $"Enemies defeated: {_enemiesDefeated}", new Vector2(GraphicsDevice.Viewport.Width / 2, 10), Color.White);

            _spriteBatch.Draw(_buttonTexture, _rulesButtonRectangle, Color.White);
            _spriteBatch.DrawString(_font, "Rules", new Vector2(_rulesButtonRectangle.X + 10, _rulesButtonRectangle.Y + 5), Color.Black);
        }

        private void DrawCardSelection()
        {
            for (var i = 0; i < _currentCards.Length; i++)
            {
                _spriteBatch.Draw(_cardTexture, _cardRectangles[i], Color.White);
                _spriteBatch.DrawString(_font, $"Attack: {_currentCards[i].Attack}", new Vector2(_cardRectangles[i].X, _cardRectangles[i].Y), Color.Black);
                _spriteBatch.DrawString(_font, $"Health: {_currentCards[i].Health}", new Vector2(_cardRectangles[i].X, _cardRectangles[i].Y + 20), Color.Black);
                _spriteBatch.DrawString(_font, $"Accuracy: {_currentCards[i].Accuracy}", new Vector2(_cardRectangles[i].X, _cardRectangles[i].Y + 40), Color.Black);
                _spriteBatch.DrawString(_font, $"Evasion: {_currentCards[i].Evasion}", new Vector2(_cardRectangles[i].X, _cardRectangles[i].Y + 60), Color.Black);
            }
        }

        private void DrawGameOver()
        {
            var message = "Game Over";
            var position = new Vector2(GraphicsDevice.Viewport.Width / 2 - _font.MeasureString(message).X / 2, GraphicsDevice.Viewport.Height / 2 - _font.MeasureString(message).Y / 2);
            _spriteBatch.DrawString(_font, message, position, Color.Red);
            if (_endGameTimer <= TimeSpan.Zero)
            {
                _spriteBatch.Draw(_buttonTexture, _buttonRectangle, Color.White);
                _spriteBatch.DrawString(_font, "Restart", new Vector2(_buttonRectangle.X + 30, _buttonRectangle.Y + 10), Color.Black);
            }
        }

        private void DrawVictory()
        {
            const string message = "You Win!";
            var position = new Vector2(GraphicsDevice.Viewport.Width / 2 - _font.MeasureString(message).X / 2, GraphicsDevice.Viewport.Height / 2 - _font.MeasureString(message).Y / 2);
            _spriteBatch.DrawString(_font, message, position, Color.Green);
            if (_endGameTimer <= TimeSpan.Zero)
            {
                _spriteBatch.Draw(_buttonTexture, _buttonRectangle, Color.White);
                _spriteBatch.DrawString(_font, "Restart", new Vector2(_buttonRectangle.X + 30, _buttonRectangle.Y + 10), Color.Black);
            }
        }

        private void DrawRules()
        {
            var rulesBackground = new Rectangle(100, 100, 600, 400);
            _spriteBatch.Draw(_buttonTexture, rulesBackground, Color.LightGray);

            const string rulesText = "Players and enemies take turns to perform actions." +
                                     "\r\nDuring their turn, players can choose to attack the enemy " +
                                     "\r\n  or use items, " +
                                     "\r\nOnce the player's turn ends, the enemy takes its turn and vice" +
                                     "\r\n  versa." +
                                     "\r\nThe game continues in this turn-based fashion until either the" +
                                     "\r\n  player defeats 10 enemies or the player's health drops to zero, " +
                                     "\r\n  resulting in a game over." +
                                     "\r\nAccuracy and evasion affect the damage you deal and take, " +
                                     "\r\n  don't underestimate them";

            var rulesPosition = new Vector2(120, 120);
            _spriteBatch.DrawString(_font, rulesText, rulesPosition, Color.Black);
        }

        private void RestartGame()
        {
            _player = new GameObjects.Character(accuracy: 35, evasion: 35, attack: 10, health: 100, maxHealth: 100);
            _enemy = new GameObjects.Character(accuracy: 25, evasion: 25, attack: 10, health: 100, maxHealth: 100);
            _random = new Random();
            _playerTurn = true;
            _enemyTurn = false;
            _buttonClicked = false;
            _enemyTurnDelay = TimeSpan.Zero;
            _enemiesDefeated = 0;
            _showCardSelection = false;
            _showRules = false;
            _gameOver = false;
            _victory = false;
            _endGameTimer = TimeSpan.Zero;

            _cards = Card.GenerateCards();
        }

        private Texture2D CreateTexture(Color color, int width, int height)
        {
            var texture = new Texture2D(GraphicsDevice, width, height);
            var data = new Color[width * height];

            for (var i = 0; i < data.Length; i++)
            {
                data[i] = color;
            }

            texture.SetData(data);
            return texture;
        }
    }
}
