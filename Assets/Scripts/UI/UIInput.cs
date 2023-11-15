
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

public class UIInput : Singleton<UIInput>
{
    [SerializeField] PlayerInput playerInput;
    Selectable selectedUIObject;

    InputSystemUIInputModule UIInputModule;

    protected override void Awake()
    {
        base.Awake();
        UIInputModule = GetComponent<InputSystemUIInputModule>();
        UIInputModule.enabled = false;

        playerInput.onConfrimByController += OnConfirmByController;
    }

    void OnDestroy()
    {
        playerInput.onConfrimByController -= OnConfirmByController;
    }


    public void SelectUI(Selectable UIObject)
    {
        UIObject.Select();
        UIObject.OnSelect(null);
        UIInputModule.enabled = true;

        selectedUIObject = UIObject;
    }

    void OnConfirmByController()
    {
        // 当事件发生时，点击选中的UI对象
        if (selectedUIObject != null)
        {
            ExecuteEvents.Execute(selectedUIObject.gameObject, new BaseEventData(EventSystem.current), ExecuteEvents.submitHandler);
        }
    }

    public void DisableAllUIInputs()
    {
        if (playerInput == null)
        {
            Debug.LogWarning("PlayerInput component is not assigned in UIInput.");
            return;
        }
        playerInput.DisableAllInputs();
        UIInputModule.enabled = false;
    }
}