using UnityEngine;
using UnityEngine.InputSystem;

namespace Managers
{
    [DefaultExecutionOrder(-100)]
    public class InputManager : Singleton<InputManager>
    {
        private TouchInputController _touchInputController;
        
        public delegate void StartTouchEvent(Vector2 position, float time);
        public event StartTouchEvent OnStartTouch;
        
        public delegate void EndTouchEvent(Vector2 position, float time);
        public event EndTouchEvent OnEndTouch;

        public void Initialize()
        {
            _touchInputController = new TouchInputController();
            InitializeCallBack();
            
            EnableTouch();
        }

        private void EnableTouch()
        {
            _touchInputController.Enable();
        }
    
        private void DisableTouch()
        {
            _touchInputController.Disable();
        }

        private void InitializeCallBack()
        {
            _touchInputController.Touch.TouchPress.started += TouchPressStarted;
            _touchInputController.Touch.TouchPress.canceled += TouchPressCanceled;

        }

        private void TouchPressCanceled(InputAction.CallbackContext ctx)
        {
            //Debug.Log("Touch Press Canceled");
            if(OnEndTouch != null)
                OnEndTouch(_touchInputController.Touch.TouchPosition.ReadValue<Vector2>(), (float)ctx.startTime);
        }

        private void TouchPressStarted(InputAction.CallbackContext ctx)
        {
            //Debug.Log("Touch Press Started" + _touchInputController.Touch.TouchPosition.ReadValue<Vector2>());
            if(OnStartTouch != null)
                OnStartTouch(_touchInputController.Touch.TouchPosition.ReadValue<Vector2>(), (float)ctx.startTime);
        }
        
        public Vector2 GetTouchPosition()
        {
            return _touchInputController.Touch.TouchPosition.ReadValue<Vector2>();
        }
    }
}
