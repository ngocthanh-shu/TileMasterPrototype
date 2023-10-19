using System;
using System.Collections.Generic;
using Tiles;
using UnityEngine;
using Utils;

namespace Managers
{
    public class GameManager : MonoBehaviour
    {
        public Action OnGameStart;
        public Action BackToMenu;
        public Action<GameObject> RefreshTilePositionAction;
        public Action<TileController> SelectedTileAction;
        public Action<GameState> GameStateChangedAction;
        public Action OnStarScoreChanged;
        public Action OnGameStateChanged;
        
        public UIManager uiManager;
        public PoolManager poolManager;
        public TileManager tileManager;
        public InputManager inputManager;
        public TouchableManager touchableManager;
        public LevelManager levelManager;
        public AudioManager audioManager;
        public ResourceManager resourceManager;
        
        public PlayableArea playableArea;
        public Collider background;
        public Collider stackArea;
        public Collider tileDefaultCollider;
        public int stackNumber;

        private GameState _gameState;

        private List<TileInformation> _currentLevel;
        
        private int _totalStarScore;
        private int _starScore;
        private int _level;
        private int _time;
        private float _realTime;
        private AudioSource _audioSource;

        private void Initialize()
        {
            if(uiManager == null)
                uiManager = UIManager.Instance;
            if(poolManager == null)
                poolManager = PoolManager.Instance;
            if(tileManager == null)
                tileManager = TileManager.Instance;
            if(inputManager == null)
                inputManager = InputManager.Instance;
            if(touchableManager == null)
                touchableManager = TouchableManager.Instance;
            if(audioManager == null)
                audioManager = AudioManager.Instance;
            if(resourceManager == null)
                resourceManager = ResourceManager.Instance;

            poolManager.Initialize();
            inputManager.Initialize();
            levelManager.Initialize(this);
            uiManager.Initialize(this);
            playableArea.Initialize(this);
            tileManager.Initialize(this, background, playableArea, stackArea, tileDefaultCollider, stackNumber);
            touchableManager.Initialize(this);
            resourceManager.Initialize();
            audioManager.Initialize();
            
            
            InitializeAction();
        }

        private void InitializeAction()
        {
            OnGameStart += GameStart;
            BackToMenu += ReturnToMenu;
            OnStarScoreChanged += StarChanged;
            OnGameStateChanged += UpdateGameState;
            GameStateChangedAction += SetGameState;
            
            RefreshTilePositionAction += tileManager.RefreshTilePosition;
            SelectedTileAction += tileManager.AddTileIntoWait;
            inputManager.OnStartTouch += touchableManager.StartTouch;
            inputManager.OnEndTouch += touchableManager.EndTouch;
        }
        
        public GameState GetGameState()
        {
            return _gameState;
        }
        
        public void ResetLevel()
        {
            _level = 1;
        }
        
        private void SetGameState(GameState obj)
        {
            _gameState = obj;
        }

        private void StarChanged()
        {
            _starScore++;
            uiManager.UpdateStarScore(_starScore);
        }

        private void UpdateGameState()
        {
            if (tileManager.GetCurrentTileActive() == 0)
            {
                _totalStarScore += _starScore;
                _level += 1;
                TurnOnMusic("Win");
                uiManager.UpdateLevel(_level);
                uiManager.ShowWinDialog(_starScore);
            }

            if (tileManager.GetTileCount() >= stackNumber)
            {
                tileManager.EnableColliderTrigger();
                tileManager.ResetStack();
                TurnOnMusic("Lose");
                uiManager.ShowLoseDialog();
            }
        }

        private void ReturnToMenu()
        {
            _gameState = GameState.NonGame;
            poolManager.DisableAll();
            TurnOnMusic("Menu");
            uiManager.ShowMenuUI(_totalStarScore);
        }

        private void GameStart()
        {
            tileManager.EnableColliderTrigger();
            tileManager.ResetStack();
            poolManager.DisableAll();
            uiManager.ShowGameUI();
            _currentLevel = levelManager.GetLevel(_level).tiles;
            tileManager.GenerateTiles(_currentLevel);
            _time = levelManager.GetLevel(_level).time + 1;
            _starScore = 0;
            _realTime = 0;
            uiManager.UpdateStarScore(_starScore);
            uiManager.UpdateTimer(_time);
            _gameState = GameState.Game;
            Time.timeScale = 1;
            TurnOnMusic("Gameplay");
        }

        void Start()
        {
            Initialize();
            _totalStarScore = 0;
            _starScore = 0;
            _level = 1;
            _audioSource = GetComponent<AudioSource>();
            uiManager.SetData(_starScore, _level);
            uiManager.ShowMenuUI(_totalStarScore);
            resourceManager.LoadAudioData();
            TurnOnMusic("Menu");
            _gameState = GameState.NonGame;
        }

        // Update is called once per frame
        void Update()
        {
            touchableManager.OnUpdate();
            
            _realTime += Time.deltaTime;

            if (_gameState == GameState.Game)
            {
                uiManager.UpdateTimer(_time - _realTime);
            }
        }
        
        private void TurnOnMusic(string key)
        {
            audioManager.PlayAudio(_audioSource, resourceManager.GetDictionaryAudio()[key]);
        }
    }

    public enum GameState
    {
        NonGame,
        Game
    }
}