// GENERATED AUTOMATICALLY FROM 'Assets/Unity Toolbox/Controls/Controls.inputactions'

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class @Controls : IInputActionCollection, IDisposable
{
    public InputActionAsset asset { get; }
    public @Controls()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""Controls"",
    ""maps"": [
        {
            ""name"": ""Default"",
            ""id"": ""25103d87-94a0-4217-ac5d-2c94f89fff1d"",
            ""actions"": [
                {
                    ""name"": ""Click"",
                    ""type"": ""Button"",
                    ""id"": ""67a84b26-080b-4e01-a793-d55700b18968"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Pointer Position"",
                    ""type"": ""Value"",
                    ""id"": ""eb0aaf61-f42b-46ac-becc-e8b878c478de"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Pointer Delta"",
                    ""type"": ""Value"",
                    ""id"": ""31d967b3-77af-49ec-b048-37fdb9809834"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Movement"",
                    ""type"": ""PassThrough"",
                    ""id"": ""392e7ac0-719f-464a-a73e-e1dac5c81d03"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Jump"",
                    ""type"": ""Button"",
                    ""id"": ""dbf65a34-244f-408b-99ee-716144214197"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Interact"",
                    ""type"": ""Button"",
                    ""id"": ""10b0f2c6-f012-4565-b903-e804e04fc140"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Dash"",
                    ""type"": ""Button"",
                    ""id"": ""8183c0c1-956e-4866-a8da-010db6b9a37c"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Attack"",
                    ""type"": ""Button"",
                    ""id"": ""9c01b47c-a6d6-4fe0-81aa-093f6a3be8d8"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""973864b8-ef40-4697-a8a4-58404ed2d6ad"",
                    ""path"": ""<Mouse>/leftButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Click"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""a5e7a021-7712-4af2-ab40-84aae8f34811"",
                    ""path"": ""<Pointer>/position"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Pointer Position"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""349ab197-0c3c-4b6a-8fb5-5482ee571ac1"",
                    ""path"": ""<Pointer>/delta"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Pointer Delta"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""WASD"",
                    ""id"": ""a7fd0f57-cfac-4332-818b-94c896f4d291"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""98e5ae24-2956-4ce6-a94c-1fa95bc20927"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""49609279-3814-4541-aa9a-c343ce52a1bc"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""9a20d21c-ef9d-40ba-9d73-27e5b24ca340"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""9bd2cf65-10a2-4baa-b3ab-c01ad8cac923"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""DPad"",
                    ""id"": ""a6c69ad1-03ab-4320-92d0-99bc8775974b"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""a098fc8f-a527-44c4-824a-5f3cef1731c8"",
                    ""path"": ""<Gamepad>/dpad/up"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""4c77ef89-7cf0-4ca5-ada4-78ad5042fc3d"",
                    ""path"": ""<Gamepad>/dpad/down"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""0767b2dc-4209-4431-9fdc-68cd4b2fd503"",
                    ""path"": ""<Gamepad>/dpad/left"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""a2f86781-1082-426e-ab97-ad220744bcc3"",
                    ""path"": ""<Gamepad>/dpad/right"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""Arrows"",
                    ""id"": ""a16cce2e-2e6a-4910-b4a7-e95dbd077ff9"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""63a2c80e-108d-40d3-9593-52372f4278a6"",
                    ""path"": ""<Keyboard>/upArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""3f2df6ff-33e9-468c-bc80-526060872675"",
                    ""path"": ""<Keyboard>/downArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""0bee610f-dec7-4507-a667-eeb7f020c193"",
                    ""path"": ""<Keyboard>/leftArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""c7384a85-8614-4c9f-a5fa-f317333578a2"",
                    ""path"": ""<Keyboard>/rightArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""67409e7d-811c-4e27-9cbb-fda88776f546"",
                    ""path"": ""<Keyboard>/space"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Jump"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""e0266fbf-759a-4807-a25b-5422ce94682a"",
                    ""path"": ""<Gamepad>/buttonSouth"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Jump"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""a9d6e5bb-8cb1-413e-8bd9-4f209716889b"",
                    ""path"": ""<Gamepad>/dpad/up"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Interact"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""c641fc34-66ca-49f1-979d-289717ae3d65"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Interact"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""fefc932e-bed6-4db1-83d4-624326ddd8c5"",
                    ""path"": ""<Keyboard>/upArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Interact"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""56b806c5-d9e5-48fd-8508-af6ad8f58171"",
                    ""path"": ""<Keyboard>/shift"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Dash"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""803b9b80-7468-4252-bb46-16437a90e061"",
                    ""path"": ""<Gamepad>/rightTrigger"",
                    ""interactions"": ""Press"",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Dash"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""90343bdb-ddc5-4534-91a6-73653a5740dc"",
                    ""path"": ""<Gamepad>/buttonWest"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Attack"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""4c38d602-86ab-4b2c-9cf5-3032aeadab38"",
                    ""path"": ""<Keyboard>/x"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Attack"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        },
        {
            ""name"": ""Developer"",
            ""id"": ""5427f35f-6471-486d-b76f-a0a0454a0b6c"",
            ""actions"": [
                {
                    ""name"": ""Console Toggle"",
                    ""type"": ""Button"",
                    ""id"": ""35e82c33-b71e-45bf-b589-463988a37b01"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""88ea4db7-cf92-4ee7-bee7-4aeea9636082"",
                    ""path"": ""<Keyboard>/backquote"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Console Toggle"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": []
}");
        // Default
        m_Default = asset.FindActionMap("Default", throwIfNotFound: true);
        m_Default_Click = m_Default.FindAction("Click", throwIfNotFound: true);
        m_Default_PointerPosition = m_Default.FindAction("Pointer Position", throwIfNotFound: true);
        m_Default_PointerDelta = m_Default.FindAction("Pointer Delta", throwIfNotFound: true);
        m_Default_Movement = m_Default.FindAction("Movement", throwIfNotFound: true);
        m_Default_Jump = m_Default.FindAction("Jump", throwIfNotFound: true);
        m_Default_Interact = m_Default.FindAction("Interact", throwIfNotFound: true);
        m_Default_Dash = m_Default.FindAction("Dash", throwIfNotFound: true);
        m_Default_Attack = m_Default.FindAction("Attack", throwIfNotFound: true);
        // Developer
        m_Developer = asset.FindActionMap("Developer", throwIfNotFound: true);
        m_Developer_ConsoleToggle = m_Developer.FindAction("Console Toggle", throwIfNotFound: true);
    }

    public void Dispose()
    {
        UnityEngine.Object.Destroy(asset);
    }

    public InputBinding? bindingMask
    {
        get => asset.bindingMask;
        set => asset.bindingMask = value;
    }

    public ReadOnlyArray<InputDevice>? devices
    {
        get => asset.devices;
        set => asset.devices = value;
    }

    public ReadOnlyArray<InputControlScheme> controlSchemes => asset.controlSchemes;

    public bool Contains(InputAction action)
    {
        return asset.Contains(action);
    }

    public IEnumerator<InputAction> GetEnumerator()
    {
        return asset.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Enable()
    {
        asset.Enable();
    }

    public void Disable()
    {
        asset.Disable();
    }

    // Default
    private readonly InputActionMap m_Default;
    private IDefaultActions m_DefaultActionsCallbackInterface;
    private readonly InputAction m_Default_Click;
    private readonly InputAction m_Default_PointerPosition;
    private readonly InputAction m_Default_PointerDelta;
    private readonly InputAction m_Default_Movement;
    private readonly InputAction m_Default_Jump;
    private readonly InputAction m_Default_Interact;
    private readonly InputAction m_Default_Dash;
    private readonly InputAction m_Default_Attack;
    public struct DefaultActions
    {
        private @Controls m_Wrapper;
        public DefaultActions(@Controls wrapper) { m_Wrapper = wrapper; }
        public InputAction @Click => m_Wrapper.m_Default_Click;
        public InputAction @PointerPosition => m_Wrapper.m_Default_PointerPosition;
        public InputAction @PointerDelta => m_Wrapper.m_Default_PointerDelta;
        public InputAction @Movement => m_Wrapper.m_Default_Movement;
        public InputAction @Jump => m_Wrapper.m_Default_Jump;
        public InputAction @Interact => m_Wrapper.m_Default_Interact;
        public InputAction @Dash => m_Wrapper.m_Default_Dash;
        public InputAction @Attack => m_Wrapper.m_Default_Attack;
        public InputActionMap Get() { return m_Wrapper.m_Default; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(DefaultActions set) { return set.Get(); }
        public void SetCallbacks(IDefaultActions instance)
        {
            if (m_Wrapper.m_DefaultActionsCallbackInterface != null)
            {
                @Click.started -= m_Wrapper.m_DefaultActionsCallbackInterface.OnClick;
                @Click.performed -= m_Wrapper.m_DefaultActionsCallbackInterface.OnClick;
                @Click.canceled -= m_Wrapper.m_DefaultActionsCallbackInterface.OnClick;
                @PointerPosition.started -= m_Wrapper.m_DefaultActionsCallbackInterface.OnPointerPosition;
                @PointerPosition.performed -= m_Wrapper.m_DefaultActionsCallbackInterface.OnPointerPosition;
                @PointerPosition.canceled -= m_Wrapper.m_DefaultActionsCallbackInterface.OnPointerPosition;
                @PointerDelta.started -= m_Wrapper.m_DefaultActionsCallbackInterface.OnPointerDelta;
                @PointerDelta.performed -= m_Wrapper.m_DefaultActionsCallbackInterface.OnPointerDelta;
                @PointerDelta.canceled -= m_Wrapper.m_DefaultActionsCallbackInterface.OnPointerDelta;
                @Movement.started -= m_Wrapper.m_DefaultActionsCallbackInterface.OnMovement;
                @Movement.performed -= m_Wrapper.m_DefaultActionsCallbackInterface.OnMovement;
                @Movement.canceled -= m_Wrapper.m_DefaultActionsCallbackInterface.OnMovement;
                @Jump.started -= m_Wrapper.m_DefaultActionsCallbackInterface.OnJump;
                @Jump.performed -= m_Wrapper.m_DefaultActionsCallbackInterface.OnJump;
                @Jump.canceled -= m_Wrapper.m_DefaultActionsCallbackInterface.OnJump;
                @Interact.started -= m_Wrapper.m_DefaultActionsCallbackInterface.OnInteract;
                @Interact.performed -= m_Wrapper.m_DefaultActionsCallbackInterface.OnInteract;
                @Interact.canceled -= m_Wrapper.m_DefaultActionsCallbackInterface.OnInteract;
                @Dash.started -= m_Wrapper.m_DefaultActionsCallbackInterface.OnDash;
                @Dash.performed -= m_Wrapper.m_DefaultActionsCallbackInterface.OnDash;
                @Dash.canceled -= m_Wrapper.m_DefaultActionsCallbackInterface.OnDash;
                @Attack.started -= m_Wrapper.m_DefaultActionsCallbackInterface.OnAttack;
                @Attack.performed -= m_Wrapper.m_DefaultActionsCallbackInterface.OnAttack;
                @Attack.canceled -= m_Wrapper.m_DefaultActionsCallbackInterface.OnAttack;
            }
            m_Wrapper.m_DefaultActionsCallbackInterface = instance;
            if (instance != null)
            {
                @Click.started += instance.OnClick;
                @Click.performed += instance.OnClick;
                @Click.canceled += instance.OnClick;
                @PointerPosition.started += instance.OnPointerPosition;
                @PointerPosition.performed += instance.OnPointerPosition;
                @PointerPosition.canceled += instance.OnPointerPosition;
                @PointerDelta.started += instance.OnPointerDelta;
                @PointerDelta.performed += instance.OnPointerDelta;
                @PointerDelta.canceled += instance.OnPointerDelta;
                @Movement.started += instance.OnMovement;
                @Movement.performed += instance.OnMovement;
                @Movement.canceled += instance.OnMovement;
                @Jump.started += instance.OnJump;
                @Jump.performed += instance.OnJump;
                @Jump.canceled += instance.OnJump;
                @Interact.started += instance.OnInteract;
                @Interact.performed += instance.OnInteract;
                @Interact.canceled += instance.OnInteract;
                @Dash.started += instance.OnDash;
                @Dash.performed += instance.OnDash;
                @Dash.canceled += instance.OnDash;
                @Attack.started += instance.OnAttack;
                @Attack.performed += instance.OnAttack;
                @Attack.canceled += instance.OnAttack;
            }
        }
    }
    public DefaultActions @Default => new DefaultActions(this);

    // Developer
    private readonly InputActionMap m_Developer;
    private IDeveloperActions m_DeveloperActionsCallbackInterface;
    private readonly InputAction m_Developer_ConsoleToggle;
    public struct DeveloperActions
    {
        private @Controls m_Wrapper;
        public DeveloperActions(@Controls wrapper) { m_Wrapper = wrapper; }
        public InputAction @ConsoleToggle => m_Wrapper.m_Developer_ConsoleToggle;
        public InputActionMap Get() { return m_Wrapper.m_Developer; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(DeveloperActions set) { return set.Get(); }
        public void SetCallbacks(IDeveloperActions instance)
        {
            if (m_Wrapper.m_DeveloperActionsCallbackInterface != null)
            {
                @ConsoleToggle.started -= m_Wrapper.m_DeveloperActionsCallbackInterface.OnConsoleToggle;
                @ConsoleToggle.performed -= m_Wrapper.m_DeveloperActionsCallbackInterface.OnConsoleToggle;
                @ConsoleToggle.canceled -= m_Wrapper.m_DeveloperActionsCallbackInterface.OnConsoleToggle;
            }
            m_Wrapper.m_DeveloperActionsCallbackInterface = instance;
            if (instance != null)
            {
                @ConsoleToggle.started += instance.OnConsoleToggle;
                @ConsoleToggle.performed += instance.OnConsoleToggle;
                @ConsoleToggle.canceled += instance.OnConsoleToggle;
            }
        }
    }
    public DeveloperActions @Developer => new DeveloperActions(this);
    public interface IDefaultActions
    {
        void OnClick(InputAction.CallbackContext context);
        void OnPointerPosition(InputAction.CallbackContext context);
        void OnPointerDelta(InputAction.CallbackContext context);
        void OnMovement(InputAction.CallbackContext context);
        void OnJump(InputAction.CallbackContext context);
        void OnInteract(InputAction.CallbackContext context);
        void OnDash(InputAction.CallbackContext context);
        void OnAttack(InputAction.CallbackContext context);
    }
    public interface IDeveloperActions
    {
        void OnConsoleToggle(InputAction.CallbackContext context);
    }
}
