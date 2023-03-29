using UnityEngine;
using UnityEngine.UI;

namespace FromAPikarmy
{
    public class CreditWindow : MonoBehaviour
    {
        [SerializeField] private Button _closeBtn;

        private void Awake()
        {
            _closeBtn.onClick.AddListener(CloseWindow);
        }

        private void CloseWindow()
        {
            gameObject.SetActive(false);
        }
    }
}
