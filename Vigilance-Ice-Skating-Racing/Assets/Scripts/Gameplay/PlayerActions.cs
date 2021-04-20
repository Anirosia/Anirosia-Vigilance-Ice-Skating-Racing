// GENERATED AUTOMATICALLY FROM 'Assets/Scripts/Gameplay/PlayerActions.inputactions'

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

namespace Gameplay
{
    public class @PlayerActions : IInputActionCollection, IDisposable
    {
        public InputActionAsset asset { get; }
        public @PlayerActions()
        {
            asset = InputActionAsset.FromJson(@"{
    ""name"": ""PlayerActions"",
    ""maps"": [
        {
            ""name"": ""Gameplay"",
            ""id"": ""24cbe63b-11ac-4c92-ae74-1f3233cce0fb"",
            ""actions"": [
                {
                    ""name"": ""PrimaryContact"",
                    ""type"": ""PassThrough"",
                    ""id"": ""fa0b06ba-45ad-4de8-b219-522268390288"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": ""Press""
                },
                {
                    ""name"": ""PrimaryPosition"",
                    ""type"": ""Value"",
                    ""id"": ""15a28016-af8e-4e5b-8c52-82674c887b43"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""ed98222e-c9ef-49f7-9467-442e4efebec3"",
                    ""path"": ""<Touchscreen>/primaryTouch/press"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Touch"",
                    ""action"": ""PrimaryContact"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""4ce48d18-8b43-46ea-8c23-8285551b6dc1"",
                    ""path"": ""<Touchscreen>/primaryTouch/position"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Touch"",
                    ""action"": ""PrimaryPosition"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": [
        {
            ""name"": ""Touch"",
            ""bindingGroup"": ""Touch"",
            ""devices"": [
                {
                    ""devicePath"": ""<Touchscreen>"",
                    ""isOptional"": false,
                    ""isOR"": false
                }
            ]
        }
    ]
}");
            // Gameplay
            m_Gameplay = asset.FindActionMap("Gameplay", throwIfNotFound: true);
            m_Gameplay_PrimaryContact = m_Gameplay.FindAction("PrimaryContact", throwIfNotFound: true);
            m_Gameplay_PrimaryPosition = m_Gameplay.FindAction("PrimaryPosition", throwIfNotFound: true);
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

        // Gameplay
        private readonly InputActionMap m_Gameplay;
        private IGameplayActions m_GameplayActionsCallbackInterface;
        private readonly InputAction m_Gameplay_PrimaryContact;
        private readonly InputAction m_Gameplay_PrimaryPosition;
        public struct GameplayActions
        {
            private @PlayerActions m_Wrapper;
            public GameplayActions(@PlayerActions wrapper) { m_Wrapper = wrapper; }
            public InputAction @PrimaryContact => m_Wrapper.m_Gameplay_PrimaryContact;
            public InputAction @PrimaryPosition => m_Wrapper.m_Gameplay_PrimaryPosition;
            public InputActionMap Get() { return m_Wrapper.m_Gameplay; }
            public void Enable() { Get().Enable(); }
            public void Disable() { Get().Disable(); }
            public bool enabled => Get().enabled;
            public static implicit operator InputActionMap(GameplayActions set) { return set.Get(); }
            public void SetCallbacks(IGameplayActions instance)
            {
                if (m_Wrapper.m_GameplayActionsCallbackInterface != null)
                {
                    @PrimaryContact.started -= m_Wrapper.m_GameplayActionsCallbackInterface.OnPrimaryContact;
                    @PrimaryContact.performed -= m_Wrapper.m_GameplayActionsCallbackInterface.OnPrimaryContact;
                    @PrimaryContact.canceled -= m_Wrapper.m_GameplayActionsCallbackInterface.OnPrimaryContact;
                    @PrimaryPosition.started -= m_Wrapper.m_GameplayActionsCallbackInterface.OnPrimaryPosition;
                    @PrimaryPosition.performed -= m_Wrapper.m_GameplayActionsCallbackInterface.OnPrimaryPosition;
                    @PrimaryPosition.canceled -= m_Wrapper.m_GameplayActionsCallbackInterface.OnPrimaryPosition;
                }
                m_Wrapper.m_GameplayActionsCallbackInterface = instance;
                if (instance != null)
                {
                    @PrimaryContact.started += instance.OnPrimaryContact;
                    @PrimaryContact.performed += instance.OnPrimaryContact;
                    @PrimaryContact.canceled += instance.OnPrimaryContact;
                    @PrimaryPosition.started += instance.OnPrimaryPosition;
                    @PrimaryPosition.performed += instance.OnPrimaryPosition;
                    @PrimaryPosition.canceled += instance.OnPrimaryPosition;
                }
            }
        }
        public GameplayActions @Gameplay => new GameplayActions(this);
        private int m_TouchSchemeIndex = -1;
        public InputControlScheme TouchScheme
        {
            get
            {
                if (m_TouchSchemeIndex == -1) m_TouchSchemeIndex = asset.FindControlSchemeIndex("Touch");
                return asset.controlSchemes[m_TouchSchemeIndex];
            }
        }
        public interface IGameplayActions
        {
            void OnPrimaryContact(InputAction.CallbackContext context);
            void OnPrimaryPosition(InputAction.CallbackContext context);
        }
    }
}
