using System;
using System.Collections;
using Tobii.Gaming;
using Tobii.Gaming.Internal;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;


[RequireComponent(typeof(GazeAware))]
[RequireComponent(typeof(Image))]
[RequireComponent(typeof(Collider))]
public class CustomButton : MonoBehaviour {
    protected GazeAware _gazeAware;
    private Image _image;
    private Collider _collider;
    public bool canInteractBeforeStart = false;
    public bool isDefault = true, isHover, isActive, isConfirmationButton;
    public Color defaultColor, activeColor, defaultTextColor = new Color(33,33,33,1), activeTextColor = new Color(238,238,238,1);
    private Color inactiveHoverColor, activeHoverColor;
    public GameObject confirmScaler;
    public bool mouseOver;
    protected RectTransform _confirmScalerRT;
    public float interactionBreakTime = 1.0f;
    private bool _canHover = true;
    public UnityEvent OnActivation, OnDeactivation;
    public bool activated;
    protected bool _usingEyeTracking;

    [SerializeField] private Image mainButtonImage;
    private bool colorsSet = false;
    private Text buttonText;
    public bool changeTextColor;

    protected virtual void Start()
    {
        buttonText = GetComponentInChildren<Text>();
        _confirmScalerRT = confirmScaler.GetComponent<RectTransform>();
        _gazeAware = GetComponent<GazeAware>();
        _collider = GetComponent<Collider>();
        GetImageComponent();
        mainButtonImage.color = defaultColor;
        
        isDefault = true;
        isActive = false;
        if (isConfirmationButton) ConfirmActivation(false);
        
        
       

        
    }
    
    protected virtual void GetImageComponent()=> mainButtonImage = GetComponent<Image>();

    protected virtual void Update() {
        if (TobiiAPI.IsConnected)
        {
            _usingEyeTracking = true;
            if (_gazeAware.HasGazeFocus) Hover();
            else if (!mouseOver) UnHover();
        }
        
    }

    protected virtual void FixedUpdate()
    {
        if (!MasterManager.Instance.isInPosition && !canInteractBeforeStart) return;
        if (isHover) {
            if (_confirmScalerRT.localScale.x < 1.0f)
                
                    _confirmScalerRT.localScale += Vector3.one / MasterManager.Instance.dwellTimeSpeed;

            else {
                _confirmScalerRT.localScale = Vector3.zero;
             
                if (!isActive) { 
                    StartCoroutine(InteractionBreakTime());
                    SetActive();
                    confirmScaler.GetComponent<Image>().color = defaultColor;
                    OnActivation?.Invoke();
                  
                }
                else {
                    StartCoroutine(InteractionBreakTime());
                    SetDefault();
                    confirmScaler.GetComponent<Image>().color = activeColor;
                    OnDeactivation?.Invoke();
               
                }
            }
        }

        else {
            if (_confirmScalerRT.localScale.x < 0.0f) return;
            
                _confirmScalerRT.localScale -= Vector3.one / MasterManager.Instance.dwellTimeSpeed;
            
        }
    }
    protected virtual void OnMouseOver()
    {
        if (!MasterManager.Instance.isInPosition && !canInteractBeforeStart) return;
        if(gameObject.layer != LayerMask.NameToLayer("RenderPanel")) return;
        mouseOver = true;
        Hover();
    }

    protected virtual void OnMouseExit() {
        if (!MasterManager.Instance.isInPosition && !canInteractBeforeStart) return;
        if(gameObject.layer != LayerMask.NameToLayer("RenderPanel")) return;
        mouseOver = false;
        UnHover();
    }

    protected virtual void SetActive()
    {
        if(changeTextColor) buttonText.color = activeTextColor;
        mainButtonImage.color = activeColor;
        if (isActive) return;
        isActive = true;
        isDefault = false;
    }

    protected virtual void Hover() {
        if (!colorsSet)
        {
            Color.RGBToHSV(defaultColor, out var uH, out var uS, out var uV);
            uV -= 0.3f;
            Color.RGBToHSV(activeColor, out var aH, out var aS, out var aV);
            aV -= 0.3f;

            inactiveHoverColor = Color.HSVToRGB(uH, uS, uV);
            activeHoverColor = Color.HSVToRGB(aH, aS, aV);

            inactiveHoverColor.a = 1;
            activeHoverColor.a = 1;
            confirmScaler.GetComponent<Image>().color = activeColor;
            colorsSet = true;
        }

        if (!_canHover || gameObject.layer != LayerMask.NameToLayer("RenderPanel")) return;
        if (isActive) mainButtonImage.color = activeHoverColor;
        else mainButtonImage.color = inactiveHoverColor;
        if (isHover) return;
        isHover = true;
    }

    protected virtual void UnHover() {
        if (!isHover) return;
        isHover = false;
        if (isConfirmationButton) SetDefault();
        else if (isDefault) SetDefault();
        else if (isActive) SetActive();
    }

    protected virtual void SetDefault() {
         mainButtonImage.color = defaultColor;
        if(changeTextColor)buttonText.color = defaultTextColor;
        confirmScaler.GetComponent<Image>().color = activeColor;
        if (isDefault) return;
        isDefault = true;
        isActive = false;
    }

    protected virtual void ConfirmActivation(bool enabled) {
        _collider.enabled = enabled;
        mainButtonImage.enabled = enabled;
    }
    

    protected virtual IEnumerator InteractionBreakTime() {
        _canHover = false;
        isHover = false;
        yield return new WaitForSeconds(interactionBreakTime);
        _canHover = true;
    }
}