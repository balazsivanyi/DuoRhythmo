using System;
using System.Collections;
using System.Collections.Generic;
using DidStuffLab;
using Mirror;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.VFX;

namespace Managers {
    public class CarouselManager : MonoBehaviour {
        private Animator _uiAnimator;
        private float _timeLeft = 10.0f;
        private Color _targetVFXColor, _targetSkyColor;
        public VisualEffect _vfx;
        private int _currentPanel = 0, _lastPanel = 0, _currentPanelBuddy = 5, _lastPanelBuddy = 5;
        private static readonly int Tint = Shader.PropertyToID("_Tint");
        public float currentRotationOfUI = 0.0f;
        [SerializeField] private int[] panelsToStartUnblurred = new int[2];

        public List<GameObject>
            panels = new List<GameObject>(10); //put panels in list so we can change their layer to blur or render them over blur

        [SerializeField] private ForwardRendererData _forwardRenderer;
        private bool isRenderingAPanel = false;
        public bool justJoined;
        public bool isSoloMode = false;

        [SerializeField] private NavigationVoteSync[] _navigationVoteSyncs;
        public delegate void MoveCarouselAction();
        public static event MoveCarouselAction OnMovedCarousel;

        private EmojiSync _emojiSync;

        public EmojiSync EmojiSync {
            get => _emojiSync;
            set {
                _emojiSync = value;
                for (byte index = 0; index < emojiSpawners.Length; index++) {
                    if(emojiParticles.ContainsKey(index)) break; // stop loop, cause it's already been iterated
                    var es = emojiSpawners[index];
                    emojiParticles.Add(index, es.GetComponentsInChildren<ParticleSystem>());
                }
            }
        }
        public GameObject [] emojiSpawners;
        public Dictionary<byte, ParticleSystem[]> emojiParticles { get; private set; } = new Dictionary<byte, ParticleSystem[]>();
        
        private void OnEnable() {
            _forwardRenderer.rendererFeatures[0].SetActive(true);
        }

        private void Start() {
            _vfx.transform.gameObject.SetActive(false);
            _targetVFXColor = MasterManager.Instance.drumColors[0];
            if (justJoined) return;
        }

        public void InitialiseBlur() {
            for (int i = 0; i < panelsToStartUnblurred.Length; i++)
            {
                foreach (var t in panels[panelsToStartUnblurred[i]].GetComponentsInChildren<Transform>()) {
                    t.gameObject.layer =
                        LayerMask.NameToLayer("RenderPanel"); //Change their layer to default so they are blurred
                }
            }
        }

        public void ToggleVFX(bool activate) {
            if (_vfx.transform.gameObject.activeInHierarchy == activate) return;
            _vfx.transform.gameObject.SetActive(activate);
        }

        public void PauseAnimation() {
            print("Pause animation");
            MasterManager.Instance.ForceSoloOffGlobal();
            BlurBackground();
        }

        public void BlurSolo(bool forward) {
            _lastPanel = _currentPanel;
            _lastPanelBuddy = _currentPanelBuddy;
            if (forward) {
                if (_currentPanel < panels.Count - 1) _currentPanel++;
                else _currentPanel = 0;
            }
            else {
                if (_currentPanel > 0) _currentPanel--;
                else _currentPanel = panels.Count - 1;
            }
        }

        public void SwapBlurForPlayer(int current, int last, int currentBuddy, int lastBuddy) {
            _currentPanel = current;
            _lastPanel = last;
            _currentPanelBuddy = currentBuddy;
            _lastPanelBuddy = lastBuddy;
        }

