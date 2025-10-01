using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace WordHopper.UI
{
    public class GuessPanel : MonoBehaviour
    {
        public TMP_InputField inputField;

        private static bool s_isOpen = false;
        private string answer;

        private void Awake()
        {
            gameObject.SetActive(false);
            if (inputField)
                inputField.onSubmit.AddListener(_ => SubmitGuess());
        }

        private void OnDestroy()
        {
            if (inputField) inputField.onSubmit.RemoveAllListeners();
        }

        public void Open(string correctWord)
        {
            // 已開啟：只重新聚焦即可
            if (s_isOpen)
            {
                if (inputField)
                {
                    EventSystem.current?.SetSelectedGameObject(inputField.gameObject);
                    inputField.ActivateInputField();
                    inputField.caretPosition = inputField.text.Length;
                }
                return;
            }

            s_isOpen = true;
            answer = (correctWord ?? string.Empty).ToUpperInvariant();
            gameObject.SetActive(true);

            if (inputField)
            {
                inputField.text = string.Empty;
                inputField.characterValidation = TMP_InputField.CharacterValidation.None;
                inputField.onValidateInput = (string t, int i, char c) =>
                {
                    if (char.IsLetter(c) || c == ' ') return char.ToUpperInvariant(c);
                    return '\0';
                };
                inputField.lineType = TMP_InputField.LineType.SingleLine;
                inputField.interactable = true;
                inputField.readOnly = false;

                EventSystem.current?.SetSelectedGameObject(inputField.gameObject);
                inputField.ActivateInputField();
                inputField.caretPosition = inputField.text.Length;
            }
        }

        private void Update()
        {
            if (!gameObject.activeSelf || inputField == null) return;
            var kb = Keyboard.current; if (kb == null) return;

            if (inputField.isFocused && (kb.enterKey.wasPressedThisFrame || kb.numpadEnterKey.wasPressedThisFrame))
                SubmitGuess();

            if (kb.escapeKey.wasPressedThisFrame)
                Close();
        }

        private void SubmitGuess()
        {
            string guess = inputField.text.Trim();
            if (guess.Equals(answer, System.StringComparison.OrdinalIgnoreCase))
            {
                UIHUD.I?.ShowWin();
                Close();
            }
            else
            {
                UIHUD.I?.LoseLife(1);

                if (UIHUD.I != null && UIHUD.I.Lives <= 0)
                {
                    Close();
                }
                else
                {
                    inputField.text = string.Empty;
                    EventSystem.current?.SetSelectedGameObject(inputField.gameObject);
                    inputField.ActivateInputField();
                    inputField.caretPosition = 0;
                }
            }
        }

        private void Close()
        {
            s_isOpen = false;
            if (gameObject.activeSelf)
                gameObject.SetActive(false);
        }
    }
}