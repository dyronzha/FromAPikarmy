using System;
using UnityEngine;
using UnityEngine.UI;

namespace FromAPikarmy
{
	public class InstructionWindow : MonoBehaviour
	{
		[Serializable]
		private class Instruction
		{
			public Sprite Image;
			public string Text;
		}

		[SerializeField] private Button _nextBtn;
		[SerializeField] private Button _closeBtn;
		[SerializeField] private Image _showImg;
		[SerializeField] private Text _showText;
		[SerializeField] private Instruction[] _instructions;

		private int _currentID;

		private void Awake()
		{
			_nextBtn.onClick.AddListener(NextInstruction);
			_closeBtn.onClick.AddListener(CloseWindow);
		}

		private void NextInstruction()
		{
			_currentID = (_currentID + 1) % _instructions.Length;
			SetContnent(_currentID);
		}

		private void SetContnent(int id)
		{
			var ins = _instructions[id];
			_showImg.sprite = ins.Image;
			_showText.text = ins.Text;
		}

		private void CloseWindow()
		{
			gameObject.SetActive(false);
			SetContnent(0);
		}
	}
}