        public void BlurFromServer(bool forward) {
            // TODO --> when player swaps position --> currentPosition = 5, lastPosition = 5, currentBuddyPos = 0, lastBuddyPos = 0
            _lastPanel = _currentPanel;
            _lastPanelBuddy = _currentPanelBuddy;
            
            if (forward) {
                if (_currentPanel < panels.Count - 1) _currentPanel++;
                else _currentPanel = 0;
                if (_currentPanelBuddy < panels.Count - 1) _currentPanelBuddy++;
                else _currentPanelBuddy = 0;
            }
            else {
                if (_currentPanel > 0) _currentPanel--;
                else _currentPanel = panels.Count - 1;
                if (_currentPanelBuddy > 0) _currentPanelBuddy--;
                else _currentPanelBuddy = panels.Count - 1;
            }

            foreach (var navsync in _navigationVoteSyncs) {
                if(navsync.VotingValue > 0) navsync.ResetVoting();
            }
            
            OnMovedCarousel?.Invoke();
        }

        public void Update() {
            if(CarouselLerpMove.Instance != null) transform.rotation = CarouselLerpMove.Instance.transform.rotation;
            if (!MasterManager.Instance.gameSetUpFinished || !isSoloMode) return;
            if (_timeLeft <= Time.deltaTime) {
                // transition complete
                // assign the target color
                _vfx.SetVector4("ParticleColor", _targetVFXColor);
                _vfx.SetVector4("Core color", _targetVFXColor);
                // start a new transition
                var index = 0;
                if (isSoloMode) {
                    if (_currentPanel % 2 == 1) index = _currentPanel - 1;
                    else index = _currentPanel;

                    index /= 2;
                }
                else {
                    index = _currentPanel;
                    if (_currentPanel > 4) index = _currentPanel - 5;
                }

                _targetVFXColor = MasterManager.Instance.drumColors[index];
                _timeLeft = 5.0f;
            }
            else {
                // transition in progress
                // calculate interpolated color

                _vfx.SetVector4("ParticleColor",
                    Color.Lerp(_vfx.GetVector4("ParticleColor"), _targetVFXColor, Time.deltaTime / _timeLeft));
                _vfx.SetVector4("Core color",
                    Color.Lerp(_vfx.GetVector4("Core color"), _targetVFXColor, Time.deltaTime / _timeLeft));

                // update the timer
                _timeLeft -= Time.deltaTime;
            }
        }

        public void BlurBackground() {
            foreach (var t in panels[_lastPanel].GetComponentsInChildren<Transform>()) {
                t.gameObject.layer =
                    LayerMask.NameToLayer("Default"); //Change their layer to default so they are blurred
            }

            if (!isSoloMode) {
                foreach (var t in panels[_lastPanelBuddy].GetComponentsInChildren<Transform>()) {
                    t.gameObject.layer =
                        LayerMask.NameToLayer("Default"); //Change their layer to default so they are blurred
                }
            }

            foreach (var t in panels[_currentPanel].GetComponentsInChildren<Transform>()) {
                t.gameObject.layer =
                    LayerMask.NameToLayer("RenderPanel"); //Change their layer to default so they are blurred
            }

            if (!isSoloMode) {
                foreach (var t in panels[_currentPanelBuddy].GetComponentsInChildren<Transform>()) {
                    t.gameObject.layer =
                        LayerMask.NameToLayer("RenderPanel"); //Change their layer to default so they are blurred
                }
            }
        }

        public void UpdateVoteToMoveToServer(bool activate, bool forward) {
            var navSync = forward ? _navigationVoteSyncs[0] : _navigationVoteSyncs[1];
            var offset = (activate ? 1 : -1);
            print("Local player wants to move forward by " + offset + " - " + forward);
            var newValue = navSync.VotingValue + offset;
            if (newValue > 1) {
                // tell the server to lerp move the carousel
                CarouselLerpMove.Instance.UpdateLerpMoveRotation(forward);
            }
            else navSync.ChangeValue((byte) (newValue));
        }

        public void OpenFeedbackSite() => Application.OpenURL("https://duorhythmo.frill.co/b/zv9dw6m1/feature-ideas");

        private void OnDisable() {
            _forwardRenderer.rendererFeatures[0].SetActive(false);
        }
    }
}